namespace lapem_sid_cli.models;

public class Contrato
{
    public string? Id { get; set; }
    public string? TipoContrato { get; set; }
    public string? NoContrato { get; set; }
    public string? Estatus { get; set; }
    public List<DetalleContrato>? DetalleContrato { get; set; }

    // Campos específicos para ContratoCFE
    public string? UrlArchivo { get; set; }
    public string? MD5 { get; set; }
    public DateTime? FechaEntregaCFE { get; set; }
}
