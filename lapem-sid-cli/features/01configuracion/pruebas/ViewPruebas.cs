using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;
using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.configuracion.pruebas;

public class ViewPruebas
{
    public static void Show(AuthResult auth)
    {
        var repo = new PruebasRepository(auth);
        var result = repo.GetAll();

        if (result.IsFailed)
        {
            AnsiConsole.MarkupLine($"[red]Error: {result.Errors[0].Message}[/]");
            return;
        }

        var pruebas = result.Value;
        if (pruebas.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay pruebas registradas.[/]");
            return;
        }

        var table = new Table().Title("[bold]Pruebas[/]")
            .AddColumn("ID")
            .AddColumn("Nombre")
            .AddColumn("Estatus")
            .AddColumn("Tipo Prueba")
            .AddColumn("Tipo Resultado")
            .AddColumn("Fecha Registro");

        foreach (var prueba in pruebas)
        {
            table.AddRow(
                prueba.Id ?? "",
                prueba.Nombre ?? "",
                prueba.Estatus ?? "",
                prueba.TipoPrueba ?? "",
                prueba.TipoResultado ?? "",
                prueba.FechaRegistro?.ToString("yyyy-MM-dd") ?? ""
            );
        }

        AnsiConsole.Write(table);
    }
}
