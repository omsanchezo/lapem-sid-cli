namespace lapem_sid_cli.models;

public class OrdenFabricacion
{
    public string? Id { get; set; }
    public string? ClaveOrdenFabricacion { get; set; }
    public string? LoteFabricacion { get; set; }
    public string? IdProducto { get; set; }
    public List<DetalleFabricacion>? DetalleFabricacion { get; set; }
}
