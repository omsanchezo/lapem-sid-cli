using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using MediatR;
using lapem_sid_cli.features.auth;

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
                // Aquí puedes llamar a la lógica correspondiente
                if (instrumentosOption == "Volver") continue;
                AnsiConsole.MarkupLine($"[italic]Seleccionaste: {instrumentosOption}[/]");
                break;
            case "Prototipos":
                var prototiposOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Prototipos[/]")
                        .AddChoices("Ver prototipos", "Agregar prototipo", "Volver"));
                if (prototiposOption == "Volver") continue;
                AnsiConsole.MarkupLine($"[italic]Seleccionaste: {prototiposOption}[/]");
                break;
            case "Normas":
                var normasOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Normas[/]")
                        .AddChoices("Ver normas", "Agregar norma", "Volver"));
                if (normasOption == "Volver") continue;
                AnsiConsole.MarkupLine($"[italic]Seleccionaste: {normasOption}[/]");
                break;
            case "Pruebas":
                var pruebasOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Pruebas[/]")
                        .AddChoices("Ver pruebas", "Agregar prueba", "Volver"));
                if (pruebasOption == "Volver") continue;
                AnsiConsole.MarkupLine($"[italic]Seleccionaste: {pruebasOption}[/]");
                break;
            case "Productos":
                var productosOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Productos[/]")
                        .AddChoices("Ver productos", "Agregar producto", "Volver"));
                if (productosOption == "Volver") continue;
                AnsiConsole.MarkupLine($"[italic]Seleccionaste: {productosOption}[/]");
                break;
        }
    }
    AnsiConsole.MarkupLine("[bold blue]¡Hasta luego![/]");
}
else
{
    AnsiConsole.MarkupLine($"[bold red]Error de autenticación:[/] {authResult.Errors[0].Message}");
}
