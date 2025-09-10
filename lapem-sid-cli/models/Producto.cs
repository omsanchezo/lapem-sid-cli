namespace lapem_sid_cli.models;

public class Producto
{
    public string? Id { get; set; }
    public string? CodigoFabricante { get; set; }
    public string? Descripcion { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? TipoFabricacion { get; set; }
    public string? Unidad { get; set; }
    public Norma? Norma { get; set; }
    public Prototipo? Prototipo { get; set; }
    public string? Estatus { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public List<Prueba>? Pruebas { get; set; }
}
