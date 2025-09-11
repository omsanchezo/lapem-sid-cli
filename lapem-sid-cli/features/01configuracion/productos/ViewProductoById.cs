using Spectre.Console;
using lapem_sid_cli.repositories;

namespace lapem_sid_cli.features.configuracion.productos;

public class ViewProductoById
{
    public static void Show(lapem_sid_cli.features.auth.AuthResult auth)
    {
        var id = AnsiConsole.Ask<string>("[green]Ingrese el ID del producto:[/]");

        try
        {
            AnsiConsole.MarkupLine("[blue]Buscando producto...[/]");

            var repo = new ProductosRepository(auth);
            var result = repo.GetById(id);

            if (result.IsFailed)
            {
                var errorMessage = result.Errors[0].Message.Replace("[", "\\[").Replace("]", "\\]");
                AnsiConsole.MarkupLine($"[red]Error al obtener el producto: {errorMessage}[/]");
                return;
            }

            var producto = result.Value;
            var table = new Table()
                .Title($"[green]Producto {producto.Id}[/]")
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

            // Mostrar pruebas asociadas si existen
            if (producto.Pruebas?.Any() == true)
            {
                AnsiConsole.WriteLine(); // Línea en blanco para separación
                var pruebasTable = new Table()
                    .Title("[blue]Pruebas Asociadas[/]")
                    .AddColumn("ID")
                    .AddColumn("Nombre")
                    .AddColumn("Tipo")
                    .AddColumn("Tipo Resultado")
                    .AddColumn("Estatus");

                foreach (var prueba in producto.Pruebas)
                {
                    pruebasTable.AddRow(
                        prueba.Id ?? "-",
                        prueba.Nombre ?? "-",
                        prueba.TipoPrueba ?? "-",
                        prueba.TipoResultado ?? "-",
                        prueba.Estatus ?? "-"
                    );
                }

                AnsiConsole.Write(pruebasTable);
            }
            else
            {
                AnsiConsole.MarkupLine("\n[yellow]Este producto no tiene pruebas asociadas.[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al procesar la solicitud: {ex.Message}[/]");
        }
    }
}
