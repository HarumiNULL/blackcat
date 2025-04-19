using System;
using System.Collections.Generic;

namespace blackcat.Models;

public partial class Informacion
{
    public int IdInfo { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdTipoinfo { get; set; }

    public string? Descrip { get; set; }

    public DateTime? FechaI { get; set; }

    public virtual Tipoinfo? IdTipoinfoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
