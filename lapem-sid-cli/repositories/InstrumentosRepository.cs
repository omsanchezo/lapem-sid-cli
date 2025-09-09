using FluentResults;
using lapem_sid_cli.features.auth;
using lapem_sid_cli.models;

namespace lapem_sid_cli.repositories
{
    public class InstrumentosRepository
    {
        private readonly AuthResult _auth;

        public InstrumentosRepository(AuthResult auth)
        {
            _auth = auth;
        }

        public Result<List<Instrumento>> GetAll()
        {
            try
            {
                using var httpClient = new HttpClient();
                var url = "https://lapem.cfe.gob.mx/sid_capacitacion/F1_ConfiguracionInicial/Instrumento";
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
    }
}