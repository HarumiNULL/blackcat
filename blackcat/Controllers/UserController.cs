using Microsoft.AspNetCore.Mvc;
using blackcat.Models;
using blackcat.Services;

namespace blackcat.Controllers;

public class UserController : Controller
{
    private readonly IAuthService _authService;

    // Inyección del servicio
    public UserController(IAuthService authService)
    {
        _authService = authService;
    }

    // GET - Página de registro
    public IActionResult Registro()
    {
        return View();
    }

    // POST - Registrar usuario
    [HttpPost]
    public async Task<IActionResult> Registro(Usuario usuario)
    {
        var resultado = await _authService.RegistrarUsuarioAsync(usuario);
        if (resultado == "Usuario registrado correctamente.")
        {
            return RedirectToAction("InicioSesion");  // Redirige al login
        }
        else
        {
            ViewBag.Error = resultado;  // Muestra mensaje de error
            return View("Registro");  // Vuelve a la vista de registro
        }
    }

    // GET - Página de inicio de sesión
    public IActionResult InicioSesion()
    {
        return View();
    }
}