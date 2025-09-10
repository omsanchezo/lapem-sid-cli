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

    public Result<Producto> Create(string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/Producto";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<Producto>($"Error al crear producto: {errorContent}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var productoCreado = new Producto();

            if (productoCreado == null)
            {
                return Result.Fail<Producto>("Error al deserializar el producto creado.");
            }

            return Result.Ok(productoCreado);
        }
        catch (Exception ex)
        {
            return Result.Fail<Producto>(ex.Message);
        }
    }
}