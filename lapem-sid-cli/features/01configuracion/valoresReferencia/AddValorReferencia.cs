using Spectre.Console;
using lapem_sid_cli.repositories;
using lapem_sid_cli.models;

namespace lapem_sid_cli.features.configuracion.valoresReferencia;

public class AddValorReferencia
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\01configuracion\valoresReferencia\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de valores de referencia.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de valores de referencia disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de valor de referencia a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar este valor de referencia?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando valor de referencia al servidor...[/]");

            var repo = new ValoresReferenciaRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar el valor de referencia: {errorMessage}[/]");
                return;
            }

            var valorReferencia = result.Value;
            var table = new Table()
                .Title("[green]Valor de referencia registrado exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", valorReferencia.Id ?? "-")
                 .AddRow("ID Producto", valorReferencia.IdProducto ?? "-")
                 .AddRow("ID Prueba", valorReferencia.IdPrueba ?? "-")
                 .AddRow("Valor", valorReferencia.Valor?.ToString() ?? "-")
                 .AddRow("Valor 2", valorReferencia.Valor2?.ToString() ?? "-")
                 .AddRow("Unidad", valorReferencia.Unidad ?? "-")
                 .AddRow("Comparación", valorReferencia.Comparacion ?? "-")
                 .AddRow("Fecha Registro", valorReferencia.FechaRegistro?.ToString() ?? "-");

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar el archivo: {ex.Message}[/]");
        }
    }
}
