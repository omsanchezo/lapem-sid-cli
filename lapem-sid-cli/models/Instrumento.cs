namespace lapem_sid_cli.models;

public class Instrumento
{
    public string? Id { get; set; }
    public string? Nombre { get; set; }
    public string? NumeroSerie { get; set; }
    public DateTime? FechaCalibracion { get; set; }
    public DateTime? FechaVencimientoCalibracion { get; set; }
    public string? UrlArchivo { get; set; }
    public string? MD5 { get; set; }
    public string? Estatus { get; set; }
    public DateTime? FechaRegistro { get; set; }
}
