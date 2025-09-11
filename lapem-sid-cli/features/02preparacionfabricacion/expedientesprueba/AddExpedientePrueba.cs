using Spectre.Console;
using lapem_sid_cli.repositories;

namespace lapem_sid_cli.features.preparacionfabricacion.expedientesprueba;

public class AddExpedientePrueba
{
    private const string DataPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\02preparacionfabricacion\expedientesprueba\data";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var basePath = DataPath;

        if (!Directory.Exists(basePath))
        {
            AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de datos de expedientes de prueba.[/]");
            return;
        }

        var files = Directory.GetFiles(basePath, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Select(name => name!)
                           .ToList();

        if (files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de expedientes de prueba disponibles.[/]");
            return;
        }

        var selectedFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Seleccione el archivo de expediente de prueba a cargar:[/]")
                .PageSize(10)
                .AddChoices(files.AsEnumerable()));

        var filePath = Path.Combine(basePath, selectedFile);
        AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar este expediente de prueba?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando expediente de prueba al servidor...[/]");

            var repo = new ExpedientePruebaRepository(auth);
            var result = repo.Create(jsonContent);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar el expediente de prueba: {errorMessage}[/]");
                return;
            }

            var expediente = result.Value;
            var table = new Table()
                .Title("[green]Expediente de Prueba registrado exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", expediente.Id ?? "-")
                 .AddRow("Clave Expediente", expediente.ClaveExpediente ?? "-")
                 .AddRow("Orden Fabricación", expediente.OrdenFabricacion?.ClaveOrdenFabricacion ?? "-")
                 .AddRow("Tamaño Muestra", expediente.TamanioMuestra.ToString())
                 .AddRow("Máximo Rechazos", expediente.MaximoRechazos.ToString())
                 .AddRow("Tipo Muestreo", expediente.TipoMuestreo ?? "-")
                 .AddRow("Estatus Pruebas", expediente.EstatusPruebas ?? "-")
                 .AddRow("Resultado Expediente", expediente.ResultadoExpediente ?? "-")
                 .AddRow("Inicio Pruebas", expediente.InicioPruebas.ToString("yyyy-MM-dd"))
                 .AddRow("Fin Pruebas", expediente.FinPruebas.ToString("yyyy-MM-dd"))
                 .AddRow("Fecha Registro", expediente.FechaRegistro.ToString("yyyy-MM-dd"));

            AnsiConsole.Write(table);


        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar el archivo: {ex.Message}[/]");
        }
    }
}
