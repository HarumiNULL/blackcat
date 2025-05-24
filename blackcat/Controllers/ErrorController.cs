using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class ErrorController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}