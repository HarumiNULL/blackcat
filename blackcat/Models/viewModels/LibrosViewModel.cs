namespace blackcat.Models.viewModels;

public class LibrosViewModel
{
    public int IdL { get; set; }

    public string? NombreL { get; set; }

    public string? Autor { get; set; }

    public string? Archivo { get; set; }

    public string? Descripcion { get; set; }

    public string? Imagen { get; set; }

    public virtual ICollection<Busquedum> Busqueda { get; set; } = new List<Busquedum>();

    public virtual ICollection<ListaU> ListaUs { get; set; } = new List<ListaU>();
    
    public byte[] ? Foto { get; set; }
}