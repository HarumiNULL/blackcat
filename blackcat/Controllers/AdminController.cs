using blackcat.Models;
using blackcat.Models.viewModels;
using Microsoft.AspNetCore.Mvc;
using blackcat.Services;
using Microsoft.AspNetCore.Authorization;

namespace blackcat.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly BlackcatDbContext _context;
    private readonly UserServices _userService;
    private readonly LibrosServices _librosService;
    private readonly IConfiguration _config;
    public AdminController(BlackcatDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
        _userService = new UserServices(_context);
        _librosService = new LibrosServices(_context, _config);
    }

    // GET
    public IActionResult PagAdmin()
    {
        return View();
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
}
