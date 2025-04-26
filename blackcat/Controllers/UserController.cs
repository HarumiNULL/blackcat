using Microsoft.AspNetCore.Mvc;
using blackcat.Models;
using blackcat.Services;

namespace blackcat.Controllers
{
    public class UserController : Controller
    {
        private readonly UserServices _userService;

        public UserController(UserServices userService)
        {
            _userService = userService;
        }

        public IActionResult Registro()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            var resultado = await _userService.RegistrarUsuarioAsync(usuario);
            if (resultado == "Usuario registrado correctamente.")
                return RedirectToAction("InicioSesion");

            ViewBag.Error = resultado;
            return View();
        }

        public IActionResult InicioSesion()
        {
            return View();
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> InicioSesion(Usuario usuario)
        {
            var user = await _userService.IniciarSesionAsync(usuario.NombreU, usuario.Cont);
    
            if (user == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }

            // Aquí puedes guardar info en sesión, cookie, etc.
            return RedirectToAction("ViewUser", "Home");
        }

    }
}
