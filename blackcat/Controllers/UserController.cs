using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}