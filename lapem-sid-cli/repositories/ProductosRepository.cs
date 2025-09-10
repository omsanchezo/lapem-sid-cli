using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class ProductosRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<List<Producto>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/Producto";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<Producto>>($"Error al obtener productos: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var productos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Producto>>(responseBody);
            return Result.Ok(productos ?? new List<Producto>());
        }
        catch (Exception ex)
        {
            return Result.Fail<List<Producto>>(ex.Message);
        }
    }
}