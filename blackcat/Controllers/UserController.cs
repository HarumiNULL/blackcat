using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using blackcat.Models;
using blackcat.Models.viewModels;
using blackcat.Repositories;
using blackcat.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Controllers
{
    public class UserController : Controller
    {
        private readonly UserServices _userService;
        private readonly LibrosServices _librosService;
        private readonly BlackcatDbContext _context;

        public UserController(BlackcatDbContext context, IConfiguration config)
        {
            _context = context;
            _userService = new UserServices(context);
            _librosService = new LibrosServices(context, config);
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
                TempData["ToastMessage"] = "Correo o contraseña incorrectos.";
                TempData["ToastType"] = "error";
                return View();
            }

            if (user.IdEstado == 3)
            {
                TempData["ToastMessage"] = "Usuario bloqueado";
                TempData["ToastType"] = "error";
                return View();
            }

            var result = await _userService.CreateCredentials(user, false, HttpContext);

            string? rol = user.Rol;

            switch (rol)
            {
                case "Administrador":
                    return RedirectToAction("PagAdmin", "Admin");
                case "Moderador":
                    return RedirectToAction("PagMode", "Moderator");
                case "Publico":
                    return RedirectToAction("ViewUser", "User");
                default:
                    TempData["ToastMessage"] = "Rol no reconocido.";
                    TempData["ToastType"] = "error";
                    return View();
            }

            // Aquí puedes guardar info en sesión, cookie, etc.
            return RedirectToAction("ViewUser", "Home");
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Puedes cambiar la vista a donde quieras redirigir
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult OlvideClave()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> OlvideClave(OlvideClaveViewModel model)
        {
            var result = await _userService.OlvideClave(model.Correo!);
            if (!result)
            {
                TempData["ToastMessage"] = "Error enviando el correo, intentenlo de nuevo";
                TempData["ToastType"] = "error";
                return View(model);
            }

            TempData["ToastMessage"] = "Ingrese a su correo electronico para continuar con el proceso";
            TempData["ToastType"] = "success";
            return View(nameof(ViewLogin));
        }

        public async Task<IActionResult> RecuperarContrasena()
        {
            string recoveryToken = Request.Query["token"].ToString();
            var result = await _userService.ExisteTokenRecuperacion(recoveryToken);
            if (!result || recoveryToken == "")
            {
                TempData["ToastMessage"] = "Token invalido o ya usado";
                TempData["ToastType"] = "error";
                return View("ViewLogin");
            }

            ViewData["Token"] = recoveryToken;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RecuperarContrasena(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.CambiarContrasena(model);
            if (!result)
            {
                TempData["ToastMessage"] = "Error intentalo de Nuevo";
                TempData["ToastType"] = "error";
                return View(model);
            }

            TempData["ToastMessage"] = "Contraseña actualizada";
            TempData["ToastType"] = "success";
            return View("ViewLogin");
        }
        
        public IActionResult ViewForoUser()
        {
            var mensajes = _context.Informacions
                .Include(i => i.IdUsuarioNavigation)
                .Where(i => i.IdTipoinfo == 3)
                .OrderBy(i => i.FechaI)
                .ToList();

            return View(mensajes);

        }

        public async Task<IActionResult> ViewMyList()
        {
            int? idUsuario = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var libros = await _librosService.GetLibrosPorUsuario(idUsuario.Value);
            return View(libros);
        }
    }
}