using blackcat.Models;
using blackcat.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Repositories;

public class InformacionRepository
{
    private readonly BlackcatDbContext _context;

    public InformacionRepository(BlackcatDbContext context)
    {
        _context = context;
    }

    public async Task<InformacionDto?> ObtenerNotaAsync(int idUsuario)
    {
        return await _context.Informacions
            .Where(i => i.IdUsuario == idUsuario && i.IdTipoinfo == 4)
            .Select(i => new InformacionDto
            {
                IdInfo = i.IdInfo,
                IdUsuario = i.IdUsuario,
                IdTipoinfo = i.IdTipoinfo,
                Descrip = i.Descrip
            })
            .FirstOrDefaultAsync();
    }

    public async Task GuardarNotaAsync(int idUsuario, string contenido)
    {
        var nota = await _context.Informacions
            .FirstOrDefaultAsync(i => i.IdUsuario == idUsuario && i.IdTipoinfo == 4);

        if (nota == null)
        {
            nota = new Informacion
            {
                IdUsuario = idUsuario,
                IdTipoinfo = 4,
                Descrip = contenido,
                FechaI = DateTime.Now
            };
            _context.Informacions.Add(nota);
        }
        else
        {
            nota.Descrip = contenido;
            _context.Informacions.Update(nota);
        }

        await _context.SaveChangesAsync();
    }

    public async Task BorrarNotaAsync(int idUsuario)
    {
        var nota = await _context.Informacions
            .FirstOrDefaultAsync(i => i.IdUsuario == idUsuario && i.IdTipoinfo == 4);

        if (nota != null)
        {
            _context.Informacions.Remove(nota);
            await _context.SaveChangesAsync();
        }
    }
}
