﻿using System.Security.Claims;
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
        private readonly ModServices _modServices;

        public UserController(BlackcatDbContext context, IConfiguration config, ModServices modServices)
        {
            _modServices = modServices;
            _userService = new UserServices(context);
            _librosService = new LibrosServices(context, config);
        }

        public IActionResult Registro()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Registro(UserViewModel usuario)
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

        public async Task<IActionResult> ViewUser(int pg=1)
        {
            var libros = await _librosService.GetLibros();
        
            if (libros == null)
                libros = new List<LibrosViewModel>();
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int recsCount = (libros != null)?libros.Count(): 0;
            var pager = new Pager(recsCount,pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = libros.Skip(recSkip).Take(pageSize).ToList();
            this.ViewBag.Pager = pager;
            var anuncios = await _modServices.ObtenerAnunciosAprobadosAsync();
            ViewBag.Anuncios = anuncios;
            return View(data);
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ViewLogin(LoginViewModel usuario)
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
            TempData["ToastMessage"] = "Inicio de sesion exitoso!";
            TempData["ToastType"] = "success";
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
        
        public async Task<IActionResult> ViewForoUser()
        {
            var mensajes = await _userService.ObtenerMensajesForoAsync(); 
            var reglas = await _modServices.ObtenerReglasAsync(); 
            ViewBag.Reglas = reglas; 
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