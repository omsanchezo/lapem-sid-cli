using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.preparacionfabricacion.ordenesfabricacion;

public class AddOrdenFabricacion
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\02preparacionfabricacion\ordenesfabricacion\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de órdenes de fabricación.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de órdenes de fabricación disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de orden de fabricación a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar esta orden de fabricación?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando orden de fabricación al servidor...[/]");

            var repo = new OrdenFabricacionRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar la orden de fabricación: {errorMessage}[/]");
                return;
            }

            var orden = result.Value;
            var table = new Table()
                .Title("[green]Orden de Fabricación registrada exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", orden.Id ?? "-")
                 .AddRow("Clave Orden Fabricación", orden.ClaveOrdenFabricacion ?? "-")
                 .AddRow("Lote Fabricación", orden.LoteFabricacion ?? "-")
                 .AddRow("ID Producto", orden.IdProducto ?? "-");

            AnsiConsole.Write(table);

            // Si hay detalles de fabricación, mostrarlos
            if (orden.DetalleFabricacion?.Any() == true)
            {
                AnsiConsole.WriteLine(); // Línea en blanco para separación
                var detallesTable = new Table()
                    .Title("[blue]Detalles de Fabricación[/]")
                    .AddColumn("Contrato")
                    .AddColumn("Tipo")
                    .AddColumn("Partida")
                    .AddColumn("Descripción")
                    .AddColumn("Unidad")
                    .AddColumn("Cant. Original")
                    .AddColumn("Cant. a Fabricar");

                foreach (var detalle in orden.DetalleFabricacion)
                {
                    detallesTable.AddRow(
                        detalle.ContratoId ?? "-",
                        detalle.TipoContrato ?? "-",
                        detalle.PartidaContratoId ?? "-",
                        detalle.DescripcionPartida ?? "-",
                        detalle.Unidad ?? "-",
                        detalle.CantidadOriginalContrato.ToString("N2"),
                        detalle.CantidadAFabricar.ToString("N2")
                    );
                }

                AnsiConsole.Write(detallesTable);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar el archivo: {ex.Message}[/]");
        }
    }
}