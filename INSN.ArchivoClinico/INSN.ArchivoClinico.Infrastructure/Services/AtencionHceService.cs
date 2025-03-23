using INSN.ArchivoClinico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using INSN.ArchivoClinico.Domain.UtilFactory.Base;
using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Domain.Models;
using INSN.ArchivoClinico.Application.DTOs;
using Newtonsoft.Json.Linq;
using System.Drawing;
using INSN.ArchivoClinico.Domain.Entities;
//using INSN.ArchivoClinico.Application.DTOs;

namespace INSN.ArchivoClinico.Infrastructure.Services
{
    public class AtencionHceService : IAtencionHceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AtencionHceService> _logger;

        public AtencionHceService(HttpClient httpClient, IConfiguration configuration, ILogger<AtencionHceService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SincronizarAtencionesAsignadasAsync(string token, string usuario)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/SincronizarAtencionesAuditoria?usuario={Uri.EscapeDataString(usuario)}";

            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al sincronizar la auditoría. Código de estado: {StatusCode}", response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST para actualizar el ítem de auditoría.");
                throw;
            }
        }

        public async Task<bool> SincronizarEvaluacionesAsignadasAsync(string token, string usuario)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/SincronizarEvaluacionesAuditoria?usuario={Uri.EscapeDataString(usuario)}";

            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al sincronizar la auditoría. Código de estado: {StatusCode}", response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST para actualizar el ítem de auditoría.");
                throw;
            }
        }


        public async Task<bool> ActualizarAuditCuentaAtencionAsync(TriajeResponseDto TriajeResponseDto, string Token)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/ActualizarAuditCuentaAtencion";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(TriajeResponseDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al guardar el ítem de auditoría. Código de estado: {StatusCode}", response.StatusCode);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST para actualizar el ítem de auditoría.");
                throw;
            }
        }

        public async Task<AtencionModel> ObtenerRegistrosTriajeSinCuenta(int id, string token)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];

            var atencionUrl = $"{baseUrl}/GetAtencionById?id={id}";
            var diagnosticosUrl = $"{baseUrl}/GetDiagnosticosByAtencionId?atencionId={id}";
            var medicamentosUrl = $"{baseUrl}/GetMedicamentosByAtencionId?atencionId={id}";
            var examenesUrl = $"{baseUrl}/GetExamenesAuxiliaresByAtencionId?atencionId={id}";
            var procedimientosUrl = $"{baseUrl}/GetExamenesProcedimientosByAtencionId?atencionId={id}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var atencionTask = _httpClient.GetAsync(atencionUrl);
                var diagnosticosTask = _httpClient.GetAsync(diagnosticosUrl);
                var medicamentosTask = _httpClient.GetAsync(medicamentosUrl);
                var examenesTask = _httpClient.GetAsync(examenesUrl);
                var procedimientosTask = _httpClient.GetAsync(procedimientosUrl);

                await Task.WhenAll(atencionTask, diagnosticosTask, medicamentosTask, examenesTask, procedimientosTask);

                var atencionResponse = await atencionTask;
                if (!atencionResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener la atención. Código de estado: {StatusCode}", atencionResponse.StatusCode);
                    return null;
                }

                var atencionJson = await atencionResponse.Content.ReadAsStringAsync();
                var atencionResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<AtencionModel>>(atencionJson);
                var atencion = atencionResponseDeserialize?.Data;

                if (atencion == null)
                {
                    return null;
                }

                // Procesar las respuestas para diagnósticos, medicamentos, exámenes y procedimientos
                var diagnosticosResponse = await diagnosticosTask;
                if (diagnosticosResponse.IsSuccessStatusCode)
                {
                    var diagnosticosJson = await diagnosticosResponse.Content.ReadAsStringAsync();
                    var diagnosticosResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<DiagnosticoModel>>>(diagnosticosJson);
                    atencion.Diagnosticos = diagnosticosResponseDeserialize.Data;
                }

                var medicamentosResponse = await medicamentosTask;
                if (medicamentosResponse.IsSuccessStatusCode)
                {
                    var medicamentosJson = await medicamentosResponse.Content.ReadAsStringAsync();
                    var medicamentosResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<Domain.Models.MedicamentoModel>>>(medicamentosJson);
                    atencion.Medicamentos = medicamentosResponseDeserialize.Data;
                }

                var examenesResponse = await examenesTask;
                if (examenesResponse.IsSuccessStatusCode)
                {
                    var examenesJson = await examenesResponse.Content.ReadAsStringAsync();
                    var examenesResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<ExamenAuxiliarModel>>>(examenesJson);
                    atencion.ExamenesAuxiliares = examenesResponseDeserialize.Data;
                }

                var procedimientosResponse = await procedimientosTask;
                if (procedimientosResponse.IsSuccessStatusCode)
                {
                    var procedimientosJson = await procedimientosResponse.Content.ReadAsStringAsync();
                    var procedimientosResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<ExamenProcedimientoModel>>>(procedimientosJson);
                    atencion.ExamenesProcedimientos = procedimientosResponseDeserialize.Data;
                }

                return atencion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST.");
                throw;
            }
        }


        public async Task<PagedResponse<AtencionHceModel>> ObtenerAtencionesAsync(string ModuloAuditoria, int tipoConsulta, DateTime fecha, string historiaClinica, string nombre, int page, int pageSize, string token)
        {
            var apUrl = _configuration["ApiUrls:Auditoria"];  
            var url = $"{apUrl}/GetAtencionesCEByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";

            if (ModuloAuditoria == "T")
            {
                switch (tipoConsulta)
                {
                    case 1: //CE
                        url = $"{apUrl}/GetAtencionesCEByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;

                    case 2: //EMER
                        url = $"{apUrl}/GetAtencionesEmrByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;

                    case 3: //HOSP
                        url = $"{apUrl}/GetAtencionesCEByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;

                    default:
                        url = $"{apUrl}/GetAtencionesEmrByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;
                }
            }
            else {
                switch (tipoConsulta)
                {
                    case 1: //CE
                        url = $"{apUrl}/GetAtencionesCEByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;

                    case 2: //EMER
                        url = $"{apUrl}/GetAtencionesCEByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;

                    case 3: //HOSP
                        url = $"{apUrl}/GetAtencionesCEByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;

                    default:
                        url = $"{apUrl}/GetAtencionesCEByFecha?fecha={fecha:yyyy-MM-dd}&historiaClinica={historiaClinica}&nombre={nombre}&pageNumber={page}&pageSize={pageSize}";
                        break;
                }
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paginatedResponse = JsonConvert.DeserializeObject<PagedResponse<AtencionHceModel>>(jsonResponse);
                    return paginatedResponse;
                }
                else
                {
                    _logger.LogError("Error al obtener los datos del servicio REST. Código de estado: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST.");
                throw;
            }
        }

        public async Task<AtencionModel> ObtenerDetalleAtencionAsync(int id, string token)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];

            var atencionUrl = $"{baseUrl}/GetAtencionById?id={id}";
            var diagnosticosUrl = $"{baseUrl}/GetDiagnosticosByAtencionId?atencionId={id}";
            var medicamentosUrl = $"{baseUrl}/GetMedicamentosByAtencionId?atencionId={id}";
            var examenesUrl = $"{baseUrl}/GetExamenesAuxiliaresByAtencionId?atencionId={id}";
            var procedimientosUrl = $"{baseUrl}/GetExamenesProcedimientosByAtencionId?atencionId={id}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var atencionTask = _httpClient.GetAsync(atencionUrl);
                var diagnosticosTask = _httpClient.GetAsync(diagnosticosUrl);
                var medicamentosTask = _httpClient.GetAsync(medicamentosUrl);
                var examenesTask = _httpClient.GetAsync(examenesUrl);
                var procedimientosTask = _httpClient.GetAsync(procedimientosUrl);

                await Task.WhenAll(atencionTask, diagnosticosTask, medicamentosTask, examenesTask, procedimientosTask);

                var atencionResponse = await atencionTask;
                if (!atencionResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener la atención. Código de estado: {StatusCode}", atencionResponse.StatusCode);
                    return null;
                }

                var atencionJson = await atencionResponse.Content.ReadAsStringAsync();
                var atencionResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<AtencionModel>>(atencionJson);
                var atencion = atencionResponseDeserialize?.Data;

                if (atencion == null)
                {
                    return null;
                }

                // Procesar las respuestas para diagnósticos, medicamentos, exámenes y procedimientos
                var diagnosticosResponse = await diagnosticosTask;
                if (diagnosticosResponse.IsSuccessStatusCode)
                {
                    var diagnosticosJson = await diagnosticosResponse.Content.ReadAsStringAsync();
                    var diagnosticosResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<DiagnosticoModel>>>(diagnosticosJson);
                    atencion.Diagnosticos = diagnosticosResponseDeserialize.Data;
                }

                var medicamentosResponse = await medicamentosTask;
                if (medicamentosResponse.IsSuccessStatusCode)
                {
                    var medicamentosJson = await medicamentosResponse.Content.ReadAsStringAsync();
                    var medicamentosResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<Domain.Models.MedicamentoModel>>>(medicamentosJson);
                    atencion.Medicamentos = medicamentosResponseDeserialize.Data;
                }

                var examenesResponse = await examenesTask;
                if (examenesResponse.IsSuccessStatusCode)
                {
                    var examenesJson = await examenesResponse.Content.ReadAsStringAsync();
                    var examenesResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<ExamenAuxiliarModel>>>(examenesJson);
                    atencion.ExamenesAuxiliares = examenesResponseDeserialize.Data;
                }

                var procedimientosResponse = await procedimientosTask;
                if (procedimientosResponse.IsSuccessStatusCode)
                {
                    var procedimientosJson = await procedimientosResponse.Content.ReadAsStringAsync();
                    var procedimientosResponseDeserialize = JsonConvert.DeserializeObject<StatusResponse<IEnumerable<ExamenProcedimientoModel>>>(procedimientosJson);
                    atencion.ExamenesProcedimientos = procedimientosResponseDeserialize.Data;
                }

                return atencion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST.");
                throw;
            }
        }

        public async Task<ItemAuditoriaDto> ObtenerDetalleItemAuditoriaAsync(int id, string puntoCarga, string token)
        {
            if (id <= 0 || string.IsNullOrEmpty(puntoCarga))
            {
                _logger.LogWarning("Parámetros inválidos para la consulta del ítem de auditoría.");
                return null;
            }

            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var itemAuditoriaUrl = $"{baseUrl}/GetItemAuditoriaById?id={id}&puntoCarga={puntoCarga}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var itemAuditoriaResponse = await _httpClient.GetAsync(itemAuditoriaUrl);

                if (!itemAuditoriaResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener el ítem de auditoría. Código de estado: {StatusCode}", itemAuditoriaResponse.StatusCode);
                    return null;
                }

                //var itemAuditoriaJson = await itemAuditoriaResponse.Content.ReadAsStringAsync();
                //var itemAuditoriaDeserialize = JsonConvert.DeserializeObject<StatusResponse<ItemAuditoriaDto>>(itemAuditoriaJson);
                //var itemAuditoria = itemAuditoriaDeserialize?.Data;

                //if (itemAuditoria == null)
                //{
                //    _logger.LogError("Error al deserializar el JSON a objeto ItemAuditoriaDto.");
                //}

                return null;
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Error al deserializar el JSON del servicio REST.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST.");
                throw;
            }
        }

        public async Task<ExamenAuxiliarDto> ObtenerDetalleExamenAuxiliarAsync(int id, string token)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var examenAuxiliarUrl = $"{baseUrl}/GetExamenAuxiliarById?idExamen={id}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync(examenAuxiliarUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener el examen auxiliar. Código de estado: {StatusCode}", response.StatusCode);
                    return null;
                }

                var examenAuxiliarJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExamenAuxiliarDto>(examenAuxiliarJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST para obtener el examen auxiliar.");
                throw;
            }
        }

        public async Task<bool> ActualizarAtencionAuditoriaAsync(AtencionCEAuditoriaDto atencionCEAuditoriaDto)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/ActualizarAuditoria";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(atencionCEAuditoriaDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", atencionCEAuditoriaDto.Token);
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al guardar la atención de auditoría. Código de estado: {StatusCode}", response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST para actualizar la atención de auditoría.");
                throw;
            }
        }

        public async Task<bool> ActualizarItemAuditoriaAsync(ItemAuditoriaDto itemAuditoriaDto)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/ActualizarAuditoriaItem";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(itemAuditoriaDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", itemAuditoriaDto.Token);
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al guardar el ítem de auditoría. Código de estado: {StatusCode}", response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio REST para actualizar el ítem de auditoría.");
                throw;
            }
        }
               
    }
}
