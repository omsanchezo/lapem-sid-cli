using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using MediatR;
using lapem_sid_cli.features.auth;
using System.Runtime.Serialization;

// Configuración de DI y MediatR
var services = new ServiceCollection();
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<lapem_sid_cli.features.auth.AuthenticateUser.Handler>());
var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();

AnsiConsole.MarkupLine("[bold yellow]Autenticación requerida[/]");

var usuario = AnsiConsole.Ask<string>("Ingrese su [green]usuario[/]:");
var password = AnsiConsole.Prompt(
    new TextPrompt<string>("Ingrese su [red]contraseña[/]:")
        .PromptStyle("red")
        .Secret());

// Enviar comando de autenticación con indicador de espera
var command = new AuthenticateUser.Command(usuario, password);
var authResult = await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("green"))
    .StartAsync("Validando credenciales...", async ctx =>
    {
        return await mediator.Send(command);
    });

// Simulación de obtención de token

if (authResult.IsSuccess)
{
    AnsiConsole.MarkupLine("[bold green]Autenticación exitosa![/]");

    while (true)
    {
        var mainOption = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold yellow]Seleccione una opción:[/]")
                .AddChoices(new[] {
                    "Instrumentos",
                    "Prototipos",
                    "Normas",
                    "Pruebas",
                    "Productos",
                    "Valores de Referencia",
                    "Contratos",
                    "Orden de Fabricación",
                    "Expedientes de Prueba",
                    "Salir"
                }));

        if (mainOption == "Salir")
            break;

        switch (mainOption)
        {
            case "Instrumentos":
                var instrumentosOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Instrumentos[/]")
                        .AddChoices("Ver instrumentos", "Agregar instrumento", "Volver"));
                if (instrumentosOption == "Volver") continue;
                if (instrumentosOption == "Ver instrumentos")
                {
                    lapem_sid_cli.features.configuracion.instrumentos.ViewInstrumentos.Show(authResult.Value);
                }
                else if (instrumentosOption == "Agregar instrumento")
                {
                    lapem_sid_cli.features.configuracion.instrumentos.AddInstrumento.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {instrumentosOption}[/]");
                }
                break;
            case "Prototipos":
                var prototiposOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Prototipos[/]")
                        .AddChoices("Ver prototipos", "Agregar prototipo", "Volver"));
                if (prototiposOption == "Volver") continue;
                if (prototiposOption == "Ver prototipos")
                {
                    lapem_sid_cli.features.configuracion.prototipos.ViewPrototipos.Show(authResult.Value);
                }
                else if (prototiposOption == "Agregar prototipo")
                {
                    lapem_sid_cli.features.configuracion.prototipos.AddPrototipo.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {prototiposOption}[/]");
                }
                break;
            case "Normas":
                var normasOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Normas[/]")
                        .AddChoices("Ver normas", "Agregar norma", "Volver"));
                if (normasOption == "Volver") continue;
                if (normasOption == "Ver normas")
                {
                    lapem_sid_cli.features.configuracion.normas.ViewNormas.Show(authResult.Value);
                }
                else if (normasOption == "Agregar norma")
                {
                    lapem_sid_cli.features.configuracion.normas.AddNorma.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {normasOption}[/]");
                }
                break;
            case "Pruebas":
                var pruebasOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Pruebas[/]")
                        .AddChoices("Ver pruebas", "Agregar prueba", "Volver"));
                if (pruebasOption == "Volver") continue;
                if (pruebasOption == "Ver pruebas")
                {
                    lapem_sid_cli.features.configuracion.pruebas.ViewPruebas.Show(authResult.Value);
                }
                else if (pruebasOption == "Agregar prueba")
                {
                    lapem_sid_cli.features.configuracion.pruebas.AddPrueba.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {pruebasOption}[/]");
                }
                break;
            case "Productos":
                var productosOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Productos[/]")
                        .AddChoices("Ver productos", "Ver un producto", "Agregar producto", "Volver"));
                if (productosOption == "Volver") continue;
                if (productosOption == "Ver productos")
                {
                    lapem_sid_cli.features.configuracion.productos.ViewProductos.Show(authResult.Value);
                }
                else if (productosOption == "Ver un producto")
                {
                    lapem_sid_cli.features.configuracion.productos.ViewProductoById.Show(authResult.Value);
                }
                else if (productosOption == "Agregar producto")
                {
                    lapem_sid_cli.features.configuracion.productos.AddProducto.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {productosOption}[/]");
                }
                break;
            case "Valores de Referencia":
                var valoresOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Valores de Referencia[/]")
                        .AddChoices("Ver valores de referencia", "Agregar valor de referencia", "Volver"));
                if (valoresOption == "Volver") continue;
                if (valoresOption == "Ver valores de referencia")
                {
                    lapem_sid_cli.features.configuracion.valoresReferencia.ViewValorReferencia.Show(authResult.Value);
                }
                else if (valoresOption == "Agregar valor de referencia")
                {
                    lapem_sid_cli.features.configuracion.valoresReferencia.AddValorReferencia.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {valoresOption}[/]");
                }
                break;
            case "Contratos":
                var contratosOptions = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("[green]Contratos[/]")
                        .AddChoices("Ver contratos", "Ver contrato por ID", "Agregar contrato", "Volver"));
                if (contratosOptions == "Volver") continue;
                if (contratosOptions == "Ver contratos")
                {
                    lapem_sid_cli.features.preparacionfabricacion.contratos.ViewContratos.Show(authResult.Value);
                }
                else if (contratosOptions == "Ver contrato por ID")
                {
                    lapem_sid_cli.features.preparacionfabricacion.contratos.ViewContratoById.Show(authResult.Value);
                }
                else if (contratosOptions == "Agregar contrato")
                {
                    lapem_sid_cli.features.preparacionfabricacion.contratos.AddContrato.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {contratosOptions}[/]");
                }
                break;
            case "Orden de Fabricación":
                var ordenFabricacionOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Orden de Fabricación[/]")
                        .AddChoices("Ver orden por ID", "Agregar orden de fabricación", "Volver"));
                if (ordenFabricacionOption == "Volver") continue;
                if (ordenFabricacionOption == "Ver orden por ID")
                {
                    lapem_sid_cli.features.preparacionfabricacion.ordenesfabricacion.ViewOrdenFabricacionById.Show(authResult.Value);
                }
                else if (ordenFabricacionOption == "Agregar orden de fabricación")
                {
                    lapem_sid_cli.features.preparacionfabricacion.ordenesfabricacion.AddOrdenFabricacion.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {ordenFabricacionOption}[/]");
                }
                break;
            case "Expedientes de Prueba":
                var expedientesOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Expedientes de Prueba[/]")
                        .AddChoices("Ver expedientes de prueba", "Ver un expediente de prueba", "Agregar expediente de prueba", "Volver"));
                if (expedientesOption == "Volver") continue;
                if (expedientesOption == "Ver expedientes de prueba")
                {
                    lapem_sid_cli.features.preparacionfabricacion.expedientesprueba.ViewExpedientesPrueba.Show(authResult.Value);
                }
                else if (expedientesOption == "Ver un expediente de prueba")
                {
                    lapem_sid_cli.features.preparacionfabricacion.expedientesprueba.ViewExpedienteByValue.Show(authResult.Value);
                }
                else if (expedientesOption == "Agregar expediente de prueba")
                {
                    lapem_sid_cli.features.preparacionfabricacion.expedientesprueba.AddExpedientePrueba.Show(authResult.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[italic]Seleccionaste: {expedientesOption}[/]");
                }
                break;
        }
    }
    AnsiConsole.MarkupLine("[bold blue]¡Hasta luego![/]");
}
else
{
    AnsiConsole.MarkupLine($"[bold red]Error de autenticación:[/] {authResult.Errors[0].Message}");
}
