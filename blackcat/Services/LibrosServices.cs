using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Models.viewModels;
using blackcat.Repositories;

namespace blackcat.Services;

public class LibrosServices
{
    private readonly LibrosRepository _librosRepository;
    private readonly IConfiguration _config;

    public LibrosServices(BlackcatDbContext dbContext, IConfiguration config)
    {
        _librosRepository = new LibrosRepository(dbContext);
        _config = config;
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
                    Foto = libroDto.Foto,
                    Archivo = libroDto.Archivo,
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

    public async Task<LibrosViewModel> GetLibro(int id)
    {
        try
        {
            LibrosDto libroDto = await _librosRepository.GetLibroAsync(id);
            
            var libroViewModel = new LibrosViewModel()
            {
                IdL = libroDto.IdL,
                NombreL = libroDto.NombreL,
                Autor = libroDto.Autor,
                Descripcion = libroDto.Descripcion,
                Imagen = libroDto.Imagen,
                Foto = libroDto.Foto,
                Archivo = libroDto.Archivo,
            };
            return libroViewModel;
        }
        catch (SystemException)
        {
            return null!;
        }
    }

    public async Task<bool> AddBookList(int idBook, int idUser)
    {
        var result = await _librosRepository.AddBookList(idBook, idUser);
        return result;
    }
    
    public async Task<bool> RemoveLibroDeLista(int idLibro, int idUsuario)
    {
            return await _librosRepository.RemoveBookFromList(idLibro, idUsuario);
    }
    
    public async Task<bool> RegistrarLibroAsync(LibrosViewModel request, int rol = 3)
    {
        if (request.ArchivoForm == null || request.ArchivoForm.Length < 1
                                        || request.ImagenForm == null || request.ImagenForm.Length < 1)
        {
            return false;
        }

        var rutaLibros = _config["Rutas:Libros"];
        var rutaImagenes = _config["Rutas:Imagenes"];
        
        //Crear los directorios si no existen
        Directory.CreateDirectory(rutaLibros);
        Directory.CreateDirectory(rutaImagenes);
        
       
        // Generar nombres únicos para los archivos
        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(request.ArchivoForm.FileName);
        var nombreImagen = Guid.NewGuid().ToString() + Path.GetExtension(request.ImagenForm.FileName); 
        
        // Guardar Archivos
        var rutaImagen = Path.Combine(rutaImagenes, nombreImagen);
        var rutaArchivoFinal = Path.Combine("wwwroot", rutaLibros, nombreArchivo);
        var rutaImagenFinal = Path.Combine("wwwroot", rutaImagenes, nombreImagen);
        
        using (var stream = new FileStream(rutaArchivoFinal, FileMode.Create))
        {
            await request.ArchivoForm.CopyToAsync(stream);
        }

        using (var stream = new FileStream(rutaImagenFinal, FileMode.Create))
        {
            await request.ImagenForm.CopyToAsync(stream);
        }
        
        var nuevoLibro = new LibrosDto()
        {
            NombreL = request.NombreL,
            Autor = request.Autor,
            Descripcion = request.Descripcion,
            Imagen = rutaImagen,
            Foto = request.Foto,
            Archivo = nombreArchivo
        };
        return await _librosRepository.AddBook(nuevoLibro);
    }
    public async Task<List<LibrosViewModel>> GetLibrosPorUsuario(int idUsuario)
    {
        try
        {
            var librosDtos = await _librosRepository.GetLibrosPorUsuarioAsync(idUsuario);
            var librosViewModels = librosDtos.Select(dto => new LibrosViewModel()
            {
                IdL = dto.IdL,
                NombreL = dto.NombreL,
                Autor = dto.Autor,
                Descripcion = dto.Descripcion,
                Imagen = dto.Imagen,
                Foto = dto.Foto,
                Archivo = dto.Archivo
            }).ToList();

            return librosViewModels;
        }
        catch (SystemException)
        {
            return null!;
        }
    }
    

    public async Task<bool> ExisteLibroEnLista(int idBook, int idUser)
    {
        return await _librosRepository.ExisteLibroEnLista(idBook, idUser);
    }

    public async Task<List<LibrosDto>> BuscarLibrosAsync(string  NombreL)
    {
        return await _librosRepository.BuscarLibrosAsync(NombreL);
    }
    
    public async Task<List<LibrosDto>> ListaLibrosAsync()
    {
        return await _librosRepository.ListaLibrosAsync();
    }

    public async Task<bool> ModificarLibroAsync(LibrosViewModel libroModificado)
    {
        var rutaLibros = _config["Rutas:Libros"];
        var rutaImagenes = _config["Rutas:Imagenes"];
        var lib = await _librosRepository.GetLibroAsync(libroModificado.IdL);
        var rutaEliminarArchivo = Path.Combine("wwwroot", rutaLibros, lib.Archivo);
        var rutaEliminarImagen = Path.Combine("wwwroot", lib.Imagen.TrimStart('/'));
        if (File.Exists(rutaEliminarArchivo))
        {
            File.Delete(rutaEliminarArchivo);
        }

        if (File.Exists(rutaEliminarImagen))
        {
            File.Delete(rutaEliminarImagen);
        }
        if (libroModificado.ArchivoForm == null || libroModificado.ArchivoForm.Length < 1
                                        || libroModificado.ImagenForm == null || libroModificado.ImagenForm.Length < 1)
        {
            return false;
        }

        
        //Crear los directorios si no existen
        Directory.CreateDirectory(rutaLibros);
        Directory.CreateDirectory(rutaImagenes);
        
       
        // Generar nombres únicos para los archivos
        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(libroModificado.ArchivoForm.FileName);
        var nombreImagen = Guid.NewGuid().ToString() + Path.GetExtension(libroModificado.ImagenForm.FileName); 
        
        // Guardar Archivos
        var rutaImagen = Path.Combine(rutaImagenes, nombreImagen);
        var rutaArchivoFinal = Path.Combine("wwwroot", rutaLibros, nombreArchivo);
        var rutaImagenFinal = Path.Combine("wwwroot", rutaImagenes, nombreImagen);
        
        using (var stream = new FileStream(rutaArchivoFinal, FileMode.Create))
        {
            await libroModificado.ArchivoForm.CopyToAsync(stream);
        }

        using (var stream = new FileStream(rutaImagenFinal, FileMode.Create))
        {
            await libroModificado.ImagenForm.CopyToAsync(stream);
        }
        var libro = new LibrosDto()
        {
            IdL = libroModificado.IdL,
            NombreL = libroModificado.NombreL,
            Autor = libroModificado.Autor,
            Descripcion = libroModificado.Descripcion,
            Archivo = nombreArchivo,
            Imagen = rutaImagen,
        };
        return await _librosRepository.UpdateBook(libro);
    }

    public async Task<bool> EliminarLibroAsync(int idL)
    {
        var libro = await _librosRepository.GetLibroAsync(idL);
        if (libro == null) return false;

        var rutaArchivo = Path.Combine("wwwroot", _config["Rutas:Libros"], libro.Archivo);
        var rutaImagen = Path.Combine("wwwroot", libro.Imagen.TrimStart('/'));

        // Eliminar archivos si existen
        if (File.Exists(rutaArchivo))
            File.Delete(rutaArchivo);

        if (File.Exists(rutaImagen))
            File.Delete(rutaImagen);

        // ✅ Eliminar libro
        return await _librosRepository.EliminarLibroAsync(idL);
    }
}