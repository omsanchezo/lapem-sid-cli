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
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F1_ConfiguracionInicial/Norma";
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

    public Result<Norma> GetById(string id)
    {
        try
        {
            var allNormas = this.GetAll();
            if (allNormas.IsFailed)
            {
                return Result.Fail<Norma>(allNormas.Errors.First().Message);
            }

            var norma = allNormas.Value.FirstOrDefault(n => n.Id == id);
            if (norma == null)
            {
                return Result.Fail<Norma>("Norma no encontrada.");
            }

            return Result.Ok(norma);
        }
        catch (Exception ex)
        {
            return Result.Fail<Norma>(ex.Message);
        }
    }

    public Result<Norma> Create(string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F1_ConfiguracionInicial/Norma";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<Norma>($"Error al crear norma: {errorContent}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var normaCreada = Newtonsoft.Json.JsonConvert.DeserializeObject<Norma>(request);

            if (normaCreada == null)
            {
                return Result.Fail<Norma>("No se pudo deserializar la respuesta del servidor");
            }

            return Result.Ok(normaCreada);
        }
        catch (Exception ex)
        {
            return Result.Fail<Norma>(ex.Message);
        }
    }
}