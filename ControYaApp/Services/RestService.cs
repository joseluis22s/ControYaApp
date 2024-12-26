using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using ControYaApp.Models;
using System.Net.Http.Json;

namespace ControYaApp.Services
{
    public class RestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public List<PrdOrdenProduccionNotificado> Items { get; private set; }

        public RestService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                // TODO: REVISAR CARACTERÍSTICAS PARA DESERIALIZAR Y SERIALIZAR EL JSON
            };
        }

        public async Task<List<PrdOrdenProduccionNotificado>> GetData(string path)
        {
            // https://learn.microsoft.com/es-es/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
            // https://github.com/MicrosoftDocs/mslearn-dotnetmaui-consume-rest-services/blob/main/src/client/PartsClient/PartsClient/Data/PartsManager.cs
            List<PrdOrdenProduccionNotificado> prdOrdenProduccionNotificadoList = null;
            HttpResponseMessage response = await _client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                prdOrdenProduccionNotificadoList = await response.Content.ReadFromJsonAsync<List<PrdOrdenProduccionNotificado>>();
            }
            return prdOrdenProduccionNotificadoList;
        }
    }
}
