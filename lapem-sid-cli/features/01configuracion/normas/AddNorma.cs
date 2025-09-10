using Spectre.Console;
using lapem_sid_cli.repositories;

namespace lapem_sid_cli.features.configuracion.normas;

public class AddNorma
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\01configuracion\normas\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de normas.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de normas disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de norma a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar esta norma?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando norma al servidor...[/]");

            var repo = new NormasRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar la norma: {errorMessage}[/]");
                return;
            }

            var norma = result.Value;
            var table = new Table()
                .Title("[green]Norma registrada exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", norma.Id ?? "-")
                 .AddRow("Clave", norma.Clave ?? "-")
                 .AddRow("Nombre", norma.Nombre ?? "-")
                 .AddRow("Edición", norma.Edicion ?? "-")
                 .AddRow("Estatus", norma.Estatus ?? "-")
                 .AddRow("Es CFE", norma.EsCFE ? "Sí" : "No")
                 .AddRow("Fecha Registro", norma.FechaRegistro?.ToString() ?? "-");

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar el archivo: {ex.Message}[/]");
        }
    }
}
