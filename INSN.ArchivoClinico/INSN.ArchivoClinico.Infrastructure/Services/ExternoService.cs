using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using INSN.ArchivoClinico.Domain.Models;
using System.Text.Json;
using INSN.ArchivoClinico.Domain.Interfaces;

namespace INSN.ArchivoClinico.Infrastructure.Services
{
    public class ExternoService : IExternoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IEvaluacionRepository _evaluacionRepository;

      
        public ExternoService(HttpClient httpClient, IConfiguration configuration, IEvaluacionRepository evaluacionRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _evaluacionRepository = evaluacionRepository;
        }

        public async Task<bool> SincronizarAtencionesSinCuentaAsync(string token)
        {
            var baseUrl = _configuration["ApiUrls:Emergencia"];
            var apiUrl = $"{baseUrl}/triajes/sincronizar_atenciones_sin_cuenta";

            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al sincronizar la auditoría. Código de estado: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar las atenciones de auditoría: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SincronizarEvaluacionesPorIdAsync(string token, int atencionId)
        {
            var baseUrl = _configuration["ApiUrls:Emergencia"];
            var apiUrl = $"{baseUrl}/evaluaciones/sincronizar_evaluacion_byid?atencionId={atencionId}";

            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al sincronizar la auditoría. Código de estado: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar el ítem de auditoría: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarAuditoriaEmergenciaAsync(string token, EstadoAuditoriaModel model)
        {
            var baseUrl = _configuration["ApiUrls:Emergencia"];
            var apiUrl = $"{baseUrl}/triajes/updateEstadoAuditoria";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al actualizar la auditoría. Código de estado: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar el ítem de auditoría: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarAuditoriaEmergenciaItemAsync(string token, ActualizarAuditoriaItemRequest model)
        {
            var baseUrl = _configuration["ApiUrls:Emergencia"];
            var apiUrl = $"{baseUrl}/evaluaciones/actualiza_estado_auditoria_item";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al actualizar la auditoría. Código de estado: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar el ítem de auditoría: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> ActualizarMasivoAuditoriaEmergenciaItemAsync(string token, AceptarMasivaEvaluacionRequest itemAuditoriaDto)
        {
            var baseUrl = _configuration["ApiUrls:Emergencia"];
            var apiUrl = $"{baseUrl}/evaluaciones/auditoria/examenes-medicamentos/estado-masivo";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(itemAuditoriaDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al actualizar masivamente la auditoría. Código de estado: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar el ítem de auditoría: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> RegistrarRecetaOrden_ApoyoDx_Async(string token, EnviaOrdenRequest itemAuditoriaDto)
        {
            var baseUrl = _configuration["ApiUrls:Emergencia"];
            var apiUrl = $"{baseUrl}/evaluaciones/auditoria/apoyo-diagnostico/enviar-ordenes-recetas";

            try
            {

                var jsonContent = JsonConvert.SerializeObject(itemAuditoriaDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al obener los resultados desde auditoría. Código de estado: {response.StatusCode}");
                    return false;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<dynamic>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null)
                    {
                        if (apiResponse.Success)
                            return true;
                        else
                        {
                            Console.WriteLine($"Error al comunicarse con el servicio REST para obtener resultados desde auditoría: {apiResponse.Message}");
                            return false;
                        }
                    }                        
                    else
                    {
                        Console.WriteLine($"Error al comunicarse con el servicio REST para obtener resultados desde auditoría: {apiResponse.Message}");
                        return false;
                    }

                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para obtener resultados resultados desde auditoría: {ex.Message}");
                return false;
            }
        }

        public async Task<AfiliadoSISDto> BuscarAfiliadoSISAsync(string s_tipobus, string s_numero)
        {
            var s_usuario = "MSOBERON";
            var s_modulo = "modcaja";
            var url = $"http://172.30.31.54/econtratosis/swCTSIS.asmx/getsisVB?s_numero={s_numero}&s_tipobus={s_tipobus}&s_usuario={s_usuario}&s_modulo={s_modulo}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var xmlResponse = await response.Content.ReadAsStringAsync();
                    return ParseXmlResponse(xmlResponse);
                }

                throw new Exception($"Error al consumir el servicio: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en BuscarAfiliadoSISAsync: {ex.Message}");
            }
        }

        private AfiliadoSISDto ParseXmlResponse(string xmlResponse)
        {
            try
            { 
                XDocument xmlDoc = XDocument.Parse(xmlResponse);
                var elements = xmlDoc
                    .Descendants(XName.Get("string", "http://tempuri.org/"))
                    .Select(x => x.Value)
                    .ToList();

                if (elements.Count < 30) // Verificar si hay suficientes datos en la respuesta
                {
                    throw new Exception("La respuesta no contiene suficientes datos para mapear el objeto.");
                }

                return new AfiliadoSISDto
                {
                    IdError = elements[0],
                    Resultado = elements[1],
                    TipoDocumento = elements[2],
                    NroDocumento = elements[3],
                    ApePaterno = elements[4],
                    ApeMaterno = elements[5],
                    Nombres = elements[6],
                    FecAfiliacion = elements[7],
                    EESS = elements[8],
                    DescEESS = elements[9],
                    EESSUbigeo = elements[10],
                    DescEESSUbigeo = elements[11],
                    Regimen = elements[12],
                    TipoSeguro = elements[13],
                    DescTipoSeguro = elements[14],
                    Contrato = elements[15],
                    FecCaducidad = elements[16],
                    Estado = elements[17],
                    Tabla = elements[18],
                    IdNumReg = elements[19],
                    Genero = elements[20],
                    FecNacimiento = elements[21],
                    IdUbigeo = elements[22],
                    Direccion = elements[23],
                    Disa = elements[24],
                    TipoFormato = elements[25],
                    NroContrato = elements[26],
                    Correlativo = elements[27],
                    IdPlan = elements[28],
                    IdGrupoPoblacional = elements[29],
                    MsgConfidencial = elements[30]
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mapear el XML al objeto AfiliadoSIS: {ex.Message}");
                return null; // Retornar null si ocurre un error
            }
        }

        public async Task<List<BoletaResultado>> ObtenerBoletaAtencionAsync(string nroHistoria, string nroBoleta, string nombPaciente, string token)
        {
            nroBoleta = nroBoleta == null ? "" : nroBoleta;
            var baseUrl = _configuration["ApiUrls:ApoyoDiagnostico"];
            var apiUrl = $"{baseUrl}/wsCaja/boletaAtencion?nro_historia={nroHistoria}&nro_boleta={nroBoleta}&paciente={nombPaciente}";

            try
            {
                // Configuración del cliente HTTP
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Llamada GET al endpoint
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al obtener la boleta de atención. Código de estado: {response.StatusCode}");
                    return null;
                }

                // Procesar la respuesta y deserializar a BoletaResultado
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var boletaResultado = JsonConvert.DeserializeObject<List<BoletaResultado>>(jsonResponse);

                return boletaResultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para obtener la boleta de atención: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> ActualizarEmergenciaEstadoFuaAsync(string token, ActualizarRespuestaFUARequest model)
        {
            var baseUrl = _configuration["ApiUrls:Emergencia"];
            var apiUrl = $"{baseUrl}/atenciones/auditoria/actualiar-estado-fua";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al actualizar la auditoría. Código de estado: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar el ítem de auditoría: {ex.Message}");
                return false;
            }
        }
    }

}
