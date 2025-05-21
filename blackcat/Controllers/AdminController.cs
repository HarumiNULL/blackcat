using System.Security.Claims;
using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Models.viewModels;
using blackcat.Repositories;
using Microsoft.AspNetCore.Mvc;
using blackcat.Services;
using Microsoft.AspNetCore.Authorization;

namespace blackcat.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly AdminServices _adminServices;
    private readonly BlackcatDbContext _context;
    private readonly UserServices _userService;
    private readonly LibrosServices _librosService;
    private readonly IConfiguration _config;
    private readonly ReportService _reportService;
    private readonly ModServices _modServices;
    private readonly BusquedaRepository _busquedaRepository;
    public AdminController(BlackcatDbContext context, IConfiguration config)
    {
        _modServices = new ModServices(context);
        _context = context;
        _config = config;
        _userService = new UserServices(_context);
        _librosService = new LibrosServices(_context, _config);
        _reportService = new ReportService(_context);
        _busquedaRepository = new BusquedaRepository(_context);
        _adminServices = new AdminServices(_context);
    }

    // GET
    public async Task<IActionResult> PagAdmin()
    {
        int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var nota = await _adminServices.ObtenerNota(idUsuario);
        ViewBag.Nota = nota?.Descrip ?? "";
        return View(nota ?? new InformacionDto());
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GestionarNota(string accion, string contenido)
    {
        int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        if (accion == "borrar")
        {
            await _adminServices.BorrarNota(idUsuario);
            TempData["Mensaje"] = "Nota eliminada correctamente.";
        }
        else if (accion == "guardar")
        {
            await _adminServices.GuardarNota(idUsuario, contenido);
            TempData["Mensaje"] = "Nota guardada correctamente.";
        }

        return RedirectToAction("PagAdmin");
    }
    
    public async Task<IActionResult> ViewListUser(int pg = 1)
    {
        var users = await _userService.GetUsuarios();
        const int pageSize = 5;
        if (pg < 1)
            pg = 1;
        int recsCount = users.Count();
        var pager = new Pager(recsCount,pg, pageSize);
        int recSkip = (pg - 1) * pageSize;
        var data = users.Skip(recSkip).Take(pageSize).ToList();
        this.ViewBag.Pager = pager;
        return View(data);
    }
    public IActionResult ViewRegmod()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ViewRegmod(Usuario usuario)
    {
        var resultado = await _userService.RegistrarUsuarioAsync(usuario,2);
        if (resultado == "Usuario registrado correctamente.")
        {
            TempData["ToastMessage"] = "¡Registro completado correctamente!";
            TempData["ToastType"] = "success";
            return View();
        }

        ViewBag.Error = resultado;
        return View();
    }
    public async Task<IActionResult> ViewBookList(int pg = 1)
    {
        var libros = await _librosService.GetLibros();
        const int pageSize = 5;
        if (pg < 1)
            pg = 1;
        int recsCount = libros.Count();
        var pager = new Pager(recsCount,pg, pageSize);
        int recSkip = (pg - 1) * pageSize;
        var data = libros.Skip(recSkip).Take(pageSize).ToList();
        this.ViewBag.Pager = pager;
        return View(data);
    }
    public IActionResult ViewRegBook()
    {
        return View();
    }
    public async Task<IActionResult> ApproveRulesAdmin()
    {
        return await ReglasPendientes();
    }
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ViewRegBook(LibrosViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var resultado = await _librosService.RegistrarLibroAsync(model);
        if (resultado)
        {
            TempData["ToastMessage"] = "¡Registro completado correctamente!";
            TempData["ToastType"] = "success";
            return View();
        }
        TempData["ToastMessage"] = "¡Registro fallido!";
        TempData["ToastType"] = "success";
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BloquearUsuario(int id)
    {
        var resultado = await _userService.CambiarEstadoUsuarioAsync(id, nuevoEstadoId:3 );
        if (!resultado)
        {
            TempData["Error"] = "No se pudo bloquear al usuario";
        }
        return RedirectToAction("ViewListUser");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DesbloquearUsuario(int id)
    {
        var resultado = await _userService.CambiarEstadoUsuarioAsync(id, nuevoEstadoId: 1);
        if (!resultado)
        {
            TempData["Error"] = "No se pudo desbloquear al usuario";
        }
        return RedirectToAction("ViewListUser");
    }
    
    [HttpPost]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
  
        var resultado = await _userService.EliminarUsuarioAsync(id);
        if (!resultado)
        {
            TempData["Error"] = "No se pudo eliminar al usuario";
        }
        return RedirectToAction("ViewListUser");
    }
    
    public async Task<IActionResult> DescargarReporteBusquedas()
    {
        var busquedas = await _busquedaRepository.ObtenerBusquedasAsync();
        var pdf = _reportService.GenerarReporteBusquedas(busquedas);
        return File(pdf, "application/pdf", "ReporteBusquedas.pdf");
    }
    
    public async Task<IActionResult> DescargarReporteListaLibros()
    {
        var librosL = await _librosService.ListaLibrosAsync();
        var pdf = _reportService.GenerarReporteListaLibros(librosL);
        return File(pdf, "application/pdf", "ReporteListaLibros.pdf");
    }
    
    public async Task<IActionResult> ReglasPendientes()
    {
        var reglas = await _adminServices.ReglasPendientesAsync(); 
        return View("ApproveRulesAdmin", reglas); 
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AprobarRegla(int id)
    {
        var regla = await _modServices.ObtenerReglaPorIdAsync(id);
        if (regla == null)
            return NotFound();

        regla.estadoC = true; // aprobar
        await _modServices.ActualizarReglaAsync(regla);

        TempData["ToastMessage"] = "Regla aprobada correctamente.";
        TempData["ToastType"] = "success";

        return RedirectToAction("ApproveRulesAdmin"); // o la vista que muestra las reglas pendientes
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RechazarRegla(int id)
    {
        await _modServices.RechazarReglaAsync(id);

        TempData["ToastMessage"] = "Regla rechazada correctamente.";
        TempData["ToastType"] = "danger";

        return RedirectToAction("ApproveRulesAdmin");
    }
    
}
