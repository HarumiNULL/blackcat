using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Repositories;

namespace blackcat.Services
{
    public class AdminServices
    {
        private readonly InformacionRepository _repository;

        public AdminServices(BlackcatDbContext context)
        {
            
            _repository = new InformacionRepository(context);
        }

        public Task<InformacionDto?> ObtenerNota(int idUsuario) =>
            _repository.ObtenerNotaAsync(idUsuario);

        public Task GuardarNota(int idUsuario, string contenido) =>
            _repository.GuardarNotaAsync(idUsuario, contenido);

        public Task BorrarNota(int idUsuario) =>
            _repository.BorrarNotaAsync(idUsuario);
    }
}