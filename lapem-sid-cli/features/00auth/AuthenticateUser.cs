using FluentResults;
using Newtonsoft.Json;
using MediatR;

namespace lapem_sid_cli.features.auth;

public static class AuthenticateUser
{
    public class Command : IRequest<Result<AuthResult>>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public Command(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class Handler : IRequestHandler<Command, Result<AuthResult>>
    {
        public async Task<Result<AuthResult>> Handle(Command command, CancellationToken cancellationToken)
        {
            try
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
                    return Result.Fail<AuthResult>($"Error de autenticación: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AuthResult>(responseBody);
                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    return Result.Fail<AuthResult>("No se encontró el token en la respuesta de autenticación.");
                }
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                return Result.Fail<AuthResult>(ex.Message);
            }
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