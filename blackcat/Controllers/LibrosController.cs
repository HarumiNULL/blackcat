﻿using blackcat.Models;
using blackcat.Models.viewModels;
using blackcat.Services;
using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class LibrosController : Controller
{ 
    BlackcatDbContext _context ;
    private readonly IConfiguration _config;
    LibrosServices _librosServices;
    public LibrosController(BlackcatDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
        _librosServices = new LibrosServices(_context, config);
    }
    // GET
    public IActionResult Index(int pg=1)
    {
        List<Libro?> Libros = _context.Libros.ToList();
        
        const int pageSize = 5;
        if (pg < 1)
            pg = 1;
        int recsCount = Libros.Count();
        var pager = new Pager(recsCount,pg, pageSize);
        int recSkip = (pg - 1) * pageSize;
        var data = Libros.Skip(recSkip).Take(pageSize).ToList();
        this.ViewBag.Pager = pager;
        
        return View(data);
    }

    public async Task<IActionResult> View(int id)
    {
        // var user = HttpContext.User.Claims.First().Value;
        var libro = await _librosServices.GetLibro(id);
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

    public async Task<IActionResult> AddBookList(int idBook)
    {
        int? idUser = HttpContext.Session.GetInt32("Id");
        if (idUser == null)
        {
            return RedirectToAction("ViewLogin", "User");
        }
        var result = await _librosServices.AddBookList(idBook, idUser.Value);
        if (result)
        {
            ViewBag.Mensaje = "Libro Añadido exitosamente";
            return RedirectToAction("View", new { id = idBook });
        }
        ViewBag.Error = "Libro no ha podido ser añadido";
        return RedirectToAction("View", new { id = idBook });
    }
}