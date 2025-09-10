namespace lapem_sid_cli.models;

public class ExpedientePrueba
{
    public string? Id { get; set; }
    public string? ClaveExpediente { get; set; }
    public List<MuestraExpediente>? MuestrasExpediente { get; set; }
    public int TamanioMuestra { get; set; }
    public int MaximoRechazos { get; set; }
    public string? TipoMuestreo { get; set; }
    public object? ResultadosPruebas { get; set; }  // Mantenemos como object ya que no se usa en el ejemplo
    public OrdenFabricacion? OrdenFabricacion { get; set; }
    public List<string>? AvisosPrueba { get; set; }  // Actualizado según el ejemplo donde son strings
    public string? EstatusPruebas { get; set; }
    public string? ResultadoExpediente { get; set; }
    public DateTime InicioPruebas { get; set; }
    public DateTime FinPruebas { get; set; }
    public DateTime FechaRegistro { get; set; }
}
