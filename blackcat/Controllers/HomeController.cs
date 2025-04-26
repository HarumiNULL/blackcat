using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blackcat.Models;
using blackcat.Models.viewModels;
using blackcat.Services;

namespace blackcat.Controllers;

public class HomeController : Controller
{
    private readonly LibrosServices _librosServices;
    
    private readonly ILogger<HomeController> _logger;
    BlackcatDbContext _context ;
    public HomeController(BlackcatDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
        _librosServices = new LibrosServices(_context);
    }
    public async Task<IActionResult> Index(int pg=1)
    {
        var libros = await _librosServices.GetLibros();
        
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

    public IActionResult Catalogo()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}