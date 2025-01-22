using System.Diagnostics;
using System.Text.Json;
using ControYaApp.Modelss;

namespace ControYaApp.Services.RestService
{
    public class RestService
    {
        private readonly HttpClient _client;
        public List<PrdOrdenProduccion> OrdenesProduccion { get; private set; }

        public RestService()
        {
            OrdenesProduccion = new List<PrdOrdenProduccion>();
            _client = new HttpClient();
        }

        public async Task<List<PrdOrdenProduccion>> GetAllPrdOrdenesProduccionAsync()
        {
            string uri = "https://localhost:7158/ordenes-produccion";
            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var ordenesProduccion = JsonSerializer.Deserialize<List<PrdOrdenProduccion>>(content);
                    if (ordenesProduccion != null)
                    {
                        OrdenesProduccion = ordenesProduccion;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return OrdenesProduccion;
        }
    }
}
