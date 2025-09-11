using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class InstrumentosRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<List<Instrumento>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F1_ConfiguracionInicial/Instrumento";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<Instrumento>>($"Error al obtener instrumentos: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var instrumentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Instrumento>>(responseBody);
            return Result.Ok(instrumentos ?? new List<Instrumento>());
        }
        catch (Exception ex)
        {
            return Result.Fail<List<Instrumento>>(ex.Message);
        }
    }

    public Result<Instrumento> Create(string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F1_ConfiguracionInicial/Instrumento";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<Instrumento>($"Error al crear instrumento: {errorContent}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var instrumentoCreado = Newtonsoft.Json.JsonConvert.DeserializeObject<Instrumento>(request);

            if (instrumentoCreado == null)
            {
                return Result.Fail<Instrumento>("No se pudo deserializar la respuesta del servidor");
            }

            return Result.Ok(instrumentoCreado);
        }
        catch (Exception ex)
        {
            return Result.Fail<Instrumento>(ex.Message);
        }
    }
}
