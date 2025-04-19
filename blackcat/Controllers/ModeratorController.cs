using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class ModeratorController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}