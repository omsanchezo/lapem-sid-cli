using lapem_sid_cli.models;
using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.configuracion.normas;

public class ViewNormas
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var repo = new NormasRepository(auth);
        var result = repo.GetAll();

        if (result.IsFailed)
        {
            AnsiConsole.MarkupLine($"[red]Error: {result.Errors[0].Message}[/]");
            return;
        }

        var normas = result.Value;
        if (normas.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay normas registradas.[/]");
            return;
        }

        var table = new Table().Title("[bold]Normas[/]")
            .AddColumn("ID")
            .AddColumn("Clave")
            .AddColumn("Nombre")
            .AddColumn("Edición")
            .AddColumn("Estatus")
            .AddColumn("Es CFE")
            .AddColumn("Fecha Registro");

        foreach (var norma in normas)
        {
            table.AddRow(
                norma.Id ?? "",
                norma.Clave ?? "",
                norma.Nombre ?? "",
                norma.Edicion ?? "",
                norma.Estatus ?? "",
                norma.EsCFE ? "Sí" : "No",
                norma.FechaRegistro?.ToString("yyyy-MM-dd") ?? ""
            );
        }

        AnsiConsole.Write(table);
    }
}
