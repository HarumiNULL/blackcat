﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using blackcat.Services;
using System.Threading.Tasks;
using blackcat.Models.Dtos;
using System.Security.Claims;


namespace blackcat.Controllers
{
    public class ModeratorController : Controller
    {
        
        private readonly ModServices _modServices;

        public ModeratorController(ModServices moderatorService)
        {
            
            _modServices = moderatorService;

        }
        public async Task<IActionResult> PagMode()
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var nota = await _modServices.ObtenerNota(idUsuario);
            ViewBag.Nota = nota?.Descrip ?? "";
            return View(nota ?? new InformacionDto());
        }
        public async Task<IActionResult> ViewForoMod()
        {
            var mensajes = await _modServices.ObtenerMensajesForoAsync(); // ← correcto
            var reglas = await _modServices.ObtenerReglasAsync(); 
            ViewBag.Reglas = reglas; 
            return View(mensajes); // ← pasas el modelo esperado
        }
       
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var mensajes = await _modServices.ObtenerMensajesForoAsync();
            var resultado = await _modServices.EliminarMensajeAsync(id);
            if (!resultado)
                return NotFound();

            return RedirectToAction("ViewForoMod");
        }

        [HttpPost]
        public async Task<IActionResult> BloquearUsuario(int idUsuario)
        {
            var resultado = await _modServices.BloquearUsuarioAsync(idUsuario);
            if (!resultado)
                return NotFound();

            return RedirectToAction("ViewForoMod");
        }
        
       
        [HttpGet]
        public IActionResult CreateRuleMod()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRuleMod(string contenido)
        {
            int idUsuario = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _modServices.RedactarReglaAsync(idUsuario, contenido);

            TempData["ToastMessage"] = "Regla enviada para revisión";
            TempData["ToastType"] = "success";

            return RedirectToAction("CreateRuleMod"); 
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteRuleMod()
        {
            var reglas = await _modServices.ObtenerReglasAsync();
            return View(reglas);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRuleMod(int id)
        {
            await _modServices.EliminarReglaAsync(id);
            return RedirectToAction("DeleteRuleMod");
        }
        
        [HttpGet]
        public async Task<IActionResult> ViewRuleMod()
        {
            var reglas = await _modServices.ObtenerReglasAsync();
            return View(reglas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GestionarNota(string accion, string contenido)
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (accion == "borrar")
            {
                await _modServices.BorrarNota(idUsuario);
                TempData["Mensaje"] = "Nota eliminada correctamente.";
                TempData["ToastType"] = "success";
            }
            else if (accion == "guardar")
            {
                await _modServices.GuardarNota(idUsuario, contenido);
                TempData["Mensaje"] = "Nota guardada correctamente.";
                TempData["ToastType"] = "success";
            }

            return RedirectToAction("PagMode");
        }
        [HttpGet]
        public IActionResult CreateAdsMod()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdsMod(IFormFile ImagenForm)
        {
            var idUsuarioStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idUsuarioStr, out var idUsuario))
            {
                ViewBag.Error = "No se pudo identificar al usuario.";
                return View();
            }

            var exito = await _modServices.RegistrarAnuncioAsync(ImagenForm, idUsuario);

            if (exito)
            {
                ViewBag.Mensaje = "Anuncio enviado para revisión.";
            }
            else
            {
                ViewBag.Error = "Error al subir el anuncio.";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAdsMod()
        {
            var anuncios = await _modServices.ObtenerAnunciosAprobadosAsync();
            return View(anuncios);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdsMod(int id)
        {
            await _modServices.EliminarAnuncioAsync(id);
            return RedirectToAction("DeleteAdsMod");
        }
        public async Task<IActionResult> ViewAdsMod(int id)
        {
            var anuncios = await _modServices.ObtenerAnunciosAprobadosAsync();
            return View(anuncios);
        }
        
        public async Task<IActionResult> Index()
        {
            var anuncios = await _modServices.ObtenerAnunciosAprobadosAsync();
            ViewBag.Anuncios = anuncios;
            return View();
        }
        
    }
}