using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class LibrosController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}