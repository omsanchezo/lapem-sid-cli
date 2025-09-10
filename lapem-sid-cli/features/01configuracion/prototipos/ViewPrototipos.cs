using lapem_sid_cli.models;
using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.configuracion.prototipos;

public class ViewPrototipos
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var repo = new PrototiposRepository(auth);
        var result = repo.GetAll();

        if (result.IsFailed)
        {
            AnsiConsole.MarkupLine($"[red]Error: {result.Errors[0].Message}[/]");
            return;
        }

        var prototipos = result.Value;
        if (prototipos.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay prototipos registrados.[/]");
            return;
        }

        var table = new Table().Title("[bold]Prototipos[/]")
            .AddColumn("ID")
            .AddColumn("Número")
            .AddColumn("Estatus")
            .AddColumn("Fecha Emisión")
            .AddColumn("Fecha Vencimiento")
            .AddColumn("Fecha Registro");

        foreach (var prot in prototipos)
        {
            table.AddRow(
                prot.Id ?? "",
                prot.Numero ?? "",
                prot.Estatus ?? "",
                prot.FechaEmision?.ToString("yyyy-MM-dd") ?? "",
                prot.FechaVencimiento?.ToString("yyyy-MM-dd") ?? "",
                prot.FechaRegistro?.ToString("yyyy-MM-dd") ?? ""
            );
        }

        AnsiConsole.Write(table);
    }
}
