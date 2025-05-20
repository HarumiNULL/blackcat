using System.Security.Claims;
using blackcat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using BCrypt.Net;
using blackcat.Models.viewModels;
using blackcat.Repositories;
using blackcat.Models.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        public async Task<UserDto?> IniciarSesionAsync(string nombre, string contra)
        {
            var usuario = await _userRepository.ObtenerUsuAsync(nombre);
            if (usuario == null)
                return null;

            bool contrasenaValida = BCrypt.Net.BCrypt.Verify(contra, usuario.Cont);
            UserDto user = new UserDto()
            {
                IdRol = usuario.IdRol,
                NombreU = usuario.NombreU,
                CorreoU = usuario.CorreoU,
                IdU = usuario.IdU,
                IdEstado = usuario.IdEstado,
                Rol = usuario.IdRolNavigation?.Nombre,
            };
            return contrasenaValida ? user : null;
        }

        public async Task<bool> CreateCredentials(UserDto user, bool remember, HttpContext hc)
        {
            try
            {
                List<Claim> c = new List<Claim>()
                {
                    new(ClaimTypes.NameIdentifier, user.IdU.ToString()),
                    new(ClaimTypes.Email, user.CorreoU!),
                    new(ClaimTypes.Name, user.NombreU!),
                    new(ClaimTypes.Role, user.Rol)
                };
                ClaimsIdentity ci = new(c, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties p = new AuthenticationProperties();
                p.AllowRefresh = true;
                p.IsPersistent = remember;
                if (remember)
                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1);
                else
                    p.ExpiresUtc = DateTimeOffset.MaxValue;
                await hc.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci), p);
                return true;
            }
            catch (SystemException)
            {
                return false;
            }
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
                        NombreU = userDto.NombreU,
                        CorreoU = userDto.CorreoU,
                        IdEstado = userDto.IdEstado,
                        IdRol = userDto.IdRol,
                        Estado = userDto.Estado,
                        Rol = userDto.Rol
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

        public async Task<bool> CambiarEstadoUsuarioAsync(int userId, int nuevoEstadoId)
        {
            try
            {
                return await _userRepository.ToggleUserStatusAsync(userId, nuevoEstadoId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EliminarUsuarioAsync(int userId)
        {
            try
            {
                return await _userRepository.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en servicio al eliminar: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> OlvideClave(string email)
        {
            try
            {
                OlvideClaveDto model = await _userRepository.CreateRecoveryToken(email);
                EmailService em = new EmailService();
                await em.EnviarCorreoRecuperacion(model.Email!, model.Name!, model.Token!);
                return true;
            }
            catch (SystemException)
            {
                return false;
            }
        }

        public async Task<bool> ExisteTokenRecuperacion(string token)
        {
            return await _userRepository.ExisteTokenAsync(token);
        }

        public async Task<bool> CambiarContrasena(ResetPasswordViewModel model)
        {
            
            try
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(model.Clave);
                var result = await _userRepository
                    .ResetPasswordAsync(model.Token!, hash);
                return result;
            }
            catch (SystemException)
            {
                return false;
            }
        }
    }
}