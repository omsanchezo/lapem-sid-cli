using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.configuracion.instrumentos;

public class ViewInstrumentos
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var repo = new InstrumentosRepository(auth);
        var result = repo.GetAll();

        if (result.IsFailed)
        {
            AnsiConsole.MarkupLine($"[red]Error: {result.Errors[0].Message}[/]");
            return;
        }

        var instrumentos = result.Value;
        if (instrumentos.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay instrumentos registrados.[/]");
            return;
        }

        var table = new Table().Title("[bold]Instrumentos[/]")
            .AddColumn("ID")
            .AddColumn("Nombre")
            .AddColumn("Serie")
            .AddColumn("Estatus")
            .AddColumn("Fecha Calibración")
            .AddColumn("Fecha Vencimiento")
            .AddColumn("Fecha Registro");

        foreach (var inst in instrumentos)
        {
            table.AddRow(
                inst.Id ?? "",
                inst.Nombre ?? "",
                inst.NumeroSerie ?? "",
                inst.Estatus ?? "",
                inst.FechaCalibracion?.ToString("yyyy-MM-dd") ?? "",
                inst.FechaVencimientoCalibracion?.ToString("yyyy-MM-dd") ?? "",
                inst.FechaRegistro?.ToString("yyyy-MM-dd") ?? ""
            );
        }

        AnsiConsole.Write(table);
    }
}
