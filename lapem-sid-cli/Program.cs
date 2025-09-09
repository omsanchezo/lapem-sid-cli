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
}
else
{
    AnsiConsole.MarkupLine($"[bold red]Error de autenticación:[/] {authResult.Errors[0].Message}");
}
