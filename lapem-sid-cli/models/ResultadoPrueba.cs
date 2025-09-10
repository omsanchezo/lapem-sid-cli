namespace lapem_sid_cli.models;

public class ResultadoPrueba
{
    public Prueba? Prueba { get; set; }
    public ValorReferencia? ValorReferencia { get; set; }
    public DateTime FechaPrueba { get; set; }
    public string? OperadorPrueba { get; set; }
    public Instrumento? InstrumentoMedicion { get; set; }
    public decimal ValorMedido { get; set; }
    public string? Resultado { get; set; }
    public int NumeroIntento { get; set; }
}
