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
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F2_PreparacionFabricacion/ExpedientePruebas?pageNumber=1&pageSize=20";
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

    public Result<ExpedientePrueba> GetByExpediente(string expedienteValue)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = $"https://lapem.cfe.gob.mx/sid_evaluacion/F2_PreparacionFabricacion/ExpedientePruebas/{expedienteValue}";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<ExpedientePrueba>($"Error al obtener el expediente de prueba: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var expedientes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExpedientePrueba>>(responseBody);

            if (expedientes == null || expedientes.Count == 0)
            {
                return Result.Fail<ExpedientePrueba>($"No se encontró el expediente {expedienteValue}");
            }

            return Result.Ok(expedientes[0]);
        }
        catch (Exception ex)
        {
            return Result.Fail<ExpedientePrueba>(ex.Message);
        }
    }

    public Result<ExpedientePrueba> Create(string request)
    {
        try
        {

            var createExpedienteCommand = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateExpediente>(request);
            if (createExpedienteCommand == null)
                return Result.Fail<ExpedientePrueba>("No se pudo deserializar el expediente de prueba creado");

            // Verifica que existe la orden de fabricación asociada
            // var ordenRepo = new OrdenFabricacionRepository(_auth);
            // var ordenResult = ordenRepo.GetById(createExpedienteCommand.OrdenFabricacion ?? string.Empty);
            // if (ordenResult.IsFailed)
            // {
            //     return Result.Fail<ExpedientePrueba>($"La orden de fabricación no existe: {ordenResult.Errors.First().Message}");
            // }

            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_evaluacion/F2_PreparacionFabricacion/ExpedientePruebas";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<ExpedientePrueba>($"Error al crear expediente de prueba: {errorContent}");
            }

            var expedienteCreado = new ExpedientePrueba();
            return Result.Ok(expedienteCreado);
        }
        catch (Exception ex)
        {
            return Result.Fail<ExpedientePrueba>(ex.Message);
        }
    }

    public Result<bool> AddResultadoPrueba(string expediente, string muestra, string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = $"https://lapem.cfe.gob.mx/sid_evaluacion/F3_Pruebas/AgregaResultadoPrueba?expediente={expediente}&muestra={muestra}";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "text/plain");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PutAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<bool>($"Error al agregar resultado de prueba: {errorContent}");
            }

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>(ex.Message);
        }
    }
}