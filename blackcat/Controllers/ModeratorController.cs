using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blackcat.Controllers;

[Authorize(Roles = "Administrador,Moderador")]
public class ModeratorController : Controller
{
    // GET
    public IActionResult PagMode()
    {
        return View();
    }
}