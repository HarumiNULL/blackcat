using System.Security.Claims;
using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Models.viewModels;
using blackcat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class LibrosController : Controller
{
    BlackcatDbContext _context;
    private readonly IConfiguration _config;
    LibrosServices _librosServices;

    public LibrosController(BlackcatDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
        _librosServices = new LibrosServices(_context, config);
    }

    // GET
    public IActionResult Index(int pg = 1)
    {
        List<Libro?> Libros = _context.Libros.ToList();

        const int pageSize = 5;
        if (pg < 1)
            pg = 1;
        int recsCount = Libros.Count();
        var pager = new Pager(recsCount, pg, pageSize);
        int recSkip = (pg - 1) * pageSize;
        var data = Libros.Skip(recSkip).Take(pageSize).ToList();
        this.ViewBag.Pager = pager;

        return View(data);
    }

    public async Task<IActionResult> View(int id)
    {
        // var user = HttpContext.User.Claims.First().Value;
        var libro = await _librosServices.GetLibro(id);
        if (User.Identity?.IsAuthenticated == true)
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            bool estaEnLista = await _librosServices.ExisteLibroEnLista(id, idUsuario);
            ViewData["estaEnLista"] = estaEnLista;
        }

        return View(libro);
    }

    public IActionResult ReadArchive(string nombreArchivo)
    {
        try
        {
            // Validar nombre de archivo por seguridad
            if (string.IsNullOrEmpty(nombreArchivo) || nombreArchivo.Contains(".."))
            {
                return BadRequest("Nombre de archivo no válido");
            }

            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, "wwwroot", "staticFiles", "books", nombreArchivo);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Determinar el tipo MIME basado en la extensión del archivo
            string contentType;
            var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();

            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".epub":
                    contentType = "application/epub+zip";
                    break;
                default:
                    return BadRequest("Tipo de archivo no soportado");
            }

            // Forzar descarga (en lugar de abrir en el navegador)
            return PhysicalFile(filePath, contentType);
        }
        catch (Exception e)
        {
            return BadRequest($"Error al leer el archivo: {e.Message}");
        }
    }

    [Authorize]
    public async Task<IActionResult> AddBookList(int idBook)
    {
        int? idUser = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _librosServices.AddBookList(idBook, idUser.Value);
        if (result)
        {
            TempData["ToastMessage"] = "Libro Añadido exitosamente";
            TempData["ToastType"] = "success";
            return RedirectToAction("View", new { id = idBook });
        }
        TempData["ToastMessage"] = "Libro no ha podido ser añadido";
        TempData["ToastType"] = "error";
        return RedirectToAction("View", new { id = idBook });
    }
    
    public async Task<IActionResult> Buscar(string nombreLibro)
    {
        var libros = await _librosServices.BuscarLibrosAsync(nombreLibro);
        return View("ViewListBooks", libros); // o el nombre de tu vista
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarDeLista([FromBody] LibrosDto dto)
    {
        try
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (idUsuario == 0)
            {
                return Json(new { success = false, message = "Usuario no autenticado" });
            }

            var resultado = await _librosServices.RemoveLibroDeLista(dto.IdL, idUsuario);

            if (resultado)
            {
                return Json(new { success = true, message = "Libro eliminado correctamente" });
            }
            else
            {
                return Json(new { success = false, message = "No se pudo eliminar el libro" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error interno: " + ex.Message);
            return Json(new { success = false, message = "Error en el servidor: " + ex.Message });
        }
    }

}

