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
                return RedirectToAction("ViewLogin");

            ViewBag.Error = resultado;
            return View();
        }

        public IActionResult ViewLogin()
        {
            return View();
        }
        public IActionResult ViewUser()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ViewLogin(Usuario usuario)
        {
            var user = await _userService.IniciarSesionAsync(usuario.NombreU, usuario.Cont);
    
            if (user == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }

            if (user.IdEstado == 3)
            {
                ViewBag.Error = "Usuario bloqueado";
                return View();
            }

            HttpContext.Session.SetString("usuario", user.NombreU);
            HttpContext.Session.SetString("rol", user.IdRolNavigation?.Nombre ?? "");
            HttpContext.Session.SetInt32("Id", user.IdU);
            
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
        
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear(); // 🧹 Limpia todos los datos de la sesión
            return RedirectToAction("Index", "Home"); // Puedes cambiar la vista a donde quieras redirigir
        }

    }
}
