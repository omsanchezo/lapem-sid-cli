namespace lapem_sid_cli.models;

public class CreateProduct
{
    public string? CodigoFabricante { get; set; }
    public string? Descripcion { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? TipoFabricacion { get; set; }
    public string? Unidad { get; set; }
    public string? Norma { get; set; }
    public string? Prototipo { get; set; }
    public string? Estatus { get; set; }
    public List<string>? Pruebas { get; set; }
}
