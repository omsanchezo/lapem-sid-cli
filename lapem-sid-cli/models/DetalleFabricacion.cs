namespace lapem_sid_cli.models;

public class DetalleFabricacion
{
    public string? ContratoId { get; set; }
    public string? TipoContrato { get; set; }
    public string? PartidaContratoId { get; set; }
    public string? DescripcionPartida { get; set; }
    public string? Unidad { get; set; }
    public decimal CantidadOriginalContrato { get; set; }
    public decimal CantidadAFabricar { get; set; }
}
