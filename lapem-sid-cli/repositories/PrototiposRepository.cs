using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class PrototiposRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;


    public Result<List<Prototipo>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F1_ConfiguracionInicial/Prototipo";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<Prototipo>>($"Error al obtener prototipos: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var prototipos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Prototipo>>(responseBody);
            return Result.Ok(prototipos ?? new List<Prototipo>());
        }
        catch (Exception ex)
        {
            return Result.Fail<List<Prototipo>>(ex.Message);
        }
    }

    public Result<Prototipo> GetById(string id)
    {
        try
        {
            var allPrototipos = this.GetAll();
            if (allPrototipos.IsFailed)
            {
                return Result.Fail<Prototipo>(allPrototipos.Errors.First().Message);
            }

            var prototipo = allPrototipos.Value.FirstOrDefault(p => p.Id == id);
            if (prototipo == null)
            {
                return Result.Fail<Prototipo>("Prototipo no encontrado.");
            }

            return Result.Ok(prototipo);
        }
        catch (Exception ex)
        {
            return Result.Fail<Prototipo>(ex.Message);
        }
    }

    public Result<Prototipo> Create(string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F1_ConfiguracionInicial/Prototipo";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, content).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<Prototipo>($"Error al crear prototipo: {response.StatusCode}. Detalles: {errorContent}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var prototipoCreado = Newtonsoft.Json.JsonConvert.DeserializeObject<Prototipo>(request);

            if (prototipoCreado == null)
            {
                return Result.Fail<Prototipo>("No se pudo deserializar la respuesta del servidor");
            }

            return Result.Ok(prototipoCreado);
        }
        catch (Exception ex)
        {
            return Result.Fail<Prototipo>(ex.Message);
        }
    }
}