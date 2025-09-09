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

// Enviar comando de autenticación
var command = new AuthenticateUser.Command(usuario, password);
var authResult = await mediator.Send(command);

// Simulación de obtención de token
