
using blackcat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace blackcat.Services;

public interface IAuthService
{
    Task<string> RegistrarUsuarioAsync(Usuario request);
}
public class RegistroU : IAuthService
{
    private readonly BlackcatDbContext _context;
    private readonly IPasswordHasher<Usuario> _hasher;

    // Constructor con inyección de dependencias
    public RegistroU(BlackcatDbContext context, IPasswordHasher<Usuario> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    // Implementación de RegistrarUsuarioAsync
    public async Task<string> RegistrarUsuarioAsync(Usuario request)
    {
        var existe = await _context.Usuarios.AnyAsync(u => u.CorreoU == request.CorreoU);
        if (existe)
            return "Este correo ya está registrado.";

        var nuevoUsuario = new Usuario
        {
            NombreU = request.NombreU,
            CorreoU = request.CorreoU,
            IdRol = 3, 
            IdEstado = 2
        };

        // Hashear la contraseña
        nuevoUsuario.Cont = _hasher.HashPassword(nuevoUsuario, request.Cont);

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        return "Usuario registrado correctamente.";
    }
}
