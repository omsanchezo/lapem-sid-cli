namespace lapem_sid_cli.models;

public class MuestraExpediente
{
    public string? Identificador { get; set; }
    public string? Estatus { get; set; }
    public List<ResultadoPrueba>? ResultadosPruebas { get; set; }
}
