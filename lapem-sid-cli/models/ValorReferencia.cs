namespace lapem_sid_cli.models;

public class ValorReferencia
{
    public string? Id { get; set; }
    public string? IdProducto { get; set; }
    public string? IdPrueba { get; set; }
    public double? Valor { get; set; }
    public double? Valor2 { get; set; }
    public string? Unidad { get; set; }
    public string? Comparacion { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public Producto? Producto { get; set; }
    public Prueba? Prueba { get; set; }
}
