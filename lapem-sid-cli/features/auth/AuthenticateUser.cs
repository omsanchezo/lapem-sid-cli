using Newtonsoft.Json;
using MediatR;

namespace lapem_sid_cli.features.auth;

public static class AuthenticateUser
{
    public class Command : IRequest<AuthResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public Command(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class Handler : IRequestHandler<Command, AuthResult>
    {
        public async Task<AuthResult> Handle(Command command, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F0_Acceso/Login";
            var payload = new
            {
                username = command.Username,
                password = command.Password
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error de autenticación: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthResult>(responseBody);
            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                throw new Exception("No se encontró el token en la respuesta de autenticación.");
            }
            return result;
        }
    }
}


public class AuthResult
{
    public string? Token { get; set; }
    public UserDetails? UserDetails { get; set; }
    public string? LastUsage { get; set; }
}

public class UserDetails
{
    public string? nombre { get; set; }
    public string? username { get; set; }
    public string? password { get; set; }
    public string? empresa { get; set; }
    public string? idEmpresa { get; set; }
    public string? rfc { get; set; }
    public string? rol { get; set; }
    public int maximoIntentosPruebas { get; set; }
    public string[]? ip { get; set; }
}