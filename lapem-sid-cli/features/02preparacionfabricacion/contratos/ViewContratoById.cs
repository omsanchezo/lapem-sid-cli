using Spectre.Console;
using lapem_sid_cli.repositories;

namespace lapem_sid_cli.features.preparacionfabricacion.contratos;

public class ViewContratoById
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var id = AnsiConsole.Ask<string>("[green]Ingrese el ID del contrato:[/]");

        try
        {
            AnsiConsole.MarkupLine("[blue]Buscando contrato...[/]");

            var repo = new ContratosRepository(auth);
            var result = repo.GetById(id);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al obtener el contrato: {errorMessage}[/]");
                return;
            }

            var contrato = result.Value;
            var table = new Table()
                .Title($"[green]Contrato {contrato.Id}[/]")
                .AddColumn("Campo")
                .AddColumn("Valor");

            table.AddRow("ID", contrato.Id ?? "-")
                 .AddRow("Tipo de Contrato", contrato.TipoContrato ?? "-")
                 .AddRow("No. Contrato", contrato.NoContrato ?? "-")
                 .AddRow("Estatus", contrato.Estatus ?? "-")
                 .AddRow("URL del Archivo", contrato.UrlArchivo ?? "-")
                 .AddRow("MD5", contrato.MD5 ?? "-")
                 .AddRow("Fecha Entrega CFE", contrato.FechaEntregaCFE?.ToString("yyyy-MM-dd") ?? "-");

            AnsiConsole.Write(table);

            // Mostrar detalles del contrato si existen
            if (contrato.DetalleContrato?.Any() == true)
            {
                AnsiConsole.WriteLine(); // Línea en blanco para separación
                var detallesTable = new Table()
                    .Title("[blue]Detalles del Contrato[/]")
                    .AddColumn("Partida")
                    .AddColumn("Descripción")
                    .AddColumn("Área Destino")
                    .AddColumn("Cantidad")
                    .AddColumn("Unidad")
                    .AddColumn("Importe Total");

                foreach (var detalle in contrato.DetalleContrato)
                {
                    detallesTable.AddRow(
                        detalle.PartidaContrato ?? "-",
                        detalle.DescripcionAviso ?? "-",
                        detalle.AreaDestinoCFE ?? "-",
                        detalle.Cantidad.ToString("N2"),
                        detalle.Unidad ?? "-",
                        detalle.ImporteTotal.ToString("C2")
                    );
                }

                AnsiConsole.Write(detallesTable);
            }
            else
            {
                AnsiConsole.MarkupLine("\n[yellow]Este contrato no tiene detalles asociados.[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar la solicitud: {ex.Message}[/]");
        }
    }
}
