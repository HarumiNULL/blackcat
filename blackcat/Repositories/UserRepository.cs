using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace blackcat.Repositories;

public class UserRepository
{
    private readonly BlackcatDbContext _context;

    public UserRepository(BlackcatDbContext  dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<UserDto>> ListUserAsync()
    {
        
        try
        {
            List<Usuario> users = await _context.Usuarios
                .Include(u => u.IdEstadoNavigation)
                .Include(u => u.IdRolNavigation)
                .ToListAsync();
            List<UserDto> usersDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var  userDto= new UserDto()
                {
                    IdU = user.IdU,
                    IdRol = user.IdRol,
                    IdEstado = user.IdEstado,
                    NombreU = user.NombreU,
                    CorreoU = user.CorreoU, 
                    Estado = user.IdEstadoNavigation?.Estado,
                    Rol = user.IdRolNavigation?.Nombre
                    
                };
                usersDtos.Add(userDto);
            }

            return usersDtos;
        }
        catch (SystemException)
        {
            return null!;
        }
    }
}