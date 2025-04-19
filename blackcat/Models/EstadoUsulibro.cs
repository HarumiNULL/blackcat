using System;
using System.Collections.Generic;

namespace blackcat.Models;

public partial class EstadoUsulibro
{
    public int IdE { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<Busquedum> Busqueda { get; set; } = new List<Busquedum>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
