using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class NormasRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<List<Norma>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/Norma";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<Norma>>($"Error al obtener normas: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var normas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Norma>>(responseBody);
            return Result.Ok(normas ?? new List<Norma>());
        }
        catch (Exception ex)
        {
            return Result.Fail<List<Norma>>(ex.Message);
        }
    }
}