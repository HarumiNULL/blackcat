using blackcat.Models;
using Microsoft.AspNetCore.Mvc;
using blackcat.Services;

namespace blackcat.Controllers;

public class AdminController : Controller
{
    private readonly UserServices _userService;

    public AdminController(UserServices userService)
    {
        _userService = userService;
    }

    // GET
    public IActionResult PagAdmin()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("usuario")))
            return RedirectToAction("ViewLogin", "User");
        return View();
    }


    public IActionResult ViewRegmod()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ViewRegmod(Usuario usuario)
    {
        var resultado = await _userService.RegistrarUsuarioAsync(usuario,2);
        if (resultado == "Usuario registrado correctamente.")
        {
            ViewBag.Mensaje = "¡Registro completado correctamente!";
            return View();
        }

        ViewBag.Error = resultado;
        return View();
    }


}