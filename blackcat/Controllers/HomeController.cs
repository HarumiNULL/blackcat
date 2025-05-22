using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blackcat.Models;
using blackcat.Models.viewModels;
using blackcat.Services;

namespace blackcat.Controllers;

public class HomeController : Controller
{
    private readonly LibrosServices _librosServices;
    private readonly IConfiguration _config;
    
    private readonly ILogger<HomeController> _logger;
    BlackcatDbContext _context ;
    public HomeController(BlackcatDbContext context, ILogger<HomeController> logger, IConfiguration config)
    {
        _context = context;
        _config = config;
        _logger = logger;
        _librosServices = new LibrosServices(_context, _config);
    }
    public async Task<IActionResult> Index(int pg=1)
    {
        var libros = await _librosServices.GetLibros();
        
        if (libros == null)
            libros = new List<LibrosViewModel>();
        const int pageSize = 6;
        if (pg < 1)
            pg = 1;
        int recsCount = (libros != null)?libros.Count(): 0;
        var pager = new Pager(recsCount,pg, pageSize);
        int recSkip = (pg - 1) * pageSize;
        var data = libros.Skip(recSkip).Take(pageSize).ToList();
        this.ViewBag.Pager = pager;
        
        return View(data);
    }

    public async Task<IActionResult> Catalogo(int pg=1)
    {
        var libros = await _librosServices.GetLibros();
        
        const int pageSize = 6;
        if (pg < 1)
            pg = 1;
        int recsCount = (libros != null)?libros.Count(): 0;
        var pager = new Pager(recsCount,pg, pageSize);
        int recSkip = (pg - 1) * pageSize;
        var data = libros.Skip(recSkip).Take(pageSize).ToList();
        this.ViewBag.Pager = pager;
        
        return View(data);
    }
    public IActionResult ViewUser()
    {
        return View();
    }

    public IActionResult ViewForoUser()
    {
        return View();
    }
    public IActionResult ViewForoMod()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}