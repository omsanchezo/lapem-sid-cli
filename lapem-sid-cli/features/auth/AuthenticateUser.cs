
using MediatR;

namespace lapem_sid_cli.features.auth;

public static class AuthenticateUser
{
    public class Command : IRequest<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public Command(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class Handler : IRequestHandler<Command, string>
    {
        public Task<string> Handle(Command command, CancellationToken cancellationToken)
        {
            // Aquí podrías agregar la lógica de autenticación
            Console.WriteLine($"Usuario: {command.Username}");
            Console.WriteLine("Credenciales capturadas. (Simulación de autenticación)");
            return Task.FromResult("TokenDeAutenticacionSimulado");
        }
    }
}
