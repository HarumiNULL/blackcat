using System;
using System.Collections.Generic;

namespace blackcat.Models;

public partial class ListaU
{
    public int IdLista { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdLibro { get; set; }

    public virtual Libro? IdLibroNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
