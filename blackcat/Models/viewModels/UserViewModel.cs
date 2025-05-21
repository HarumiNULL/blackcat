using System.ComponentModel.DataAnnotations;

namespace blackcat.Models.viewModels;

public class UserViewModel
{
    public int IdU { get; set; }

    public int? IdRol { get; set; }

    public int? IdEstado { get; set; }

    [Required, Display(Name = "Nombre Usuario")]
    public string? NombreU { get; set; }

    [Required, DataType(DataType.EmailAddress), Display(Name = "Email")]
    public string? CorreoU { get; set; }
    public string? Estado { get; set; }
    public string? Rol { get; set; }
    [Display(Name = "Contraseña"), DataType(DataType.Password)]
    public string? Cont { get; set; }
}