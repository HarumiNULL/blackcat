using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Utilities;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Repositories;

public class LibrosRepository
{
    private readonly BlackcatDbContext _context;

    public LibrosRepository(BlackcatDbContext  dbContext)
    {
        _context = dbContext;
    }
    public async Task<List<LibrosDto>> ListaLibrosAsync()
    {
        
        try
        {
            List<Libro> libros = await _context.Libros.ToListAsync();
            List<LibrosDto> librosDtos = new List<LibrosDto>();
            foreach (var libro in libros)
            {
                byte[]? imageBytes;
                imageBytes = new PhotoUtilities().GetPhotoFromFile(Directory.GetCurrentDirectory() + "/wwwroot/" + libro.Imagen!);
                
                var  libroDto = new LibrosDto()
                {
                    IdL = libro.IdL,
                    NombreL = libro.NombreL,
                    Autor = libro.Autor,
                    Descripcion = libro.Descripcion,
                    Imagen = libro.Imagen,
                    Foto = imageBytes
                    
                };
                librosDtos.Add(libroDto);
            }

            return librosDtos;
        }
        catch (SystemException)
        {
            return null!;
        }
    }
}