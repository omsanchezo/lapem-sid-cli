using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class OrdenFabricacionRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<OrdenFabricacion> GetById(string id)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = $"https://lapem.cfe.gob.mx/sid_capacitacion/F2_PreparacionFabricacion/OrdenFabricacion/{id}";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<OrdenFabricacion>($"Error al obtener la orden de fabricación: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var ordenes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrdenFabricacion>>(responseBody);

            if (ordenes == null || ordenes.Count == 0)
            {
                return Result.Fail<OrdenFabricacion>("No se encontró la orden de fabricación");
            }

            return Result.Ok(ordenes[0]);
        }
        catch (Exception ex)
        {
            return Result.Fail<OrdenFabricacion>($"Error al obtener la orden de fabricación: {ex.Message}");
        }
    }
}