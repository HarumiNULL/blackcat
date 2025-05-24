using blackcat.Models;
using blackcat.Models.Dtos;
using blackcat.Models.viewModels;
using blackcat.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace blackcat.Repositories;

public class UserRepository
{
    private readonly BlackcatDbContext _context;

    public UserRepository(BlackcatDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.CorreoU == correo);
    }

    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.NombreU == nombreUsuario);
    }

    public async Task<Usuario?> RegistrarUsuarioAsync(UserDto usuario)
    {
        var user = new Usuario()
        {
            NombreU = usuario.NombreU,
            CorreoU = usuario.CorreoU,
            IdRol = usuario.IdRol,
            IdEstado = usuario.IdEstado,
            Cont = usuario.Cont
        };

        _context.Usuarios.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }


    public async Task<Usuario?> ObtenerUsuAsync(string nombre)
    {
        try
        {
            return await _context.Usuarios.Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.NombreU == nombre);
        }
        catch (Exception ex)
        {
            // Loggear el error (usando ILogger, Serilog, etc.)
            Console.WriteLine($"Error al obtener usuario: {ex.Message}");
            return null; // O lanzar una excepción personalizada
        }
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
                var userDto = new UserDto()
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
    
    

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _context.Usuarios
                .Include(u => u.IdEstadoNavigation)
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdU == id);
            return new UserDto()
            {
                CorreoU = user.CorreoU,
                NombreU = user.NombreU,
                IdEstado = user.IdEstado,
                IdRol = user.IdRol,
                IdU = user.IdU,
                Estado = user.IdEstadoNavigation.Estado,
                Rol = user.IdRolNavigation.Nombre
            };
        }
        catch (SystemException)
        {
            return null;
        }
    }

    public async Task<bool> ToggleUserStatusAsync(int userId, int nuevoEstadoId)
    {
        try
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) return false;

            user.IdEstado = nuevoEstadoId;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (SystemException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) return false;

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
            return false;
        }
    }

    public async Task<OlvideClaveDto> CreateRecoveryToken(string email)
    {
        try
        {
            var user = await _context.Usuarios
                .Where(u => u.CorreoU == email)
                .FirstOrDefaultAsync();
            string recoveryToken = Guid.NewGuid().ToString();
            user!.ContrasenaToken = recoveryToken;
            await _context.SaveChangesAsync();
            return new OlvideClaveDto()
            {
                Email = user.CorreoU,
                Name = user.NombreU,
                Token = recoveryToken
            };
        }
        catch (SystemException)
        {
            return null!;
        }
    }

    public async Task<bool> ExisteTokenAsync(string token)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.ContrasenaToken == token);
    }

    public async Task<bool> ResetPasswordAsync(string token, string claveHash)
    {
        try
        {
            var user = await _context.Usuarios
                .Where(u => u.ContrasenaToken == token)
                .FirstOrDefaultAsync();
            user!.Cont = claveHash;
            user.ContrasenaToken = "";
            await _context.SaveChangesAsync();
            return true;
        }
        catch (SystemException)
        {
            return false;
        }
    }
    
    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<bool> UpdateUser(UserDto user)
    {
        var u = await _context.Usuarios.FindAsync(user.IdU);
        u.NombreU = user.NombreU;
        u.CorreoU = user.CorreoU;
        u.IdRol = user.IdRol;
        u.IdEstado = user.IdEstado;
        _context.Usuarios.Update(u);
        return await _context.SaveChangesAsync() > 0;
    }
    
    
}