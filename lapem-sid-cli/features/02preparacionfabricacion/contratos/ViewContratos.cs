using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.preparacionfabricacion.contratos;

public class ViewContratos
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var repo = new ContratosRepository(auth);
        var result = repo.GetAll();

        if (result.IsFailed)
        {
            var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
            Console.WriteLine($"Error: {errorMessage}");
            return;
        }

        var contratos = result.Value;
        if (contratos.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay contratos registrados.[/]");
            return;
        }

        var table = new Table().Title("[bold]Contratos[/]")
            .AddColumn("ID")
            .AddColumn("Tipo")
            .AddColumn("No. Contrato")
            .AddColumn("Estatus")
            .AddColumn("URL")
            .AddColumn("Fecha Entrega");

        foreach (var contrato in contratos)
        {
            table.AddRow(
                contrato.Id ?? "",
                contrato.TipoContrato ?? "",
                contrato.NoContrato ?? "",
                contrato.Estatus ?? "",
                contrato.UrlArchivo ?? "-",
                contrato.FechaEntregaCFE?.ToString("yyyy-MM-dd") ?? "-"
            );
        }
        AnsiConsole.Write(table);
    }
}
