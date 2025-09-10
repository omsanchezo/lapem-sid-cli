using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class ValoresRefenciaRepository
{
    private readonly AuthResult _auth;

    public ValoresRefenciaRepository(AuthResult auth)
    {
        _auth = auth;
    }

    public Result<List<ValorReferencia>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/ValorReferencia";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<ValorReferencia>>($"Error al obtener valores de referencia: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var valores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValorReferencia>>(responseBody);
            return Result.Ok(valores ?? new List<ValorReferencia>());
        }
        catch (Exception ex)
        {
            return Result.Fail<List<ValorReferencia>>(ex.Message);
        }
    }
}
