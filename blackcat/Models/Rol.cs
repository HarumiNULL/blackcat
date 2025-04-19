using System;
using System.Collections.Generic;

namespace blackcat.Models;

public partial class Rol
{
    public int Idrol { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
