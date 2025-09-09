namespace lapem_sid_cli.models;

public class Prototipo
{
    public string? Id { get; set; }
    public string? Numero { get; set; }
    public DateTime? FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string? UrlArchivo { get; set; }
    public string? MD5 { get; set; }
    public string? Estatus { get; set; }
    public DateTime? FechaRegistro { get; set; }
}
