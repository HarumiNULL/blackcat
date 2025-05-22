using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Services
{
    public class AdminServices 
    {
        
        private readonly BlackcatDbContext _context;
        private readonly InformacionRepository _repository;
        private readonly LibrosRepository _librosRepository;

        public AdminServices(BlackcatDbContext context)
        {
            _context = context;
            _repository = new InformacionRepository(context);
            _librosRepository = new LibrosRepository(context);
        }

        public Task<InformacionDto?> ObtenerNota(int idUsuario) =>
            _repository.ObtenerNotaAsync(idUsuario);

        public Task GuardarNota(int idUsuario, string contenido) =>
            _repository.GuardarNotaAsync(idUsuario, contenido);

        public Task BorrarNota(int idUsuario) =>
            _repository.BorrarNotaAsync(idUsuario);
        
        public async Task<List<Informacion>> ReglasPendientesAsync()
        {
            var reglas = await _context.Informacions
                .Include(i => i.IdUsuarioNavigation)
                .Where(i => i.IdTipoinfo == 1 && !i.estadoC)
                .ToListAsync();

            return reglas;
        }
        
    }
}