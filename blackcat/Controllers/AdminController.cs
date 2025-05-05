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
    //Hay que agregar el cambio de rol y agregar un Modal(Ventana emergente donde indique usuario registrado)
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ViewRegmod(Usuario usuario)
    {
        var resultado = await _userService.RegistrarUsuarioAsync(usuario);
        if (resultado == "Usuario registrado correctamente.")
            return RedirectToAction("InicioSesion");

        ViewBag.Error = resultado;
        return View();
    }


}