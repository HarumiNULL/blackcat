namespace blackcat.Models.Dtos;

public class UserDto
{
    public int IdU { get; set; }

    public int? IdRol { get; set; }

    public int? IdEstado { get; set; }

    public string? NombreU { get; set; }

    public string? CorreoU { get; set; }
    public string? Estado { get; set; }
    public string? Rol { get; set; }
    
    public virtual ICollection<ListaU> ListaUsu { get; set; } = new List<ListaU>();
    
} 