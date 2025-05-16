using System.ComponentModel.DataAnnotations;

namespace blackcat.Models.viewModels;

public class OlvideClaveViewModel
{
    [Required, DataType(DataType.EmailAddress)]
    public string? Correo { get; set; }
}