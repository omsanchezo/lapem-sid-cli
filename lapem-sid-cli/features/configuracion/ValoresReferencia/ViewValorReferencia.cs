using Spectre.Console;
using lapem_sid_cli.repositories;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.features.configuracion.ValoresReferencia;

public class ViewValorReferencia(AuthResult auth)
{

    private AuthResult _auth = auth;

    public static void Show(AuthResult auth)
    {
        var repo = new ValoresRefenciaRepository(auth);
        var result = repo.GetAll();
        if (result.IsFailed)
        {
            AnsiConsole.MarkupLine($"[red]Error: {result.Errors[0].Message}[/]");
            return;
        }
        var valores = result.Value;

        if (valores.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay valores de referencia registrados.[/]");
            return;
        }

        var table = new Table().Title("Valores Referencia");
        table.AddColumn("ID");
        table.AddColumn("IdProducto");
        table.AddColumn("IdPrueba");
        table.AddColumn("Valor");
        table.AddColumn("Valor2");
        table.AddColumn("Unidad");
        table.AddColumn("Comparacion");
        table.AddColumn("FechaRegistro");
        foreach (ValorReferencia v in valores)
        {
            table.AddRow(
                v.Id ?? "",
                v.IdProducto ?? "",
                v.IdPrueba ?? "",
                v.Valor?.ToString() ?? "",
                v.Valor2?.ToString() ?? "",
                v.Unidad ?? "",
                v.Comparacion ?? "",
                v.FechaRegistro?.ToString("yyyy-MM-dd") ?? ""
            );
        }
        AnsiConsole.Write(table);
    }
}
