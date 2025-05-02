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
        public IActionResult ViewUser()
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
            
            string rol = user.IdRolNavigation?.Nombre ?? "";
            
            switch (rol)
            {
                case "Administrador":
                    return RedirectToAction("PagAdmin", "Admin");
                case "Moderador":
                    return RedirectToAction("PagMode", "Moderator");
                case "Publico":
                    return RedirectToAction("ViewUser", "User");
                default:
                    ViewBag.Error = "Rol no reconocido.";
                    return View();
            }
            
            // Aquí puedes guardar info en sesión, cookie, etc.
            return RedirectToAction("ViewUser", "Home");
        }
        
        [HttpPost]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
