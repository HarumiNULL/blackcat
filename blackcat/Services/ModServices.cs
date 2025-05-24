using blackcat.Models;
using blackcat.Repositories;
using blackcat.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Services
{
    public class ModServices
    {
        private readonly BlackcatDbContext _context;
        private readonly IConfiguration _config;
        private readonly InformacionRepository _repository;

        public ModServices(BlackcatDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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
        
        public async Task<bool> RedactarReglaAsync(int idUsuario, string contenido)
        {
            var nuevaRegla = new Informacion
            {
                IdUsuario = idUsuario,
                Descrip = contenido,
                FechaI = DateTime.Now,
                IdTipoinfo = 1, // tipo regla
                estadoC = false
            };

            _context.Informacions.Add(nuevaRegla);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EliminarReglaAsync(int id)
        {
            var regla = await _context.Informacions
                .FirstOrDefaultAsync(i => i.IdInfo== id && i.IdTipoinfo == 1);

            if (regla == null) return false;

            _context.Informacions.Remove(regla);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Informacion>> ObtenerReglasAsync()
        {
            return await _context.Informacions
                .Where(i => i.IdTipoinfo == 1 && i.estadoC == true)  // Solo reglas aprobadas (estadoC = true)
                .OrderBy(i => i.FechaI)
                .ToListAsync();
        }
        public async Task<Informacion?> ObtenerReglaPorIdAsync(int id)
        {
            return await _context.Informacions.FirstOrDefaultAsync(r => r.IdInfo == id && r.IdTipoinfo == 1);
        }

        public async Task ActualizarReglaAsync(Informacion regla)
        {
            _context.Informacions.Update(regla);
            await _context.SaveChangesAsync();
        }
        public async Task RechazarReglaAsync(int id)
        {
            var regla = await _context.Informacions.FindAsync(id);
            if (regla != null)
            {
                _context.Informacions.Remove(regla);
                await _context.SaveChangesAsync();
            }
        }

        
        public Task<InformacionDto?> ObtenerNota(int idUsuario) =>
            _repository.ObtenerNotaAsync(idUsuario);

        public Task GuardarNota(int idUsuario, string contenido) =>
            _repository.GuardarNotaAsync(idUsuario, contenido);

        public Task BorrarNota(int idUsuario) =>
            _repository.BorrarNotaAsync(idUsuario);
        
        public async Task<bool> RegistrarAnuncioAsync(IFormFile ImagenForm, int idUsuario)
        {
            if (ImagenForm == null || ImagenForm.Length < 1)
                return false;

            var rutaAnuncios = _config["Rutas:Anuncios"]; // agrega esto en appsettings.json
            Directory.CreateDirectory(rutaAnuncios);

            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ImagenForm.FileName);
            var rutaCompleta = Path.Combine("wwwroot", rutaAnuncios, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await ImagenForm.CopyToAsync(stream);
            }

            var nuevoAnuncio = new Informacion
            {
                IdUsuario = idUsuario,
                Descrip = Path.Combine(rutaAnuncios, nombreArchivo), // guarda solo ruta relativa
                FechaI = DateTime.Now,
                IdTipoinfo = 2, // 2 = Anuncio
                estadoC = false // pendiente de revisión
            };

            _context.Informacions.Add(nuevoAnuncio);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarAnuncioAsync(int id)
        {
            var anuncio = await _context.Informacions.FindAsync(id);
            if (anuncio == null || anuncio.IdTipoinfo != 2) return false;

            // Eliminar el archivo físico
            var rutaFisica = Path.Combine("wwwroot", anuncio.Descrip);
            if (File.Exists(rutaFisica))
                File.Delete(rutaFisica);

            _context.Informacions.Remove(anuncio);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Informacion>> ObtenerAnunciosAprobadosAsync()
        {
            return await _context.Informacions
                .Include(i => i.IdUsuarioNavigation)
                .Where(i => i.IdTipoinfo == 2 && i.estadoC == true)
                .OrderByDescending(i => i.FechaI)
                .ToListAsync();
        }
    }
}