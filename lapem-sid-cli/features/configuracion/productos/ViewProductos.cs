using lapem_sid_cli.models;
using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.configuracion.productos;

public class ViewProductos
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var repo = new ProductosRepository(auth);
        var result = repo.GetAll();

        if (result.IsFailed)
        {
            AnsiConsole.MarkupLine($"[red]Error: {result.Errors[0].Message}[/]");
            return;
        }

        var productos = result.Value;
        if (productos.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay productos registrados.[/]");
            return;
        }

        var table = new Table().Title("[bold]Productos[/]")
            .AddColumn("ID")
            .AddColumn("Código Fabricante")
            .AddColumn("Descripción")
            .AddColumn("Descripción Corta")
            .AddColumn("Tipo Fabricación")
            .AddColumn("Unidad")
            .AddColumn("Norma - Clave")
            .AddColumn("Norma - Nombre")
            .AddColumn("Prototipo - Número")
            .AddColumn("Estatus")
            .AddColumn("Fecha Registro");

        foreach (var prod in productos)
        {
            table.AddRow(
                prod.Id ?? "",
                prod.CodigoFabricante ?? "",
                prod.Descripcion ?? "",
                prod.DescripcionCorta ?? "",
                prod.TipoFabricacion ?? "",
                prod.Unidad ?? "",
                prod.Norma?.Clave ?? "",
                prod.Norma?.Nombre ?? "",
                prod.Prototipo?.Numero ?? "",
                prod.Estatus ?? "",
                prod.FechaRegistro?.ToString("yyyy-MM-dd") ?? ""
            );
        }

        AnsiConsole.Write(table);
    }
}
