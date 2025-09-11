using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories;

public class ValoresReferenciaRepository(AuthResult auth)
{
    private readonly AuthResult _auth = auth;

    public Result<List<ValorReferencia>> GetAll()
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/ValorReferencia";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<List<ValorReferencia>>($"Error al obtener valores de referencia: {response.StatusCode}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var valores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValorReferencia>>(responseBody);

            // Cargar las relaciones
            foreach (var valor in valores ?? [])
            {
                if (!string.IsNullOrEmpty(valor.IdProducto))
                {
                    var productosRepo = new ProductosRepository(_auth);
                    var productoResult = productosRepo.GetById(valor.IdProducto);
                    if (productoResult.IsSuccess)
                    {
                        valor.Producto = productoResult.Value;
                    }
                }

                if (!string.IsNullOrEmpty(valor.IdPrueba))
                {
                    var pruebasRepo = new PruebasRepository(_auth);
                    var pruebaResult = pruebasRepo.GetById(valor.IdPrueba);
                    if (pruebaResult.IsSuccess)
                    {
                        valor.Prueba = pruebaResult.Value;
                    }
                }
            }

            return Result.Ok(valores ?? new List<ValorReferencia>());
        }
        catch (Exception ex)
        {
            return Result.Fail<List<ValorReferencia>>(ex.Message);
        }
    }

    public Result<ValorReferencia> Create(string request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/ValorReferencia";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync(url, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<ValorReferencia>($"Error al crear valor de referencia: {errorContent}");
            }

            var valorCreado = Newtonsoft.Json.JsonConvert.DeserializeObject<ValorReferencia>(request);

            if (valorCreado == null)
            {
                return Result.Fail<ValorReferencia>("Error al deserializar el valor de referencia creado.");
            }

            return Result.Ok(valorCreado);
        }
        catch (Exception ex)
        {
            return Result.Fail<ValorReferencia>(ex.Message);
        }
    }
}
