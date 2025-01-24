using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using ControYaApp.Models;

namespace ControYaApp.Services.RestService
{
    public class RestService
    {
        private readonly HttpClient _client;
        public ObservableCollection<PrdOrdenProduccion> OrdenesProduccion { get; private set; }

        public RestService()
        {
            OrdenesProduccion = new ObservableCollection<PrdOrdenProduccion>();
            _client = new HttpClient();
        }

        public async Task<ObservableCollection<PrdOrdenProduccion>> GetAllPrdOrdenesProduccionAsync()
        {
            //string uri = "https://localhost:7158/ordenes-produccion";
            string uri = "http://192.168.47.69:100/ordenes-produccion";
            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var ordenesProduccion = JsonSerializer.Deserialize<ObservableCollection<PrdOrdenProduccion>>(content);
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
