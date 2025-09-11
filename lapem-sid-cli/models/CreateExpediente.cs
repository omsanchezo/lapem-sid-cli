namespace lapem_sid_cli.models;

public class CreateExpediente
{
    public string? ClaveExpediente { get; set; }
    public string? OrdenFabricacion { get; set; }
    public int CantidadMuestras { get; set; }
    public int MaximoRechazos { get; set; }
    public List<string>? Muestras { get; set; }
}