using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class ExpedientePruebaRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<List<ExpedientePrueba>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F2_PreparacionFabricacion/ExpedientePruebas?pageNumber=1&pageSize=20";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<ExpedientePrueba>>($"Error al obtener expedientes de prueba: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var expedientesResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpedientesResponse>(responseBody);

            if (expedientesResponse?.Expedientes == null)
            {
                return Result.Fail<List<ExpedientePrueba>>("No se pudo deserializar la respuesta del servidor");
            }

            return Result.Ok(expedientesResponse.Expedientes);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<ExpedientePrueba>>(ex.Message);
        }
    }
}