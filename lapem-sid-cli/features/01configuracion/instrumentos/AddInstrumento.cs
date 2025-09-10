using Spectre.Console;
using lapem_sid_cli.repositories;

namespace lapem_sid_cli.features.configuracion.instrumentos;

public class AddInstrumento
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\01configuracion\instrumentos\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de instrumentos.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de instrumentos disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de instrumento a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar este instrumento?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando instrumento al servidor...[/]");

            var repo = new InstrumentosRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar el instrumento: {errorMessage}[/]");
                return;
            }

            var instrumento = result.Value;
            var table = new Table()
                .Title("[green]Instrumento registrado exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", instrumento.Id ?? "-")
                 .AddRow("Nombre", instrumento.Nombre ?? "-")
                 .AddRow("No. Serie", instrumento.NumeroSerie ?? "-")
                 .AddRow("Fecha Calibración", instrumento.FechaCalibracion?.ToString() ?? "-")
                 .AddRow("Fecha Vencimiento", instrumento.FechaVencimientoCalibracion?.ToString() ?? "-")
                 .AddRow("Estatus", instrumento.Estatus ?? "-")
                 .AddRow("Fecha Registro", instrumento.FechaRegistro?.ToString() ?? "-");

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar el archivo: {ex.Message}[/]");
        }
    }
}
