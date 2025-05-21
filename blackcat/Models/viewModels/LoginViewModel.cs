using System.ComponentModel.DataAnnotations;

namespace blackcat.Models.viewModels;

public class LoginViewModel
{
    [Required, Display(Name = "Nombre Usuario")]
    public string? NombreU { get; set; }
    [Required, Display(Name = "Contraseña"), DataType(DataType.Password)]
    public string? Cont { get; set; }
}