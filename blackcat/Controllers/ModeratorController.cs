using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using blackcat.Services;
using System.Threading.Tasks;
using blackcat.Models.Dtos;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GestionarNota(string accion, string contenido)
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (accion == "borrar")
            {
                await _modServices.BorrarNota(idUsuario);
                TempData["Mensaje"] = "Nota eliminada correctamente.";
            }
            else if (accion == "guardar")
            {
                await _modServices.GuardarNota(idUsuario, contenido);
                TempData["Mensaje"] = "Nota guardada correctamente.";
            }

            return RedirectToAction("PagMode");
        }
        
    }
}