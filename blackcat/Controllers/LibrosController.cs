using blackcat.Models;
using blackcat.Models.viewModels;
using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class LibrosController : Controller
{ 
    BlackcatDbContext _context ;
    public LibrosController(BlackcatDbContext context)
    {
        _context = context;
    }
    // GET
    public IActionResult Index(int pg=1)
    {
        List<Libro> Libros = _context.Libros.ToList();
        
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
}