using Spectre.Console;
using lapem_sid_cli.repositories;

namespace lapem_sid_cli.features.configuracion.prototipos;

public class AddPrototipo
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\01configuracion\prototipos\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de prototipos.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de prototipos disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de prototipo a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar este prototipo?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando prototipo al servidor...[/]");

            var repo = new PrototiposRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar el prototipo: {errorMessage}[/]");
                return;
            }

            var prototipo = result.Value;
            var table = new Table()
                .Title("[green]Prototipo registrado exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", prototipo.Id ?? "-")
                 .AddRow("Número", prototipo.Numero ?? "-")
                 .AddRow("Fecha Emisión", prototipo.FechaEmision?.ToString() ?? "-")
                 .AddRow("Fecha Vencimiento", prototipo.FechaVencimiento?.ToString() ?? "-")
                 .AddRow("URL Archivo", prototipo.UrlArchivo ?? "-")
                 .AddRow("MD5", prototipo.MD5 ?? "-")
                 .AddRow("Estatus", prototipo.Estatus ?? "-")
                 .AddRow("Fecha Registro", prototipo.FechaRegistro?.ToString() ?? "-");

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar el archivo: {ex.Message}[/]");
        }
    }
}
