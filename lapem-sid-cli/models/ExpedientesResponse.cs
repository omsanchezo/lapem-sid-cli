namespace lapem_sid_cli.models;

public class ExpedientesResponse
{
    public int Total { get; set; }
    public int PaginaActual { get; set; }
    public int TamañoPagina { get; set; }
    public List<ExpedientePrueba>? Expedientes { get; set; }
}
