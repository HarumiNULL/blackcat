using System.Security.Claims;
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