using blackcat.Models;
using blackcat.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Repositories
{


    public class BusquedaRepository
    {
        private readonly BlackcatDbContext _context;

        public BusquedaRepository(BlackcatDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusquedaDto>> ObtenerBusquedasAsync()
        {
            var busquedas = await _context.Busqueda
                .Include(b => b.IdEstadoBusNavigation)
                .Include(b => b.IdLibroNavigation)
                .ToListAsync();
            List<BusquedaDto> busquedaDto = new List<BusquedaDto>();
            foreach (var busqueda in busquedas)
            {
                busquedaDto.Add(new BusquedaDto()
                {
                    IdBus = busqueda.IdBus,
                    IdEstadoBus = busqueda.IdEstadoBus,
                    IdLibro = busqueda.IdLibro,
                    NomLib = busqueda.NomLib,
                    CantB = busqueda.CantB,
                    CantBn = busqueda.CantBn,
                    Estado = busqueda.IdEstadoBusNavigation.Estado,
                });
            }
            return busquedaDto;
        }
    }
}