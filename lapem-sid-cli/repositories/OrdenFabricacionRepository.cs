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


    public Result<OrdenFabricacion> Create(string request)
    {
        try
        {
            var orden = Newtonsoft.Json.JsonConvert.DeserializeObject<OrdenFabricacion>(request);
            if (orden == null)
                return Result.Fail<OrdenFabricacion>("No se pudo deserializar la orden de fabricación creada");

            // Verifica que existe el producto asociado
            var productosRepo = new ProductosRepository(_auth);
            var productoResult = productosRepo.GetById(orden.IdProducto ?? string.Empty);
            if (productoResult.IsFailed)
            {
                return Result.Fail<OrdenFabricacion>($"El producto asociado no existe: {productoResult.Errors.First().Message}");
            }

            // Verifica los detalles de fabricación
            if (orden.DetalleFabricacion == null || orden.DetalleFabricacion.Count == 0)
            {
                return Result.Fail<OrdenFabricacion>("La orden de fabricación debe tener al menos un detalle de fabricación.");
            }
            var contratosRepo = new ContratosRepository(_auth);
            foreach (var detalle in orden.DetalleFabricacion)
            {
                // Verifica el contrato                
                var contratoResult = contratosRepo.GetById(detalle.ContratoId ?? string.Empty);
                if (contratoResult.IsFailed)
                {
                    return Result.Fail<OrdenFabricacion>($"El contrato asociado en el detalle de fabricación no existe: {contratoResult.Errors.First().Message}");
                }
                // Verifica la partida
                var partida = contratoResult.Value.DetalleContrato?.FirstOrDefault(x => x.PartidaContrato == detalle.PartidaContratoId);
                if (partida == null)
                {
                    return Result.Fail<OrdenFabricacion>($"La partida de contrato '{detalle.PartidaContratoId}' no existe en el contrato '{detalle.ContratoId}'.");
                }
            }

            using var httpClient = new HttpClient();
            var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F2_PreparacionFabricacion/OrdenFabricacion";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_auth.Token}");
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, content).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                return Result.Fail<OrdenFabricacion>($"Error al crear la orden de fabricación: {errorContent}");
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseBody);
            return Result.Ok(orden);
        }
        catch (Exception ex)
        {
            return Result.Fail<OrdenFabricacion>($"Error al crear la orden de fabricación: {ex.Message}");
        }
    }
}