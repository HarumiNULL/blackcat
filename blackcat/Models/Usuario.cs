using System;
using System.Collections.Generic;

namespace blackcat.Models;

public partial class Usuario
{
    public int IdU { get; set; }

    public int? IdRol { get; set; }

    public int? IdEstado { get; set; }

    public string? NombreU { get; set; }

    public string? CorreoU { get; set; }

    public string? Cont { get; set; }

    public string? ContrasenaToken { get; set; }

    public virtual EstadoUsulibro? IdEstadoNavigation { get; set; }

    public virtual Rol? IdRolNavigation { get; set; }
    
    public virtual ICollection<Informacion> Informacions { get; set; } = new List<Informacion>();

    public virtual ICollection<ListaU> ListaUs { get; set; } = new List<ListaU>();
}
