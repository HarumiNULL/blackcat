using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Repositories;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Services
{
    public class ModServices
    {
        private readonly BlackcatDbContext _context;
        private readonly InformacionRepository _repository;

        public ModServices(BlackcatDbContext context)
        {
            _context = context;
            _repository = new InformacionRepository(_context);
        }

        public async Task<List<Informacion>> ObtenerMensajesForoAsync()
        {
            return await _context.Informacions
                .Include(i => i.IdUsuarioNavigation)
                .Where(i => i.IdTipoinfo == 3)
                .OrderBy(i => i.FechaI)
                .ToListAsync();
        }

        public async Task<bool> EliminarMensajeAsync(int id)
        {
            var mensaje = await _context.Informacions.FindAsync(id);
            if (mensaje == null)
                return false;

            _context.Informacions.Remove(mensaje);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BloquearUsuarioAsync(int idUsuario)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return false;

            usuario.IdEstado = 3; // Suponiendo que 3 es el estado bloqueado
            await _context.SaveChangesAsync();
            return true;
        }
        public Task<InformacionDto?> ObtenerNota(int idUsuario) =>
            _repository.ObtenerNotaAsync(idUsuario);

        public Task GuardarNota(int idUsuario, string contenido) =>
            _repository.GuardarNotaAsync(idUsuario, contenido);

        public Task BorrarNota(int idUsuario) =>
            _repository.BorrarNotaAsync(idUsuario);
    }
}