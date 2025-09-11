using Spectre.Console;
using lapem_sid_cli.repositories;
using lapem_sid_cli.models;

namespace lapem_sid_cli.features.preparacionfabricacion.expedientesprueba;

public class AddResultadoPrueba
{
    private const string ResultadosPath = @"D:\lapem\lapem-sid-cli\lapem-sid-cli\features\02preparacionfabricacion\expedientesprueba\resultados";

    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        try
        {
            var basePath = ResultadosPath;

            if (!Directory.Exists(basePath))
            {
                AnsiConsole.MarkupLine("[red]Error: No se encontró el directorio de resultados de prueba.[/]");
                return;
            }

            var files = Directory.GetFiles(basePath, "*.json")
                               .Select(Path.GetFileName)
                               .Where(name => name != null)
                               .Select(name => name!)
                               .ToList();

            if (files.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay archivos JSON de resultados de prueba disponibles.[/]");
                return;
            }

            var selectedFile = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Seleccione el archivo de resultado de prueba a cargar:[/]")
                    .PageSize(10)
                    .AddChoices(files.AsEnumerable()));

            var filePath = Path.Combine(basePath, selectedFile);
            AnsiConsole.MarkupLine($"[blue]Archivo seleccionado: {selectedFile}[/]");

            var jsonContent = File.ReadAllText(filePath);
            var resultadosPrueba = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResultadoPruebaRequest>>(jsonContent);

            if (resultadosPrueba == null || resultadosPrueba.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Error: El archivo no contiene resultados de prueba válidos.[/]");
                return;
            }

            // Mostrar lista de resultados y permitir selección
            var selectedResultado = AnsiConsole.Prompt(
                new SelectionPrompt<ResultadoPruebaRequest>()
                    .Title("[green]Seleccione el resultado de prueba a registrar:[/]")
                    .PageSize(10)
                    .UseConverter(r => $"Prueba: {r.IdPrueba}, Valor: {r.ValorMedido}, Fecha: {r.FechaPrueba:yyyy-MM-dd}")
                    .AddChoices(resultadosPrueba));

            // Mostrar y confirmar detalles del resultado seleccionado
            AnsiConsole.MarkupLine("[blue]Detalles del resultado de prueba:[/]");
            AnsiConsole.MarkupLine($"ID Prueba: {selectedResultado.IdPrueba}");
            AnsiConsole.MarkupLine($"ID Valor Referencia: {selectedResultado.IdValorReferencia}");
            AnsiConsole.MarkupLine($"Fecha Prueba: {selectedResultado.FechaPrueba:yyyy-MM-dd}");
            AnsiConsole.MarkupLine($"Operador: {selectedResultado.OperadorPrueba}");
            AnsiConsole.MarkupLine($"ID Instrumento: {selectedResultado.IdInstrumentoMedicion}");
            AnsiConsole.MarkupLine($"Valor Medido: {selectedResultado.ValorMedido}");
            AnsiConsole.MarkupLine($"Resultado: {selectedResultado.Resultado}");
            AnsiConsole.MarkupLine($"Número de Intento: {selectedResultado.NumeroIntento}");

            if (!AnsiConsole.Confirm("[yellow]¿Los detalles son correctos?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada por el usuario.[/]");
                return;
            }

            // Solicitar información del expediente y muestra
            var expediente = AnsiConsole.Ask<string>("[green]Ingrese el número de expediente (ejemplo: FGW-2025-00123):[/]");
            var muestra = AnsiConsole.Ask<string>("[green]Ingrese el número de muestra (ejemplo: M-00123-01):[/]");

            if (!AnsiConsole.Confirm("[yellow]¿Está seguro que desea registrar este resultado de prueba?[/]"))
            {
                AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Enviando resultado de prueba al servidor...[/]");

            var repo = new ExpedientePruebaRepository(auth);
            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(selectedResultado);
            var result = repo.AddResultadoPrueba(expediente, muestra, jsonRequest);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al registrar el resultado de la prueba: {errorMessage}[/]");
                return;
            }

            var table = new Table()
                .Title("[green]Resultado de prueba registrado exitosamente[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("Expediente", expediente)
                 .AddRow("Muestra", muestra)
                 .AddRow("ID Prueba", selectedResultado.IdPrueba ?? "-")
                 .AddRow("ID Valor Referencia", selectedResultado.IdValorReferencia ?? "-")
                 .AddRow("Fecha Prueba", selectedResultado.FechaPrueba.ToString("yyyy-MM-dd HH:mm:ss"))
                 .AddRow("Operador", selectedResultado.OperadorPrueba ?? "-")
                 .AddRow("Instrumento", selectedResultado.IdInstrumentoMedicion ?? "-")
                 .AddRow("Valor Medido", selectedResultado.ValorMedido.ToString("N2"))
                 .AddRow("Resultado", selectedResultado.Resultado ?? "-")
                 .AddRow("Intento", selectedResultado.NumeroIntento.ToString());

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar la solicitud: {ex.Message}[/]");
        }
    }
}
