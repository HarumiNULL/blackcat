using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class ModeratorController : Controller
{
    // GET
    public IActionResult PagMode()
    {
        return View();
    }
}