﻿using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using CbMovil.Models;
using ControYaApp.Models;
using ControYaApp.Services.SharedData;

namespace ControYaApp.Services.WebService
{
    public class RestService
    {
        private readonly string _loginUsuarioUri = "/usuarios/login-usuario";

        private readonly string _getAllUsuariosUri = "/usuarios/get-all";

        private readonly string _getRangosPeriodosUri = "/periodos/get-rangos";

        private readonly string _getAllEmpleadosUri = "/empleados/get-all";

        private readonly string _getAllOrdenesProduccionUri = "/ordenes-produccion/get-all";

        private readonly string _getAllOrdenesProduccionPtUri = "/ordenes-produccion/get-all-pt";

        private readonly string _getAllOrdenesProduccionMpUri = "/ordenes-produccion/get-all-mp";

        private readonly string _getUnAprrovedPtPrdInv = "/notificados/get-unapproved-pt";

        private readonly string _getUnAprrovedMpPrdInv = "/notificados/get-unapproved-mp";

        private readonly string _processNotified = "/notificados/process-ptmp-notificados";

        private readonly string _getAllLotes = "/lotes/get-all";

        private readonly string _saveAllNewLotes = "/lotes/save-all-new";

        private readonly HttpClient _client = new();

        private JsonSerializerOptions _jsonSerializerOptions;


        public ISharedData SharedData { get; set; }



        public RestService(ISharedData sharedData)
        {
            SharedData = sharedData; // Debe ir primero, no mover.

            _jsonSerializerOptions = SettingJsonSerializerOptions();
        }



        private JsonSerializerOptions SettingJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private string GetIp()
        {
            return SharedData.Protocolo + SharedData.IpAddress;
        }


        public async Task<Usuario> CheckUsuarioCredentialsAsync(Usuario usuarioReq)
        {
            string uri = GetIp() + _loginUsuarioUri;

            usuarioReq.UsuarioSistema = "";

            try
            {
                string json = JsonSerializer.Serialize(usuarioReq, _jsonSerializerOptions);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);
                string resContent = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<Dictionary<string, Usuario>>(resContent, _jsonSerializerOptions);

                if (values != null &&
                    values.TryGetValue("usuario", out Usuario? usuarioRes))

                {
                    return usuarioRes;
                }
                return null;
            }
            catch (Exception) { throw; }
        }

        public async Task<ObservableCollection<Usuario>> GetAllUsuariosAsync()
        {
            string uri = GetIp() + _getAllUsuariosUri;

            try
            {
                StringContent content = new("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<Usuario>>>(resContent, _jsonSerializerOptions);

                    if (values != null &&
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

        public async Task<ObservableCollection<OrdenProduccion>> GetAllOrdenesProduccionAsync(string codigoUsuarioAprobar)
        {
            string uri = GetIp() + _getAllOrdenesProduccionUri + $"?codigoUsuarioAprobar={codigoUsuarioAprobar}";

            try
            {
                string json = JsonSerializer.Serialize("", _jsonSerializerOptions);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<OrdenProduccion>>>(resContent, _jsonSerializerOptions);

                    if (values != null &&
                        values.TryGetValue("ordenesProduccion", out ObservableCollection<OrdenProduccion>? ordenesProduccion))
                    {
                        return new ObservableCollection<OrdenProduccion>(ordenesProduccion);
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
            string uri = GetIp() + _getRangosPeriodosUri;

            Periodos periodos = new();

            try
            {
                StringContent content = new("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(resContent, _jsonSerializerOptions);
                    if (values != null &&
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

        public async Task<ObservableCollection<OrdenProduccionPt>> GetAllOrdenesProduccionPtAsync(string codigoUsuarioAprobar)
        {
            string uri = GetIp() + _getAllOrdenesProduccionPtUri + $"?codigoUsuarioAprobar={codigoUsuarioAprobar}";

            try
            {
                string json = JsonSerializer.Serialize("", _jsonSerializerOptions);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<OrdenProduccionPt>>>(resContent, _jsonSerializerOptions);

                    if (values != null &&
                        values.TryGetValue("ordenesProduccionPt", out ObservableCollection<OrdenProduccionPt>? ordenesProduccionPt))
                    {
                        return new ObservableCollection<OrdenProduccionPt>(ordenesProduccionPt);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }

        public async Task<ObservableCollection<OrdenProduccionMp>> GetAllOrdenesProduccionPmAsync(string codigoUsuarioAprobar)
        {
            string uri = GetIp() + _getAllOrdenesProduccionMpUri + $"?codigoUsuarioAprobar={codigoUsuarioAprobar}";

            try
            {
                string json = JsonSerializer.Serialize("", _jsonSerializerOptions);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<OrdenProduccionMp>>>(resContent, _jsonSerializerOptions);

                    if (values != null &&
                        values.TryGetValue("ordenesProduccionMp", out ObservableCollection<OrdenProduccionMp>? ordenesProduccionMp))
                    {
                        return new ObservableCollection<OrdenProduccionMp>(ordenesProduccionMp);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }

        public async Task<ObservableCollection<EmpleadoSistema>> GetAllEmpleadosAsync()
        {
            string uri = GetIp() + _getAllEmpleadosUri;

            try
            {
                StringContent content = new("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<EmpleadoSistema>>>(resContent, _jsonSerializerOptions);

                    if (values != null &&
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

        public async Task<List<PtNotificado>> GetUnapproveddPtPrdInv(string codigoUsuarioSistema)
        {
            string uri = GetIp() + _getUnAprrovedPtPrdInv + $"?codigoUsuarioSistema={codigoUsuarioSistema}";
            List<PtNotificado> items = [];
            try
            {
                StringContent content = new("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    items = JsonSerializer.Deserialize<List<PtNotificado>>(resContent, _jsonSerializerOptions);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return items;
        }


        public async Task<List<MpNotificado>> GetUnapproveddMpPrdInv(string codigoUsuarioSistema)
        {
            string uri = GetIp() + _getUnAprrovedMpPrdInv + $"?codigoUsuarioSistema={codigoUsuarioSistema}";
            List<MpNotificado> items = [];
            try
            {
                StringContent content = new("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    items = JsonSerializer.Deserialize<List<MpNotificado>>(resContent, _jsonSerializerOptions);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return items;
        }


        public async Task<bool> ProcessPtMpNotificados(object ptMpNotificados)
        {// TODO: control cuano ambos etsan vacios
            string uri = GetIp() + _processNotified;

            try
            {
                string json = JsonSerializer.Serialize(ptMpNotificados, _jsonSerializerOptions);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync(uri, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Lote>> GetAllLotesAsync()
        {
            string uri = GetIp() + _getAllLotes;
            try
            {
                StringContent content = new("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<Lote>>>(resContent, _jsonSerializerOptions);
                    if (values != null &&
                        values.TryGetValue("lotes", out ObservableCollection<Lote>? lotes))
                    {
                        return new List<Lote>(lotes);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return [];
        }

        public async Task SaveAllNewLoteAsync(object lotes)
        {
            string uri = GetIp() + _saveAllNewLotes;
            try
            {
                string json = JsonSerializer.Serialize(lotes, _jsonSerializerOptions);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
