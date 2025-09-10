using Spectre.Console;
using lapem_sid_cli.repositories;
using lapem_sid_cli.models;

namespace lapem_sid_cli.features.configuracion.productos;

public class AddProducto
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\01configuracion\productos\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de productos.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de productos disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de producto a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar este producto?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando producto al servidor...[/]");

            var repo = new ProductosRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar el producto: {errorMessage}[/]");
                return;
            }

            var producto = result.Value;
            var table = new Table()
                .Title("[green]Producto registrado exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", producto.Id ?? "-")
                 .AddRow("Código Fabricante", producto.CodigoFabricante ?? "-")
                 .AddRow("Descripción", producto.Descripcion ?? "-")
                 .AddRow("Descripción Corta", producto.DescripcionCorta ?? "-")
                 .AddRow("Tipo Fabricación", producto.TipoFabricacion ?? "-")
                 .AddRow("Unidad", producto.Unidad ?? "-")
                 .AddRow("Estatus", producto.Estatus ?? "-")
                 .AddRow("Fecha Registro", producto.FechaRegistro?.ToString() ?? "-");

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar el archivo: {ex.Message}[/]");
        }
    }
}
