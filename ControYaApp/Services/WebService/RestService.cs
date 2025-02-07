using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ControYaApp.Services.WebService
{
    public class RestService
    {
        private IpServidorRepo _ipServidorRepo;
        private readonly HttpClient _client = new();

        private JsonSerializerSettings _jsonSerializerSettings;

        public RestService(IpServidorRepo ipServidorRepo)
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            _ipServidorRepo = ipServidorRepo;
        }


        public async Task<Dictionary<string, object>> CheckUsuarioCredentialsAsync(Usuario usuario)
        {
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/usuarios/loginusuario";
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
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            { "estaRegistrado", false },
                            { "usuarioSistema", "Error" }
                        };
                    }

                }
                else
                {
                    return new Dictionary<string, object>
                        {
                            { "estaRegistrado", false },
                            { "usuarioSistema", response.StatusCode }
                        };
                }
            }
            catch (Exception ex)
            {

                return new Dictionary<string, object>
                        {
                            { "estaRegistrado", false },
                            { "usuarioSistema", ex.Message }
                        };
            }
        }

        public async Task<ObservableCollection<Usuario>> GetAllUsuariosAsync()
        {
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/usuarios/getall";

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
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/ordenes/getall";

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


        public async Task<Periodos> GetRangosPeriodos()
        {
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/periodos/getrangos";

            Periodos periodos = new Periodos();
            try
            {

                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(content, _jsonSerializerSettings);
                    if (!values.IsNullOrEmpty() &&
                        values.TryGetValue("fechaMinima", out DateTime fechaMinima) &&
                        values.TryGetValue("fechaMaxima", out DateTime fechaMaxima))
                    {
                        periodos.FechaMax = fechaMaxima;
                        periodos.FechaMin = fechaMinima;
                        return periodos;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return periodos;
        }

        public async Task<ObservableCollection<NotificarPt>> GetAllPt()
        {
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/productos/getall";

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
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/materiales/getall";

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
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/empleados/getall";

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

        public async Task NotificarPtAsync(PtNotificadoReq producto)
        {
            string uri = await _ipServidorRepo.GetIpServidorAsync();
            uri = uri + "/productos/sp-notificarpt";
            try
            {
                string json = JsonConvert.SerializeObject(producto, _jsonSerializerSettings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Procedimiento ejecutado");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }


    }
}
