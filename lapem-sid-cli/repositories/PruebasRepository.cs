using FluentResults;
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

    public FluentResults.Result<Prueba> GetById(string id)
    {
        try
        {
            var allPruebas = this.GetAll();
            if (allPruebas.IsFailed)
            {
                return FluentResults.Result.Fail<Prueba>(allPruebas.Errors.First().Message);
            }

            var prueba = allPruebas.Value.FirstOrDefault(p => p.Id == id);
            if (prueba == null)
            {
                return FluentResults.Result.Fail<Prueba>("Prueba no encontrada.");
            }

            return FluentResults.Result.Ok(prueba);
        }
        catch (Exception ex)
        {
            return FluentResults.Result.Fail<Prueba>(ex.Message);
        }
    }

    public Result<Prueba> Create(string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/Prueba";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<Prueba>($"Error al crear prueba: {errorContent}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var pruebaCreada = Newtonsoft.Json.JsonConvert.DeserializeObject<Prueba>(request);

            if (pruebaCreada == null)
            {
                return Result.Fail<Prueba>("Error al deserializar la respuesta de la prueba creada.");
            }

            return Result.Ok(pruebaCreada);
        }
        catch (Exception ex)
        {
            return Result.Fail<Prueba>(ex.Message);
        }
    }
}