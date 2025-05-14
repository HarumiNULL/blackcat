using blackcat.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using blackcat.Models.viewModels;
using blackcat.Repositories;
using blackcat.Models.Dtos;

namespace blackcat.Services
{
    public class UserServices
    {
        private readonly BlackcatDbContext _context;
        private readonly UserRepository _userRepository;

        public UserServices(BlackcatDbContext context)
        {
            _context = context;
            _userRepository = new UserRepository(context);
        }

        public async Task<string> RegistrarUsuarioAsync(Usuario request, int rol = 3)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.CorreoU == request.CorreoU);
            if (existe)
                return "Este correo ya está registrado.";

            var nuevoUsuario = new Usuario
            {
                NombreU = request.NombreU,
                CorreoU = request.CorreoU,
                Cont = BCrypt.Net.BCrypt.HashPassword(request.Cont),
                IdRol = rol,
                IdEstado = 1

            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return "Usuario registrado correctamente.";
        }

        public async Task<Usuario?> IniciarSesionAsync(string nombre, string contra)
        {
            var usuario = await _context.Usuarios.Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.NombreU == nombre);
            if (usuario == null)
                return null;

            bool contrasenaValida = BCrypt.Net.BCrypt.Verify(contra, usuario.Cont);
            return contrasenaValida ? usuario : null;
        }

        public async Task<List<UserViewModel>> GetUsuarios()
        {
            try
            {
                List<UserDto> usersDtos = await _userRepository.ListUserAsync();
            
                List<UserViewModel> userViewModels = new List<UserViewModel>();
                foreach (var userDto in usersDtos)
                {
                    var userViewModel = new UserViewModel()
                    {
                        IdU = userDto.IdU,
                        NombreU =  userDto.NombreU,
                        CorreoU =  userDto.CorreoU,
                        IdEstado =  userDto.IdEstado,
                        IdRol =   userDto.IdRol,
                        Estado = userDto.Estado,
                        Rol =   userDto.Rol
                    };
                    userViewModels.Add(userViewModel);
                }

                return userViewModels;
            }
            catch (SystemException)
            {
                return null!;
            }
        }

}
}