using Microsoft.AspNetCore.Mvc;
using blackcat.Models;
using blackcat.Repositories;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Controllers
{
    public class ForoController : Controller
    {
        private readonly BlackcatDbContext _context;
        private readonly UserRepository _userRepository;

        public ForoController(BlackcatDbContext context, UserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }
        
        [HttpPost]
        public async Task<IActionResult> Publicar(string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                TempData["ToastMessage"] = "El contenido no puede estar vacío.";
                TempData["ToastType"] = "error";
                return RedirectToAction("ViewForoUser");
            }

            var nombreUsuario = User.Identity?.Name;
            if (string.IsNullOrEmpty(nombreUsuario))
                return Unauthorized();

            var usuario = await _userRepository.ObtenerUsuAsync(nombreUsuario);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            var nuevo = new Informacion()
            {
                IdUsuario = usuario.IdU,
                IdTipoinfo = 3,
                Descrip = contenido,
                FechaI = DateTime.Now
            };

            _context.Informacions.Add(nuevo);
            await _context.SaveChangesAsync();

            TempData["ToastMessage"] = "Mensaje publicado correctamente.";
            TempData["ToastType"] = "success";

            return RedirectToAction("ViewForoUser", "User");

        }

    }
}