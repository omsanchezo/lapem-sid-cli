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
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F2_PreparacionFabricacion/Contratos?pageNumber=1&pageSize=20";
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

    public Result<Contrato> GetById(string id)
    {
        try
        {
            var allContratos = this.GetAll();
            if (allContratos.IsFailed)
            {
                return Result.Fail<Contrato>(allContratos.Errors.First().Message);
            }

            var contrato = allContratos.Value.FirstOrDefault(c => c.Id == id);
            if (contrato == null)
            {
                return Result.Fail<Contrato>("Contrato no encontrado.");
            }

            return Result.Ok(contrato);
        }
        catch (Exception ex)
        {
            return Result.Fail<Contrato>(ex.Message);
        }
    }

    public Result<Contrato> Create(string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F2_PreparacionFabricacion/Contratos";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<Contrato>($"Error al crear contrato: {errorContent}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var contratoCreado = Newtonsoft.Json.JsonConvert.DeserializeObject<Contrato>(request);

            if (contratoCreado == null)
            {
                return Result.Fail<Contrato>("No se pudo deserializar la respuesta del servidor");
            }

            return Result.Ok(contratoCreado);
        }
        catch (Exception ex)
        {
            return Result.Fail<Contrato>(ex.Message);
        }
    }
}