using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Models.viewModels;
using blackcat.Repositories;

namespace blackcat.Services;

public class LibrosServices
{
    private readonly LibrosRepository _librosRepository;

    public LibrosServices(BlackcatDbContext dbContext)
    {
        _librosRepository = new LibrosRepository(dbContext);
    }
    
    public async Task<List<LibrosViewModel>> GetLibros()
    {
        try
        {
            List<LibrosDto> librosDtos = await _librosRepository.ListaLibrosAsync();
            
            List<LibrosViewModel> libroViewModels = new List<LibrosViewModel>();
            foreach (var libroDto in librosDtos)
            {
                var libroViewModel = new LibrosViewModel()
                {
                    IdL = libroDto.IdL,
                    NombreL = libroDto.NombreL,
                    Autor = libroDto.Autor,
                    Descripcion = libroDto.Descripcion,
                    Imagen = libroDto.Imagen,
                    Foto = libroDto.Foto
                };
                libroViewModels.Add(libroViewModel);
            }

            return libroViewModels;
        }
        catch (SystemException)
        {
            return null!;
        }
    }
}