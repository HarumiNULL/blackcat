using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class ForoController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}