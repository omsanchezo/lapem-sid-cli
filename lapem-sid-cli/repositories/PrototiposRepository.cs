using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class PrototiposRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<List<Prototipo>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/Prototipo";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<Prototipo>>($"Error al obtener prototipos: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var prototipos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Prototipo>>(responseBody);
            return Result.Ok(prototipos ?? new List<Prototipo>());
        }
        catch (Exception ex)
        {
            return Result.Fail<List<Prototipo>>(ex.Message);
        }
    }
}