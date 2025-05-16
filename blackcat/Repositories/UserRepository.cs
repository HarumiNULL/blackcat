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
    
    public async Task<Usuario> RegistrarUsuarioAsync(Usuario usuario)
    {
        usuario.Cont = BCrypt.Net.BCrypt.HashPassword(usuario.Cont);
        usuario.IdEstado = 1; // Estado activo por defecto
        
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        
        return usuario;
    }


    public async Task<Usuario?> ObtenerUsuAsync(string nombre)
    {
        return await _context.Usuarios.Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.NombreU == nombre);
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
    
    public async Task<Usuario?> GetUserByIdAsync(int id)
    {
        try
        {
            return await _context.Usuarios
                .Include(u => u.IdEstadoNavigation)
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdU == id);
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
}