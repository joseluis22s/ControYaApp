using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using ControYaApp.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ControYaApp.Services.WebService
{
    public class RestService
    {
        private readonly string _uri = "http://192.168.47.4:100";
        private readonly HttpClient _client = new();

        private JsonSerializerSettings _jsonSerializerSettings;

        public RestService()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }


        public async Task<Dictionary<string, object>> CheckUsuarioCredentialsAsync(Usuario usuario)
        {
            string uri = _uri + "/usuarios/loginusuario";
            try
            {
                var usuarioLogin = new
                {
                    NombreUsuario = usuario.NombreUsuario,
                    UsuarioSistema = "",
                    Contrasena = usuario.Contrasena
                };
                string json = JsonConvert.SerializeObject(usuarioLogin, _jsonSerializerSettings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(resContent);

                    if (values != null &&
                        values.TryGetValue("estaRegistrado", out object? estaRegistrado) &&
                        values.TryGetValue("usuarioSistema", out object? usuarioSistema))
                    {
                        return new Dictionary<string, object>
                        {
                            { "estaRegistrado", estaRegistrado },
                            { "usuarioSistema", usuarioSistema }
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return new Dictionary<string, object>
                        {
                            { "estaRegistrado", false },
                            { "usuarioSistema", "" }
                        };
        }


        public async Task<ObservableCollection<Usuario>> GetAllUsuariosAsync()
        {
            string uri = _uri + "/usuarios/getall";

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<Usuario>>>(content, _jsonSerializerSettings);
                    if (values.IsNullOrEmpty() &&
                        values.TryGetValue("usuarios", out ObservableCollection<Usuario>? usuarios))
                    {
                        return new ObservableCollection<Usuario>(usuarios);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }

        public async Task<ObservableCollection<OrdenProduccion>> GetAllOrdenesProduccionByUsuarioAsync(string? usuarioSistema)
        {
            string uri = _uri + $"/ordenes/by-usuario";
            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioSistema,
                Contrasena = ""
            };
            try
            {
                string json = JsonConvert.SerializeObject(usuario, _jsonSerializerSettings);
                StringContent request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<OrdenProduccion>>>(content, _jsonSerializerSettings);

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

    }
}
