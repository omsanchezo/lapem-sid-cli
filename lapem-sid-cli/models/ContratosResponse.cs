namespace lapem_sid_cli.models;

public class ContratosResponse
{
    public int Total { get; set; }
    public int PaginaActual { get; set; }
    public int TamañoPagina { get; set; }
    public List<Contrato>? Contratos { get; set; }
}
