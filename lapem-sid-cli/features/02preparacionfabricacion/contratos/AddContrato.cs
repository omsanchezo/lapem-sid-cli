
using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.preparacionfabricacion.contratos;

public class AddContrato
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\02preparacionfabricacion\contratos\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de contratos.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de contratos disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de contrato a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar este contrato?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando contrato al servidor...[/]");

            var repo = new ContratosRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar el contrato: {errorMessage}[/]");
                return;
            }

            AnsiConsole.MarkupLine("[green]Contrato registrado exitosamente.[/]");
            var contrato = result.Value;
            var table = new Table()
                .Title("[green]Contrato registrado exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", contrato.Id ?? "-")
                 .AddRow("Tipo de Contrato", contrato.TipoContrato ?? "-")
                 .AddRow("No. Contrato", contrato.NoContrato ?? "-")
                 .AddRow("Estatus", contrato.Estatus ?? "-")
                 .AddRow("URL del Archivo", contrato.UrlArchivo ?? "-")
                 .AddRow("MD5", contrato.MD5 ?? "-")
                 .AddRow("Fecha Entrega CFE", contrato.FechaEntregaCFE?.ToString("yyyy-MM-dd") ?? "-");

            // Si hay detalles del contrato, mostrarlos
            if (contrato.DetalleContrato?.Any() == true)
            {
                AnsiConsole.WriteLine(); // Línea en blanco para separación
                var detallesTable = new Table()
                    .Title("[blue]Detalles del Contrato[/]")
                    .AddColumn("Partida")
                    .AddColumn("DescripcionAviso")
                    .AddColumn("AreaDestinoCFE")
                    .AddColumn("Cantidad")
                    .AddColumn("Unidad")
                    .AddColumn("ImporteTotal")
                    ;

                foreach (var detalle in contrato.DetalleContrato)
                {
                    detallesTable.AddRow(
                        detalle.PartidaContrato ?? "-",
                        detalle.DescripcionAviso ?? "-",
                        detalle.AreaDestinoCFE ?? "-",
                        detalle.Cantidad.ToString() ?? "-",
                        detalle.Unidad ?? "-",
                        detalle.ImporteTotal.ToString("C") ?? "-"
                    );
                }

                AnsiConsole.Write(detallesTable);
            }

            AnsiConsole.MarkupLine("[green]Contrato registrado exitosamente.[/]");
            AnsiConsole.Write(table);
        }

        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al leer el archivo: {ex.Message}[/]");
        }
    }
}