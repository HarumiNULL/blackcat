using System;
using System.Collections.Generic;

namespace blackcat.Models;

public partial class Tipoinfo
{
    public int IdTipo { get; set; }

    public bool? Tipo { get; set; }

    public virtual ICollection<Informacion> Informacions { get; set; } = new List<Informacion>();
}
