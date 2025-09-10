using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class PruebasRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public FluentResults.Result<List<Prueba>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/Prueba";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return FluentResults.Result.Fail<List<Prueba>>($"Error al obtener pruebas: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var pruebas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Prueba>>(responseBody);
            return FluentResults.Result.Ok(pruebas ?? new List<Prueba>());
        }
        catch (Exception ex)
        {
            return FluentResults.Result.Fail<List<Prueba>>(ex.Message);
        }
    }
}