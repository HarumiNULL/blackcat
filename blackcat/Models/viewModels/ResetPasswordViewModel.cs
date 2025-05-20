using System.ComponentModel.DataAnnotations;

namespace blackcat.Models.viewModels;

public class ResetPasswordViewModel
{

    [Required, Display(Name = "Contraseña nueva"), DataType(DataType.Password)]
    public string? Clave { get; set; }
    [Required, Display(Name="Confirmar contraseña"), DataType(DataType.Password),Compare(nameof(Clave))]
    public string? ConfirmarClave { get; set; }
    public string? Token { get; set; }
}