using Microsoft.AspNetCore.Mvc;
using blackcat.Services;
using System.Threading.Tasks;

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
            return View();
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
    }
}