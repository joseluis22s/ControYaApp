using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ControYaApp.Models;

namespace ControYaApp.Services.WebService
{
    public class RestService
    {
        private readonly string _uri = "http://192.168.47.4:100";
        private readonly HttpClient _client = new();


        public ObservableCollection<OrdenProduccion> OrdenesProduccion { get; private set; }


        public RestService()
        {
            OrdenesProduccion = [];
        }


        public async Task<Dictionary<string, object>> CheckUsuarioCredentialsAsync(Usuario usuario)
        {
            string uri = _uri + "/usuarios/login-usuario";
            try
            {
                string json = JsonSerializer.Serialize<Usuario>(usuario, SerializerOptions());
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, object>>(resContent);

                    //var estaResgitrado = (bool?)values?["estaResgitrado"];
                    //var usuarioSistema = (Usuario?)values?["usuarioSistema"];

                    if (values != null &&
                        values.TryGetValue("estaResgitrado", out object? estaResgitrado) &&
                        values.TryGetValue("usuarioSistema", out object? usuarioSistema))
                    {
                        return new Dictionary<string, object>
                        {
                            { "estaResgitrado", estaResgitrado },
                            { "usuarioSistema", usuarioSistema }
                        };
                    }
                    else
                    {
                        return [];
                    }

                }
                return [];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return [];
            }
        }

        public async Task<ObservableCollection<OrdenProduccion>> GetAllOrdenesProduccionAsync(string? nombreUsuario)
        {
            string uri = _uri + $"/ordenes/by-usuario";
            Usuario usuario = new Usuario()
            {
                NombreUsuario = nombreUsuario,
                Contrasena = ""
            };
            try
            {
                string json = JsonSerializer.Serialize<Usuario>(usuario, SerializerOptions());
                StringContent request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<OrdenProduccion>>>(content, SerializerOptions());

                    if (deserialized != null && deserialized.TryGetValue("ordenes", out ObservableCollection<OrdenProduccion>? ordenesProduccion))
                    {
                        return ordenesProduccion;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return [];
        }

        private JsonSerializerOptions SerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
    }
}
