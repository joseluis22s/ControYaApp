using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ControYaApp.Services.WebService
{
    public class RestService
    {
        private IpServidorRepo _ipServidorRepo;
        private readonly HttpClient _client = new();

        private JsonSerializerOptions _jsonSerializerOptions;

        public RestService(IpServidorRepo ipServidorRepo)
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            _ipServidorRepo = ipServidorRepo;
        }


        public async Task<Dictionary<string, object>> CheckUsuarioCredentialsAsync(Usuario usuario)
        {
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/usuarios/loginusuario";
            try
            {
                var usuarioLogin = new
                {
                    NombreUsuario = usuario.NombreUsuario,
                    UsuarioSistema = "",
                    Contrasena = usuario.Contrasena
                };
                string json = JsonSerializer.Serialize(usuarioLogin, _jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, object>>(resContent);

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
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/usuarios/getall";

            try
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<Usuario>>>(resContent, _jsonSerializerOptions);
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
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/ordenes/getall";

            try
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<OrdenProduccion>>>(resContent, _jsonSerializerOptions);
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
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/periodos/getrangos";

            Periodos periodos = new Periodos();
            try
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(resContent, _jsonSerializerOptions);
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

        public async Task<ObservableCollection<NotificarPt>> GetAllProductosTerminado()
        {
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/productos/getall";

            try
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<NotificarPt>>>(resContent, _jsonSerializerOptions);
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

        public async Task<ObservableCollection<NotificarEm>> GetAllMaterialesEgreso()
        {
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/materiales/getall";

            try
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<NotificarEm>>>(resContent, _jsonSerializerOptions);
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
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/empleados/getall";

            try
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<EmpleadoSistema>>>(resContent, _jsonSerializerOptions);
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

        public async Task NotificarProductoTerminadoAsync(PtNotificadoReq producto)
        {
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            string uri = ip.Protocolo + ip.Ip + "/productos/sp-notificarpt";
            try
            {
                string json = JsonSerializer.Serialize(producto, _jsonSerializerOptions);
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
