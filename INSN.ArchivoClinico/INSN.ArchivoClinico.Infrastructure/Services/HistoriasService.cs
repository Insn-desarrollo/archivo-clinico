using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Interfaces;
using INSN.ArchivoClinico.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using INSN.ArchivoClinico.Domain.UtilFactory.Base;
using Newtonsoft.Json;
using System;
using System.Reflection.Metadata;

namespace INSN.ArchivoClinico.Infrastructure.Services
{
    public class HistoriasService : IHistoriasService
    {
        private readonly IHistoriasRepository _atencionRepository;
        private readonly ICuentaRepository _cuentaRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HistoriasService(IHistoriasRepository atencionRepository, ICuentaRepository cuentaRepository, HttpClient httpClient, IConfiguration configuration)
        {
            _atencionRepository = atencionRepository;
            _cuentaRepository = cuentaRepository;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<ContadorPendientes>> ObtenerContadoresBandejaAsync()
        {
            var atenciones = await _atencionRepository.ObtenerContadoresBandejaAsync();
            return atenciones;
        }

        public async Task<IEnumerable<HistoriaClinicaDto>> ConsultarHistoriasAsync(HistoriaFiltro filtro)
        {
            var atenciones = await _atencionRepository.ConsultarHistoriasAsync(filtro);
            return atenciones;        
        }

        public async Task<HistoriaClinicaConsultaDto> ConsultarPacienteAsync(string historia)
        {
            var auditoriaUrl = _configuration["ApiUrls:Auditoria"];
            var emergenciaUrl = _configuration["ApiUrls:Emergencia"];
            var urlPdf = "";
            var paciente = await _atencionRepository.ConsultarPacienteAsync(historia);
            var atenciones = await _atencionRepository.ConsultarAtencionesEmergenciaAsync(historia);            
            foreach (var atencion in atenciones)
            {
                var evaluaciones = await _atencionRepository.ConsultarEvaluacionesEmergenciaAsync(atencion.atencion_id);
                var documentosA = await _atencionRepository.ConsultaDocumentoAtencion(atencion.atencion_id);
                foreach (var item in documentosA)
                {
                    if (item.tipo_documento == "Formato Fua" && item.documento == null)
                        urlPdf = $"{auditoriaUrl}/Fua/reporte?idAtencion={item.atencion_id}";

                    if (item.tipo_documento == "Papeleta Egreso" && item.documento == null)
                        urlPdf = $"{emergenciaUrl}/egresos/pdf?atencionId={item.atencion_id}";

                    item.documento = (item.documento == null) ? urlPdf : item.documento;
                }
                atencion.documentosAtencion = documentosA;

                foreach (var evaluacion in evaluaciones)
                {
                    var documentos = await _atencionRepository.ConsultaDocumentoEvaluacion(evaluacion.evaluacion_id);
                    
                    foreach (var item in documentos)
                    {
                        if (item.tipo_documento == "Historia Clinica" && item.documento == null)
                            urlPdf = $"{emergenciaUrl}/evaluaciones/pdf?evaluacionId={item.evaluacion_id}";

                        if (item.tipo_documento == "Orden" && item.documento == null)
                            urlPdf = $"{emergenciaUrl}/evaluaciones/ordenes?evaluacionId={item.evaluacion_id}";

                        if (item.tipo_documento == "Receta" && item.documento == null)
                            urlPdf = $"{emergenciaUrl}/evaluaciones/receta?evaluacionId={item.evaluacion_id}";

                        item.documento = (item.documento == null) ? urlPdf : item.documento;
                    }
                    evaluacion.documentos = documentos;
                }
                atencion.evaluaciones = evaluaciones;
            }
            paciente.atenciones = atenciones;
            return paciente;
        }

        public async Task<IEnumerable<AtencionHcDto>> ConsultarAtencionesEmergenciaAsync(string historia)
        {
            var atenciones = await _atencionRepository.ConsultarAtencionesEmergenciaAsync(historia);
            return atenciones;
        }

        public async Task<IEnumerable<EvaluacionHcDto>> ConsultarEvaluacionesEmergenciaAsync(int atencion_id)
        {
            var evaluaciones = await _atencionRepository.ConsultarEvaluacionesEmergenciaAsync(atencion_id);
            return evaluaciones;
        }


        public async  Task<AtencionDto> GetAtencionByIdAsync(string token, int atencionId)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/atencion/atencionById?idAtencion={atencionId.ToString()}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al obtener la atencion. Código de estado: {response.StatusCode}");
                    return new AtencionDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var atencionResponse = JsonConvert.DeserializeObject<ApiResponse<AtencionDto>>(jsonResponse);

                return atencionResponse.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para obtener la atencion: {ex.Message}");
                return new AtencionDto();
            }
        }

        public async Task<AtencionDto> GetAtencionOrdenesByIdAsync(string token, int atencionId)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/evaluacion/por-atencion?idAtencion={atencionId.ToString()}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al obtener la atencion. Código de estado: {response.StatusCode}");
                    return new AtencionDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var atencionResponse = JsonConvert.DeserializeObject<ApiResponse<AtencionDto>>(jsonResponse);

                return atencionResponse.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar el ítem de auditoría: {ex.Message}");
                return new AtencionDto();
            }
        }

        public async Task<AtencionDto> GetAtencionCuentasByIdAsync(string token, int atencionId)
        {
            var baseUrl = _configuration["ApiUrls:Auditoria"];
            var apiUrl = $"{baseUrl}/evaluacion/por-atencion-con-resultado?idAtencion={atencionId.ToString()}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al obtener la atencion. Código de estado: {response.StatusCode}");
                    return new AtencionDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var atencionResponse = JsonConvert.DeserializeObject<ApiResponse<AtencionDto>>(jsonResponse);

                return atencionResponse.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al comunicarse con el servicio REST para actualizar el ítem de auditoría: {ex.Message}");
                return new AtencionDto();
            }
        }


        public async Task<List<AuditorDto>> ObtenerListaUsuariosAsync()
        {
            return await _atencionRepository.ObtenerListaUsuariosAsync();
        }

        public async Task<TriajeResponseDto> ActualizarTriaje(ActualizarTriajeRequest request)
        {
            return await _atencionRepository.ActualizarTriajeAsync(request);
        }     

        public async Task<bool> AsignarAtencionesAsync()
        {
            return await _atencionRepository.AsignarAtencionesAsync();
        }

        public async Task<List<Atencion>> GetAtencionAsync(int idAtencion)
        {
            // Simulación de carga de datos de un objeto de tipo Atencion
            var atencion_uno = new Atencion
            {
                idAtencion = 999,
                loteFua = "24",
                nroFua = "00000001",
                renipress = "00000001",
                idCategoria = "01",
                nivel = "01",
                idPuntoDigitacion = 1,
                idComponente = "1",
                idDisaAsegurado = "001",
                idLoteAsegurado = "02",
                idCorrelativoAsegurado = "000000001",
                idSecuenciaAsegurado = "02",
                idTablaAsegurado = "1",
                idContratoAsegurado = "0000000010",
                idPlan = "02",
                idGrupoPoblacional = "01",
                idTipoDocAsegurado = "1",
                numDocAsegurado = "000000001",
                apePaterno = "Eche",
                apeMaterno = "Eche",
                nombres = "Jenniffer",
                fecNac = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                idSexo = "1",
                idUbigeo = "140101",
                historiaClinica = "00000000000000000020",
                idTipoAtencion = "1",
                idCondicionMaterna = "1",
                idModalidadAtencion = "1",
                nroAutorizacion = "000000000000015",
                montoAutorizado = 1,
                fecHoraAtencion = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                renipressReferencia = "00000001",
                nroHojaReferencia = "001",
                idServicio = "01",
                idOrigenPersonal = "1",
                idLugarAtencion = "1",
                idDestinoAsegurado = "2",
                fecIngresoHospitalizacion = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                fecAltaHospitalizacion = DateTime.Parse("2024-11-01T20:19:32.824Z"),
                renipressContraReferencia = "00000002",
                nroHojaContraReferencia = "002",
                fecParto = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                idGrupoRiesgo = "1",
                fecFallecimiento = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                renipressOfertaFlexible = "001",
                idEtnia = "1",
                idIafas = "2",
                idCodigoIafas = "001",
                idUps = "1",
                fecCorteAdministrativo = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                idUdrAutorizaVinculado = "000",
                loteAutorizaVinculado = "02",
                nroAutorizaVinculado = "003",
                disaFuaVinculado = "001",
                loteFuaVinculado = "03",
                nroFuaVinculado = "004",
                idTipoDocRespAte = "02",
                numDocRespAte = "000000002",
                idTipoPersonalSalud = "03",
                idEspecialidadRespAte = "04",
                esEgresadoRespAte = "1",
                colegiaturaRespAte = "123456",
                rneRespAte = "987654",
                idTipoDocDigitador = "1",
                numDocDigitador = "000000003",
                fecHoraRegistro = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                observacion = "Observación de prueba INSN",
                versionAplicativo = "1.0.0",
                codigoAcreditacion = "12345",
                fecHoraIniFuaAdm = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                fecHoraFinFuaAdm = DateTime.Parse("2024-10-28T20:19:32.824Z"),
                idMotivoIngresoCasaMaterna = 1,
                //idCasaMaterna = "02",
                //fecHoraCrea = DateTime.Parse("2024-10-27T20:19:32.824Z"),
                //idEstado = "R",
                atDiagnosticos = new List<AtDiagnostico>
        {
            new AtDiagnostico
            {
                codigo = "D001",
                nroDiagnostico = 1,
                tipoMovimiento = "1",
                tipoDiagnostico = 2
            },
            new AtDiagnostico
            {
                codigo = "D002",
                nroDiagnostico = 2,
                tipoMovimiento = "1",
                tipoDiagnostico = 3
            }
        },
                atInsumos = new List<AtInsumo>
        {
            new AtInsumo
            {
                codigo = "I001",
                nroDiagnostico = 1,
                cantPrescrita = 10,
                cantEntregada = 8,
                lote = "L12345",
                nroSerie = "NS67890",
                registroSanitario = "RS001",
                fecVencimiento = DateTime.Now.AddMonths(6).ToString("yyyy-MM-dd"),
                contieneOtrosDatos = ""
            }
        },
                atMedicamentos = new List<AtMedicamento>
        {
            new AtMedicamento
            {
                codigo = "M001",
                nroDiagnostico = 1,
                cantPrescrita = 5,
                cantEntregada = 5,
                fecPetitorio = DateTime.Now,
                nroDocPetitorio = "PET123",
                lote = "L12345",
                nroSerie = "NS12345",
                registroSanitario = "RS002",
                fecVencimiento = DateTime.Now.AddMonths(12).ToString("yyyy-MM-dd"),
                contieneOtrosDatos = ""
            }
        },
                atProcedimientos = new List<AtProcedimiento>
        {
            new AtProcedimiento
            {
                codigo = "P001",
                nroDiagnostico = 1,
                cantPrescrita = 1,
                cantEntregada = 1,
                resultado = "Normal"
            }
        },
                atRecienNacidos = new List<AtRecienNacido>
        {
            new AtRecienNacido
            {
                nroRN = 1,
                tipoDocumento = "1",
                nroDocumento = "12345678",
                disaAfiliacion = "123",
                formatoContratoAfil = "01",
                nroContratoAfil = "NC123",
                secuenciaContAfil = "01",
                apePaterno = "Perez",
                apeMaterno = "Lopez",
                primerNombre = "Juan",
                segundoNombre = "Carlos",
                identificadorRegAfil = 1,
                identificadorTabla = "T"
            }
        },
                atServAdicionales = new List<AtServAdicionales>
        {
            new AtServAdicionales
            {
                codigo = "12"
            }
        }


            };

            return new List<Atencion> { atencion_uno };
        }
                        
    }
       

}
