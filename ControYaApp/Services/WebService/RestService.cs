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
                    if (!values.IsNullOrEmpty() &&
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

        public async Task<ObservableCollection<OrdenProduccion>> GetOrdenesProduccionAsync()
        {
            string uri = _uri + "/ordenes/getall";

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<OrdenProduccion>>>(content, _jsonSerializerSettings);
                    if (!values.IsNullOrEmpty() &&
                        values.TryGetValue("ordenes", out ObservableCollection<OrdenProduccion>? ordenes))
                    {
                        return new ObservableCollection<OrdenProduccion>(ordenes);
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

        public async Task<ObservableCollection<Periodos>> GetAllPeriodosAsync()
        {
            string uri = _uri + "/periodos/getall";

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<Periodos>>>(content, _jsonSerializerSettings);
                    if (!values.IsNullOrEmpty() &&
                        values.TryGetValue("periodos", out ObservableCollection<Periodos>? periodos))
                    {
                        return new ObservableCollection<Periodos>(periodos);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }

        public async Task<ObservableCollection<NotificarPt>> GetAllPt()
        {
            string uri = _uri + "/productos/getall";

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<NotificarPt>>>(content, _jsonSerializerSettings);
                    if (!values.IsNullOrEmpty() &&
                        values.TryGetValue("notificarPt", out ObservableCollection<NotificarPt>? productos))
                    {
                        return new ObservableCollection<NotificarPt>(productos);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }

        public async Task<ObservableCollection<NotificarEm>> GetAllEm()
        {
            string uri = _uri + "/materiales/getall";

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<NotificarEm>>>(content, _jsonSerializerSettings);
                    if (!values.IsNullOrEmpty() &&
                        values.TryGetValue("notificarMaterial", out ObservableCollection<NotificarEm>? materiales))
                    {
                        return new ObservableCollection<NotificarEm>(materiales);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }


        public async Task<ObservableCollection<EmpleadoSistema>> GetAllEmpleados()
        {
            string uri = _uri + "/empleados/getall";

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<EmpleadoSistema>>>(content, _jsonSerializerSettings);
                    if (!values.IsNullOrEmpty() &&
                        values.TryGetValue("empleados", out ObservableCollection<EmpleadoSistema>? empleados))
                    {
                        return new ObservableCollection<EmpleadoSistema>(empleados);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }

    }
}
