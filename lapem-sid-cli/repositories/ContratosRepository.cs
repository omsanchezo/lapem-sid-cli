using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class ContratosRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<List<Contrato>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F2_PreparacionFabricacion/Contratos?pageNumber=1&pageSize=20";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<Contrato>>($"Error al obtener contratos: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var contratosResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ContratosResponse>(responseBody);

            if (contratosResponse?.Contratos == null)
            {
                return Result.Fail<List<Contrato>>("No se pudo deserializar la respuesta del servidor");
            }

            return Result.Ok(contratosResponse.Contratos);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<Contrato>>(ex.Message);
        }
    }
}