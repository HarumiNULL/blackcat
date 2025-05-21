namespace blackcat.Models.Dtos;

public class BusquedaDto
{
    public int IdBus { get; set; }

    public int? IdEstadoBus { get; set; }

    public int? IdLibro { get; set; }

    public string? NomLib { get; set; }

    public int? CantB { get; set; }

    public int? CantBn { get; set; }
    public string? Estado { get; set; }
}