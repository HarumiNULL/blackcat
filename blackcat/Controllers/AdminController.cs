using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

public class AdminController : Controller
{
    // GET
    public IActionResult PagAdmin()
    {
        return View();
    }

}