using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.preparacionfabricacion.expedientesprueba;

public class ViewExpedientesPrueba
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var repo = new ExpedientePruebaRepository(auth);
        var result = repo.GetAll();

        if (result.IsFailed)
        {
            var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
            Console.WriteLine($"Error: {errorMessage}");
            return;
        }

        var expedientes = result.Value;
        if (expedientes.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay expedientes de prueba registrados.[/]");
            return;
        }

        var table = new Table().Title("[bold]Expedientes de Prueba[/]")
            .AddColumn("ID")
            .AddColumn("Clave")
            .AddColumn("Tipo Muestreo")
            .AddColumn("Tamaño Muestra")
            .AddColumn("Max. Rechazos")
            .AddColumn("Estatus")
            .AddColumn("Resultado")
            .AddColumn("Inicio")
            .AddColumn("Fin");

        foreach (var expediente in expedientes)
        {
            table.AddRow(
                expediente.Id ?? "",
                expediente.ClaveExpediente ?? "",
                expediente.TipoMuestreo ?? "",
                expediente.TamanioMuestra.ToString(),
                expediente.MaximoRechazos.ToString(),
                expediente.EstatusPruebas ?? "",
                expediente.ResultadoExpediente ?? "",
                expediente.InicioPruebas.ToString("yyyy-MM-dd HH:mm"),
                expediente.FinPruebas.ToString("yyyy-MM-dd HH:mm")
            );
        }
        AnsiConsole.Write(table);
    }
}
