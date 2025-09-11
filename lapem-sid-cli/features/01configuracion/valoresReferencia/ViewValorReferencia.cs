using Spectre.Console;
using lapem_sid_cli.repositories;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.features.configuracion.valoresReferencia;

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
        table.AddColumn("Producto");
        table.AddColumn("IdPrueba");
        table.AddColumn("Prueba");
        table.AddColumn("Valor");
        table.AddColumn("Valor2");
        table.AddColumn("Unidad");
        table.AddColumn("Comparacion");
        table.AddColumn("FechaRegistro");
        foreach (ValorReferencia v in valores)
        {
            var producto = v.Producto != null ? $"{v.Producto.CodigoFabricante} - {v.Producto.Descripcion}" : "N/A";
            var prueba = v.Prueba != null ? $"{v.Prueba.Nombre}" : "N/A";
            table.AddRow(
                v.Id ?? "",
                v.IdProducto ?? "",
                producto,
                v.IdPrueba ?? "",
                prueba,
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
