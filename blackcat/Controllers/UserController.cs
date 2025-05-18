﻿using Microsoft.AspNetCore.Mvc;
using blackcat.Models;
using blackcat.Models.viewModels;
using blackcat.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
                    ViewBag.Error = "Rol no reconocido.";
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
        public IActionResult OlvideClave(OlvideClaveViewModel model)
        {
            return View();
        }
    }
}
