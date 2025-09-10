namespace lapem_sid_cli.models;

public class DetalleContrato
{
    public string? PartidaContrato { get; set; }
    public string? DescripcionAviso { get; set; }
    public string? AreaDestinoCFE { get; set; }  // Solo para ContratoCFE
    public decimal Cantidad { get; set; }
    public string? Unidad { get; set; }
    public decimal ImporteTotal { get; set; }
}
