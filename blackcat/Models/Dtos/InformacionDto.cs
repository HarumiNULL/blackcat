﻿namespace blackcat.Models.Dtos;

public class InformacionDto
{
    public int IdInfo { get; set; }
    
    public int? IdUsuario { get; set; }
    
    public int IdTipoinfo { get; set; }
    
    public string? Descrip { get; set; }
    
    public virtual Tipoinfo? IdTipoinfoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
    