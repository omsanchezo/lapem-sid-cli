using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.preparacionfabricacion.ordenesfabricacion;

public class ViewOrdenFabricacionById
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var id = AnsiConsole.Ask<string>("[green]Ingrese el ID de la orden de fabricación:[/]");

        var repo = new OrdenFabricacionRepository(auth);
        var result = repo.GetById(id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
            Console.WriteLine($"Error: {errorMessage}");
            return;
        }

        var orden = result.Value;

        // Tabla principal con los datos de la orden
        var table = new Table().Title($"[bold]Orden de Fabricación: {orden.ClaveOrdenFabricacion}[/]")
            .AddColumn("ID")
            .AddColumn("Clave")
            .AddColumn("Lote")
            .AddColumn("ID Producto");

        table.AddRow(
            orden.Id ?? "",
            orden.ClaveOrdenFabricacion ?? "",
            orden.LoteFabricacion ?? "",
            orden.IdProducto ?? ""
        );

        AnsiConsole.Write(table);

        // Si hay detalles de fabricación, mostrar una segunda tabla
        if (orden.DetalleFabricacion?.Count > 0)
        {
            var detalleTable = new Table().Title("[bold]Detalles de Fabricación[/]")
                .AddColumn("Contrato")
                .AddColumn("Tipo")
                .AddColumn("Partida")
                .AddColumn("Descripción")
                .AddColumn("Unidad")
                .AddColumn("Cant. Original")
                .AddColumn("Cant. a Fabricar");

            foreach (var detalle in orden.DetalleFabricacion)
            {
                detalleTable.AddRow(
                    detalle.ContratoId ?? "",
                    detalle.TipoContrato ?? "",
                    detalle.PartidaContratoId ?? "",
                    detalle.DescripcionPartida ?? "",
                    detalle.Unidad ?? "",
                    detalle.CantidadOriginalContrato.ToString("N2"),
                    detalle.CantidadAFabricar.ToString("N2")
                );
            }

            AnsiConsole.WriteLine(); // Línea en blanco para separar las tablas
            AnsiConsole.Write(detalleTable);
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Esta orden no tiene detalles de fabricación.[/]");
        }
    }
}
