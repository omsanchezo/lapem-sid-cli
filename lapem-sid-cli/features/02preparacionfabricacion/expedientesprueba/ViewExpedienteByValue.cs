using lapem_sid_cli.repositories;
using Spectre.Console;

namespace lapem_sid_cli.features.preparacionfabricacion.expedientesprueba;

public class ViewExpedienteByValue
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var expedienteValue = AnsiConsole.Ask<string>("[green]Ingrese la clave del expediente (ej: EXP-2025-01):[/]");

        var repo = new ExpedientePruebaRepository(auth);
        var result = repo.GetByExpediente(expedienteValue);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
            Console.WriteLine($"Error: {errorMessage}");
            return;
        }

        var expediente = result.Value;

        // Tabla principal con los datos del expediente
        var table = new Table().Title($"[bold]Expediente: {expediente.ClaveExpediente}[/]")
            .AddColumn("ID")
            .AddColumn("Tipo Muestreo")
            .AddColumn("Tamaño Muestra")
            .AddColumn("Max. Rechazos")
            .AddColumn("Estatus")
            .AddColumn("Resultado")
            .AddColumn("Inicio")
            .AddColumn("Fin");

        table.AddRow(
            expediente.Id ?? "",
            expediente.TipoMuestreo ?? "",
            expediente.TamanioMuestra.ToString(),
            expediente.MaximoRechazos.ToString(),
            expediente.EstatusPruebas ?? "",
            expediente.ResultadoExpediente ?? "",
            expediente.InicioPruebas.ToString("yyyy-MM-dd HH:mm"),
            expediente.FinPruebas.ToString("yyyy-MM-dd HH:mm")
        );

        AnsiConsole.Write(table);

        // Si hay muestras, mostrar una tabla con las muestras y sus pruebas
        if (expediente.MuestrasExpediente?.Count > 0)
        {
            var muestrasTable = new Table().Title("[bold]Muestras del Expediente[/]")
                .AddColumn("Identificador")
                .AddColumn("Estatus")
                .AddColumn("Pruebas")
                .AddColumn("Operador")
                .AddColumn("Fecha")
                .AddColumn("Resultado");

            foreach (var muestra in expediente.MuestrasExpediente)
            {
                var resultadosPruebas = muestra.ResultadosPruebas?
                    .Select(r => $"{r.Prueba?.Nombre}: {r.ValorMedido} {r.ValorReferencia?.Unidad ?? ""} (Intento {r.NumeroIntento})")
                    .ToList() ?? new List<string>();

                muestrasTable.AddRow(
                    muestra.Identificador ?? "",
                    muestra.Estatus ?? "",
                    string.Join("\\n", resultadosPruebas),
                    muestra.ResultadosPruebas?.FirstOrDefault()?.OperadorPrueba ?? "",
                    muestra.ResultadosPruebas?.FirstOrDefault()?.FechaPrueba.ToString("yyyy-MM-dd HH:mm") ?? "",
                    string.Join("\\n", muestra.ResultadosPruebas?.Select(r => r.Resultado) ?? Array.Empty<string>())
                );
            }

            AnsiConsole.WriteLine(); // Línea en blanco para separar las tablas
            AnsiConsole.Write(muestrasTable);
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Este expediente no tiene muestras registradas.[/]");
        }

        // Si hay orden de fabricación, mostrar sus detalles
        if (expediente.OrdenFabricacion != null)
        {
            var ordenTable = new Table().Title("[bold]Orden de Fabricación[/]")
                .AddColumn("ID")
                .AddColumn("Clave")
                .AddColumn("Lote")
                .AddColumn("ID Producto");

            ordenTable.AddRow(
                expediente.OrdenFabricacion.Id ?? "",
                expediente.OrdenFabricacion.ClaveOrdenFabricacion ?? "",
                expediente.OrdenFabricacion.LoteFabricacion ?? "",
                expediente.OrdenFabricacion.IdProducto ?? ""
            );

            AnsiConsole.WriteLine(); // Línea en blanco para separar las tablas
            AnsiConsole.Write(ordenTable);

            // Si la orden tiene detalles de fabricación, mostrarlos
            if (expediente.OrdenFabricacion.DetalleFabricacion?.Count > 0)
            {
                var detalleTable = new Table().Title("[bold]Detalles de Fabricación[/]")
                    .AddColumn("Contrato")
                    .AddColumn("Tipo")
                    .AddColumn("Partida")
                    .AddColumn("Descripción")
                    .AddColumn("Unidad")
                    .AddColumn("Cant. Original")
                    .AddColumn("Cant. a Fabricar");

                foreach (var detalle in expediente.OrdenFabricacion.DetalleFabricacion)
                {
                    detalleTable.AddRow(
                        detalle.ContratoId ?? "",
                        detalle.TipoContrato ?? "",
                        detalle.PartidaContratoId ?? "",
                        detalle.DescripcionPartida ?? "",
                        detalle.Unidad ?? "",
                        detalle.CantidadOriginalContrato.ToString("N2"),
                        detalle.CantidadAFabricar.ToString("N2")
                    );
                }

                AnsiConsole.WriteLine(); // Línea en blanco para separar las tablas
                AnsiConsole.Write(detalleTable);
            }
        }

        // Si hay avisos de prueba, mostrarlos
        if (expediente.AvisosPrueba?.Count > 0)
        {
            AnsiConsole.WriteLine(); // Línea en blanco para separar
            AnsiConsole.MarkupLine("[bold]Avisos de Prueba:[/]");
            foreach (var aviso in expediente.AvisosPrueba)
            {
                AnsiConsole.MarkupLine($"- {aviso}");
            }
        }
    }
}
