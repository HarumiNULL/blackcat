using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Utilities;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Repositories;

public class LibrosRepository
{
    private readonly BlackcatDbContext _context;

    public LibrosRepository(BlackcatDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<LibrosDto>> ListaLibrosAsync()
    {

        try
        {
            List<Libro?> libros = await _context.Libros.ToListAsync();
            List<LibrosDto> librosDtos = new List<LibrosDto>();
            foreach (var libro in libros)
            {
                byte[]? imageBytes;
                imageBytes =
                    new PhotoUtilities().GetPhotoFromFile(Directory.GetCurrentDirectory() + "/wwwroot/" +
                                                          libro.Imagen!);

                var libroDto = new LibrosDto()
                {
                    IdL = libro.IdL,
                    NombreL = libro.NombreL,
                    Autor = libro.Autor,
                    Descripcion = libro.Descripcion,
                    Imagen = libro.Imagen,
                    Archivo = libro.Archivo,
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

    public async Task<LibrosDto> GetLibroAsync(int id)
    {
        try
        {
            Libro? libro = await _context.Libros.FindAsync(id);
            byte[]? imageBytes;
            imageBytes =
                new PhotoUtilities().GetPhotoFromFile(Directory.GetCurrentDirectory() + "/wwwroot/" + libro?.Imagen!);

            if (libro != null)
            {
                var libroDto = new LibrosDto()
                {
                    IdL = libro.IdL,
                    NombreL = libro.NombreL,
                    Autor = libro.Autor,
                    Descripcion = libro.Descripcion,
                    Imagen = libro.Imagen,
                    Foto = imageBytes,
                    Archivo = libro.Archivo
                };
                return libroDto;
            }

            return null!;
        }
        catch (SystemException)
        {
            return null!;
        }
    }

    public async Task<bool> AddBookList(int idBook, int idUser)
    {
        try
        {
            var exists = await _context.ListaUs.Where(l => l.IdLibro == idBook && l.IdUsuario == idUser).AnyAsync();
            if (exists)
                return false;
            var newComment = new ListaU()
            {
                IdLibro = idBook,
                IdUsuario = idUser
            };
            await _context.ListaUs.AddAsync(newComment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (SystemException)
        {
            return false;
        }
    }
    
    public async Task<bool> RemoveBookFromList(int idBook, int idUser)
    {
        try
        {
            // Buscar el registro específico en la lista
            var listItem = await _context.ListaUs
                .FirstOrDefaultAsync(l => l.IdLibro == idBook && l.IdUsuario == idUser);

            if (listItem == null)
                return false; // No existe en la lista

            // Eliminar el registro
            _context.ListaUs.Remove(listItem);
            await _context.SaveChangesAsync();
        
            return true; // Eliminación exitosa
        }
        catch (Exception ex) 
        {
            // Log del error 
            Console.WriteLine($"Error al eliminar libro de la lista: {ex.Message}");
            return false;
        }
    }
    

    public async Task<bool> AddBook(LibrosDto librosDto)
    {
        try
        {
            var libro = new Libro()
            {
                NombreL = librosDto.NombreL,
                Autor = librosDto.Autor,
                Descripcion = librosDto.Descripcion,
                Imagen = librosDto.Imagen,
                Archivo = librosDto.Archivo
            };
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (SystemException)
        {
            return false;
        }
    }

    public async Task<List<LibrosDto>> GetLibrosPorUsuarioAsync(int idUsuario)
    {
        try
        {
            var libros = await _context.ListaUs
                .Where(lu => lu.IdUsuario == idUsuario)
                .Include(lu => lu.IdLibroNavigation) // Incluye la navegación a Libro
                .ToListAsync();
            return libros.Select(l => new LibrosDto()
            {
                IdL = l.IdLibroNavigation.IdL,
                NombreL = l.IdLibroNavigation.NombreL,
                Autor = l.IdLibroNavigation.Autor,
                Descripcion = l.IdLibroNavigation.Descripcion,
                Imagen = l.IdLibroNavigation.Imagen,
                Foto = new PhotoUtilities().GetPhotoFromFile(Directory.GetCurrentDirectory() + "/wwwroot/" +
                                                             l.IdLibroNavigation.Imagen!),
                Archivo = l.IdLibroNavigation.Archivo
            }).ToList();

        }
        catch (SystemException)
        {
            return null!;
        }
    }

    public async Task<bool> ExisteLibroEnLista(int idBook, int idUser)
    {
        return await _context.ListaUs.AnyAsync(l => l.IdLibro == idBook && l.IdUsuario == idUser);
    }

    public async Task<List<LibrosDto>> BuscarLibrosAsync(string nombreLibro)
    {
        try
        {
            var libros = await _context.Libros
                .Where(l => l.NombreL!.Contains(nombreLibro))
                .ToListAsync();

            List<LibrosDto> librosDtos = new();

            foreach (var libro in libros)
            {
                // Carga imagen en bytes
                byte[]? imageBytes;
                imageBytes =
                    new PhotoUtilities().GetPhotoFromFile(Directory.GetCurrentDirectory() + "/wwwroot/" +
                                                          libro.Imagen!);

                librosDtos.Add(new LibrosDto
                {
                    IdL = libro.IdL,
                    NombreL = libro.NombreL,
                    Autor = libro.Autor,
                    Descripcion = libro.Descripcion,
                    Imagen = libro.Imagen,
                    Foto = imageBytes,
                    Archivo = libro.Archivo
                });

                // 🔄 REGISTRO EN BUSQUEDUM
                var busquedaExistente = await _context.Busqueda
                    .FirstOrDefaultAsync(b => b.IdLibro == libro.IdL);

                if (busquedaExistente != null)
                {
                    // Actualiza contador
                    busquedaExistente.CantB = (busquedaExistente.CantB ?? 0) + 1;
                    _context.Busqueda.Update(busquedaExistente);
                }
                else
                {
                    // Nueva búsqueda
                    var nuevaBusqueda = new Busquedum
                    {
                        IdLibro = libro.IdL,
                        NomLib = libro.NombreL,
                        CantB = 1,
                        CantBn = 0,
                        IdEstadoBus = 1 // o el estado predeterminado
                    };
                    await _context.Busqueda.AddAsync(nuevaBusqueda);
                }
            }

            await _context.SaveChangesAsync();
            return librosDtos;
        }
        catch (Exception)
        {
            return new List<LibrosDto>();
        }
    }

    public async Task<List<LibrosDto>> ListalibrosAsync()
    {
        try
        {
            List<Libro> librosL = await _context.Libros.ToListAsync();
            List<LibrosDto> librosDtos = new List<LibrosDto>();

            foreach (var l in librosL) // Cambiado de librosDtos a librosL
            {
                byte[]? imageBytes = null;

                if (!string.IsNullOrEmpty(l.Imagen))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", l.Imagen);
                    imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                }

                librosDtos.Add(new LibrosDto()
                {
                    IdL = l.IdL,
                    NombreL = l.NombreL,
                    Autor = l.Autor,
                    Descripcion = l.Descripcion,
                    Imagen = l.Imagen,
                    Foto = imageBytes
                });
            }

            return librosDtos; // Mover el return fuera del foreach
        }
        catch (Exception ex)
        {
            // Manejo de errores recomendado
            Console.WriteLine($"Error al obtener libros: {ex.Message}");
            return new List<LibrosDto>();
        }
    }

    public async Task<bool> UpdateBook(LibrosDto libros)
    {
        var l = await _context.Libros.FindAsync(libros.IdL);
        l.IdL = libros.IdL;
        l.NombreL = libros.NombreL;
        l.Autor = libros.Autor;
        l.Descripcion = libros.Descripcion;
        l.Archivo = libros.Archivo;
        l.Imagen = libros.Imagen;
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarLibroAsync(int idL)
    {
        var libro = await _context.Libros.FindAsync(idL);

        if (libro == null) return false;
        var busquedas = _context.Busqueda.Where(b => b.IdLibro == idL);
        _context.Busqueda.RemoveRange(busquedas);
        var lista  = _context.ListaUs.Where(l => l.IdLibro == idL);
        _context.ListaUs.RemoveRange(lista);
        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();
        return true;

    }


}
    