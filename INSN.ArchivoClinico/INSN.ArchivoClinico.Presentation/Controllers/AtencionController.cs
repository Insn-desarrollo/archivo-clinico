using Microsoft.AspNetCore.Mvc;
using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Application.DTOs;
using INSN.ArchivoClinico.Domain.Entities;
using System.Text;
using INSN.ArchivoClinico.UtilFactory.Base;
using INSN.ArchivoClinico.Infrastructure.Services;
using Azure.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;


namespace INSN.ArchivoClinico.Controllers
{    
    public class AtencionController : Controller
    {
        private readonly ILogger<AtencionController> _logger;
        private readonly IHistoriasService _historiaService;
        private readonly IEvaluacionService _evaluacionAppService;
        private readonly IFuaEmitidoService _IFuaEmitidoService;
        private readonly ICuentaService _cuentaAppService;
        private readonly IExternoService _externoAppService;
        private readonly AtencionHceService _atencionHceService;
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AtencionController(
            ILogger<AtencionController> logger,
            IHistoriasService historiaService,
            IEvaluacionService evaluacionAppService,
            IFuaEmitidoService iFuaEmitidoService,
            ICuentaService cuentaAppService,
            IExternoService externoAppService,
            HttpClient httpClient,
            IConfiguration configuration,
            AtencionHceService atencionHceService,
            AuthService authService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _historiaService = historiaService;
            _evaluacionAppService = evaluacionAppService;
            _IFuaEmitidoService = iFuaEmitidoService;
            _cuentaAppService = cuentaAppService;
            _externoAppService = externoAppService;
            _atencionHceService = atencionHceService;

            _httpClient = httpClient;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _authService = authService;
        }

        private void ConfigureAuthorizationHeader()
        {
            var token = GetAuthToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

         
        public IActionResult ContenidoModal()
        {
            return PartialView("_Atencion");
        }

         
        public IActionResult CargarTriaje()
        {
            return PartialView("_TriajePartial");
        }

         
        public IActionResult CargarOrdenes()
        {
            return PartialView("_OrdenesPartial");
        }

        private IActionResult RedirectToLoginFromModal()
        {
            return Content("<script>window.top.location.href = '/Account/Login';</script>", "text/html");
        }


        public async Task<IActionResult> Triaje(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                return RedirectToLoginFromModal();
            }

            try
            {
                var atencion = await _historiaService.GetAtencionByIdAsync(token, id);
                if (atencion == null)
                {
                    return NotFound("Atención no encontrada.");
                }

                return View(atencion);
          
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ocurrió un error al cargar los datos: {ex.Message}";
                return View("Error");
            }        
            
        }

         
        public async Task<IActionResult> Atenciones(string id)
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                return RedirectToLoginFromModal();
            }

            try
            {
                var paciente = await _historiaService.ConsultarPacienteAsync(id);
                if (paciente == null)
                {
                    return NotFound("Atención no encontrada.");
                }

                return View(paciente);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ocurrió un error al cargar los datos: {ex.Message}";
                return View("Error");
            }

        }

        public async Task<IActionResult> Evaluaciones(int id)
        {
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var evaluaciones = await _historiaService.ConsultarEvaluacionesEmergenciaAsync(id);
                if (evaluaciones != null)
                {
                    return Json(new { success = true, data = evaluaciones });
                }
                else
                {
                    return Json(new { success = false, message = "Error al obtener la atención." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio de Evaluacion.");
                return Json(new { success = false, message = "Ocurrió un error al comunicarse con el servicio de Evaluacion." });
            }
        }   

        public async Task<IActionResult> Cuenta(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                return RedirectToLoginFromModal();
            }

            try
            {
                var atencion = await _historiaService.GetAtencionCuentasByIdAsync(token, id);
                if (atencion == null)
                {
                    return NotFound("Atención no encontrada.");
                }

                List<EvaluacionCuentaDto> ListEvaluacionCuenta = new List<EvaluacionCuentaDto>();
                List<EvaluacionCuentaDiagnosticoDto> ListEvaluacionCuentaDiagnostico = new List<EvaluacionCuentaDiagnosticoDto>();
                List<EvaluacionCuentaProcedimientoDto> ListEvaluacionCuentaProcedimiento = new List<EvaluacionCuentaProcedimientoDto>();
                List<EvaluacionCuentaMedicamentoDto> ListEvaluacionCuentaMedicamento = new List<EvaluacionCuentaMedicamentoDto>();


                foreach (var eval in atencion.evaluaciones)
                {
                    EvaluacionCuentaDto evaluacionCuenta = new EvaluacionCuentaDto();
                    var evaluacion = await _evaluacionAppService.ObtenerDetalleEvaluacionAsync(eval.evaluacion_eess_id);
                    evaluacionCuenta.evaluacion_id = evaluacion.evaluacion_id;
                    evaluacionCuenta.evaluacion_eess_id = evaluacion.evaluacion_eess_id;
                    evaluacionCuenta.fecha_evaluacion = evaluacion.fecha_evaluacion;
                    evaluacionCuenta.servicio_eess = evaluacion.servicio_eess;
                    evaluacionCuenta.hora_evaluacion = evaluacion.hora_evaluacion;
                    evaluacionCuenta.auditoria_codigo_estado = evaluacion.auditoria_codigo_estado;
                    evaluacionCuenta.auditoria_estado = eval.auditoria_estado;
                    evaluacionCuenta.auditoria_observacion = evaluacion.auditoria_observacion;
                    ListEvaluacionCuenta.Add(evaluacionCuenta);

                    foreach (var diagnostico in evaluacion.Diagnosticos)
                    {
                        EvaluacionCuentaDiagnosticoDto evaluacionCuentaDiagnostico = new EvaluacionCuentaDiagnosticoDto();
                        evaluacionCuentaDiagnostico.fecha_evaluacion = evaluacion.fecha_evaluacion;
                        evaluacionCuentaDiagnostico.hora_evaluacion = evaluacion.hora_evaluacion;
                        evaluacionCuentaDiagnostico.codigo_tipo_diagnostico = diagnostico.codigo_tipo_diagnostico;
                        evaluacionCuentaDiagnostico.tipo_diagnostico = diagnostico.tipo_diagnostico;
                        evaluacionCuentaDiagnostico.codigo_diagnostico = diagnostico.codigo_diagnostico;
                        evaluacionCuentaDiagnostico.diagnostico = diagnostico.diagnostico;
                        evaluacionCuentaDiagnostico.es_principal = diagnostico.es_principal;
                        ListEvaluacionCuentaDiagnostico.Add(evaluacionCuentaDiagnostico);
                    }


                    foreach (var examenAuxiliar in evaluacion.ExamenesAuxiliares)
                    {
                        EvaluacionCuentaProcedimientoDto evaluacionCuentaProcedimiento = new EvaluacionCuentaProcedimientoDto();
                        evaluacionCuentaProcedimiento.fecha_evaluacion = evaluacion.fecha_evaluacion;
                        evaluacionCuentaProcedimiento.hora_evaluacion = evaluacion.hora_evaluacion;
                        evaluacionCuentaProcedimiento.servicio = examenAuxiliar.servicio;
                        evaluacionCuentaProcedimiento.grupo = examenAuxiliar.grupo;
                        evaluacionCuentaProcedimiento.evaluacion_procedimiento_id = examenAuxiliar.evaluacion_procedimiento_id;
                        evaluacionCuentaProcedimiento.codigo_procedimiento = examenAuxiliar.codigo_procedimiento;
                        evaluacionCuentaProcedimiento.procedimiento = examenAuxiliar.procedimiento;
                        evaluacionCuentaProcedimiento.cantidad_prescrita = examenAuxiliar.cantidad_prescrita;
                        evaluacionCuentaProcedimiento.cantidad_entregada = examenAuxiliar.cantidad_entregada;
                        evaluacionCuentaProcedimiento.auditoria_cantidad = examenAuxiliar.auditoria_cantidad;
                        evaluacionCuentaProcedimiento.codigo_diagnostico = examenAuxiliar.codigo_diagnostico;
                        evaluacionCuentaProcedimiento.auditoria_codigo_estado = examenAuxiliar.auditoria_codigo_estado;
                        evaluacionCuentaProcedimiento.auditoria_estado = examenAuxiliar.auditoria_estado;
                        evaluacionCuentaProcedimiento.auditoria_observacion = examenAuxiliar.auditoria_observacion;
                        evaluacionCuentaProcedimiento.precio = examenAuxiliar.precio;
                        ListEvaluacionCuentaProcedimiento.Add(evaluacionCuentaProcedimiento);
                    }


                    foreach (var med in evaluacion.Medicamentos)
                    {
                        EvaluacionCuentaMedicamentoDto evaluacionCuentaMedicamento = new EvaluacionCuentaMedicamentoDto();
                        evaluacionCuentaMedicamento.fecha_evaluacion = evaluacion.fecha_evaluacion;
                        evaluacionCuentaMedicamento.hora_evaluacion = evaluacion.hora_evaluacion;
                        evaluacionCuentaMedicamento.evaluacion_medicamento_id = med.evaluacion_medicamento_id;
                        evaluacionCuentaMedicamento.codigo_medicamento = med.codigo_medicamento;
                        evaluacionCuentaMedicamento.medicamento = med.medicamento;
                        evaluacionCuentaMedicamento.dosis = med.dosis;
                        evaluacionCuentaMedicamento.frecuencia = med.frecuencia;
                        evaluacionCuentaMedicamento.dias = med.dias;
                        evaluacionCuentaMedicamento.indicaciones = med.indicaciones;
                        evaluacionCuentaMedicamento.cantidad_prescrita = med.cantidad_prescrita;
                        evaluacionCuentaMedicamento.cantidad_entregada = med.cantidad_entregada;
                        evaluacionCuentaMedicamento.auditoria_cantidad = med.auditoria_cantidad;
                        evaluacionCuentaMedicamento.codigo_diagnostico = med.codigo_diagnostico;
                        evaluacionCuentaMedicamento.auditoria_codigo_estado = med.auditoria_codigo_estado;
                        evaluacionCuentaMedicamento.auditoria_estado = med.auditoria_estado;
                        evaluacionCuentaMedicamento.auditoria_observacion = med.auditoria_observacion;
                        evaluacionCuentaMedicamento.precio = med.precio;
                        ListEvaluacionCuentaMedicamento.Add(evaluacionCuentaMedicamento);
                    }

                }

                atencion.evaluaciones_cuenta = ListEvaluacionCuenta;
                atencion.evaluaciones_cuenta_diagnosticos = ListEvaluacionCuentaDiagnostico;
                atencion.evaluaciones_cuenta_medicamentos = ListEvaluacionCuentaMedicamento;
                atencion.evaluaciones_cuenta_procedimientos = ListEvaluacionCuentaProcedimiento;

                return View(atencion);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ocurrió un error al cargar los datos: {ex.Message}";
                return View("Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTiposFinanciamiento(int fuenteFinanciamientoId)
        {
            try
            {
                var tiposFinanciamiento = await _cuentaAppService.ObtenerTiposFinanciamientoAsync(fuenteFinanciamientoId);
                return Json(tiposFinanciamiento);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error en ObtenerTiposFinanciamiento: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener los tipos de financiamiento.");
            }
        }

        [HttpPost("ActualizarTriaje")]
        public async Task<IActionResult> ActualizarTriaje([FromBody] ActualizarTriajeRequest request)
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                return Unauthorized(RedirectToLoginFromModal());
            }            

            if (request == null)
            {
                return BadRequest("La solicitud es inválida.");
            }

            try
            {
                var usuario = GetUsuario();

                // Limpieza de datos
                request.AuditoriaTriajeObservacion = request.AuditoriaTriajeObservacion?.Trim();
                request.AuditoriaTriajeSubsanaObsTexto = request.AuditoriaTriajeSubsanaObsTexto?.Trim();
                request.AuditoriaUsuario = usuario;

                var resultado = await _historiaService.ActualizarTriaje(request).ConfigureAwait(false);
                if (resultado == null)
                {
                    return NotFound("No se actualizó la cuenta.");
                }

                // Crear el modelo de auditoría
                var estadoAuditoriaModel = new EstadoAuditoriaModel
                {
                    AtencionIdEESS = resultado.atencion_id_eess,
                    CuentaAtencionId = resultado.cuenta_atencion_id,
                    Observacion = request.AuditoriaTriajeObservacion,
                    CodigoEstado = request.AuditoriaTriajeCodigoEstado,
                    FormatoFua = resultado.formato_fua,
                    FuenteFinanciamiento = request.FuenteFinanciamientoId == 3 ? "SIS" : "Particular"
                };

                var resultadoAuditoria = await _externoAppService.ActualizarAuditoriaEmergenciaAsync(token, estadoAuditoriaModel)
                                                                   .ConfigureAwait(false);

                if (resultadoAuditoria == null)
                {
                    return NotFound("No se encontró la atención.");
                }

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error de validación: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Error de operación: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



        [HttpPost("ActualizarCuenta")]
        //[HttpPost]
        public async Task<IActionResult> ActualizarCuenta([FromBody] ActualizarCuentaRequest request)
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                return Unauthorized(RedirectToLoginFromModal());
            }

            var usuario = this.GetUsuario();
            try
            {
                request.AuditoriaUsuario = usuario;
                var resultado = await _cuentaAppService.ActualizarCuenta(request);
                if (resultado == null)
                {
                    return NotFound("No se actualizo la cuenta.");
                }             

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
        
        [HttpPost("CargarAtencionesSinCuentaTriaje")]
        //[HttpPost]
        public async Task<IActionResult> CargarAtencionesSinCuentaTriaje() 
        {
            var token = GetAuthToken();
            var usuario = this.GetUsuario();
            try
            {
                #region sincronizar triaje  
                var resultado = await _externoAppService.SincronizarAtencionesSinCuentaAsync(token);
                if (resultado == null)
                {
                    return NotFound("No se pudo obtener y grabar las atenciones");
                }
                #endregion


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

         
        [HttpPost("CargarEvaluacionesOrdenes")]
        //[HttpPost]
        public async Task<IActionResult> CargarEvaluacionesOrdenes()
        {
            var token = GetAuthToken();
            var usuario = this.GetUsuario();
            try
            {
                var resultado = await _atencionHceService.SincronizarEvaluacionesAsignadasAsync(token, usuario);
                if (resultado == null)
                {
                    return NotFound("No se encontró la atención.");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

         
        [HttpPost("AsignacionAutomatica")]
        //[HttpPost]
        public async Task<IActionResult> AsignacionAutomatica()
        {
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = this.GetUsuario();
            try
            {
                var resultado = await _historiaService.AsignarAtencionesAsync();
                if (resultado == null)
                {
                    return NotFound("No se pudo asignar automaticamente.");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /**/
         
        [HttpGet]
        public async Task<IActionResult> DetalleEvaluacionAsync(long id)
        {
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var evaluacion = await _evaluacionAppService.ObtenerDetalleEvaluacionAsync(id);
                if (evaluacion != null)
                {
                    return Json(new { success = true, data = evaluacion });
                }
                else
                {
                    return Json(new { success = false, message = "Error al obtener la atención." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio de Evaluacion.");
                return Json(new { success = false, message = "Ocurrió un error al comunicarse con el servicio de Evaluacion." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmitirFua(int id)
        {
            var tokenAppHce = "";
            try
            {
                tokenAppHce = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = this.GetUsuario();
            var tokenSIS = await _authService.AuthenticateAsync();

            var atencionPaciente = await _historiaService.GetAtencionOrdenesByIdAsync(tokenAppHce, id);

            if (atencionPaciente == null)
            {
                return NotFound("Atención no encontrada.");
            }

            var categoria_ipress = "08";
            var nivel_ipress = "03";
            var ubigeo_ipress = "150105";
            var renipress = "00006216";
            var tipo_atencion = "3";
            var modalidad_atencion = "1";

            var IdCorrelativoAsegurado = "000000001";
            if (atencionPaciente.sis_asegurado_correlativo != null && atencionPaciente.sis_asegurado_correlativo != "")
            {
                IdCorrelativoAsegurado = atencionPaciente.sis_asegurado_correlativo;
            }

            
            string fechaActual = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var atencion_uno = new Atencion
            {         
                idAtencion = id,
                loteFua = atencionPaciente.lote_fua,
                nroFua = atencionPaciente.nro_formato_fua.PadLeft(8, '0'),
                renipress = renipress,
                idCategoria = categoria_ipress,
                nivel = nivel_ipress,
                idPuntoDigitacion = 0,
                idComponente = atencionPaciente.sis_asegurado_componente, // "1",
                idDisaAsegurado = atencionPaciente.sis_asegurado_disa,
                idLoteAsegurado = atencionPaciente.sis_asegurado_lote,
                idCorrelativoAsegurado = IdCorrelativoAsegurado,
                idSecuenciaAsegurado = "02",
                idTablaAsegurado = atencionPaciente.sis_asegurado_tipo_tabla, //"1",
                idContratoAsegurado = atencionPaciente.sis_asegurado_numero, //"0000000010"
                idPlan = atencionPaciente.sis_asegurado_plan_cobertura, // "02",
                idGrupoPoblacional = atencionPaciente.sis_asegurado_grp_poblacional == null ? "01" : atencionPaciente.sis_asegurado_grp_poblacional, // "01",
                idTipoDocAsegurado = atencionPaciente.codigo_tipo_documento.ToString(),
                numDocAsegurado = atencionPaciente.numero_documento,
                apePaterno = atencionPaciente.apellido_paterno,
                apeMaterno = atencionPaciente.apellido_materno,
                nombres = atencionPaciente.nombres,
                fecNac = DateTime.Parse(atencionPaciente.fecha_nacimiento), //  DateTime.Parse("2024-10-27T20:19:32.824Z"),
                idSexo = atencionPaciente.codigo_tipo_sexo.ToString(), // "1",
                idUbigeo = ubigeo_ipress,
                historiaClinica = atencionPaciente.historia_clinica,
                idTipoAtencion = tipo_atencion,
                idCondicionMaterna = "0",
                idModalidadAtencion = modalidad_atencion, //"1",
                nroAutorizacion = null, // "000000000000015",
                montoAutorizado = 1,
                fecHoraAtencion = atencionPaciente.fecha_ingreso_atencion, // DateTime.Parse("2024-10-27T20:19:32.824Z"),
                renipressReferencia = renipress, //"", // "00000001",
                nroHojaReferencia = "", //"001",
                idServicio = "062",
                idOrigenPersonal = "1",
                idLugarAtencion = "1",
                idDestinoAsegurado = "2",
                fecIngresoHospitalizacion = null, //             DateTime.Parse("2024-10-27T20:19:32.824Z"),
                fecAltaHospitalizacion = null, //                DateTime.Parse("2024-11-01T20:19:32.824Z"),
                renipressContraReferencia = null, // "00000002",
                nroHojaContraReferencia = null, // "002",
                fecParto = null, //                DateTime.Parse("2024-10-27T20:19:32.824Z"),
                idGrupoRiesgo = null, //"1",
                fecFallecimiento = null, // DateTime.Parse("2024-10-27T20:19:32.824Z"),
                renipressOfertaFlexible = null, // "001",
                idEtnia = "80",
                idIafas = null, //"2",
                idCodigoIafas = null, //"001",
                idUps = null, //"1",
                fecCorteAdministrativo = null, // DateTime.Parse("2024-10-27T20:19:32.824Z"),
                idUdrAutorizaVinculado = null, // "000",
                loteAutorizaVinculado = null, //"02",
                nroAutorizaVinculado = null, // "003",
                disaFuaVinculado = null, // "001",
                loteFuaVinculado = null, // "03",
                nroFuaVinculado = null, //"004",
                idTipoDocRespAte = "1", //"02",
                numDocRespAte = atencionPaciente.codigo_medico_ingreso_eess, // "000000002",
                idTipoPersonalSalud = "01",
                idEspecialidadRespAte = null, //"04",
                esEgresadoRespAte = "0", // "1",
                colegiaturaRespAte = null, // "061478",
                rneRespAte = null, //"987654",
                idTipoDocDigitador = "1",
                numDocDigitador = "41744434", // "000000003",
                fecHoraRegistro = DateTime.Parse(fechaActual),
                //DateTime.Parse(DateTime.Now.ToString()),
                // // DateTime.Now, // DateTime.Parse("2024-10-27T20:19:32.824Z"),
                observacion = "-", //"Observación de prueba INSN",
                versionAplicativo = "1.0.0",
                codigoAcreditacion = "12345", //"12345",
                fecHoraIniFuaAdm = null, // DateTime.Parse("2024-10-27T20:19:32.824Z"),
                fecHoraFinFuaAdm = null, // DateTime.Parse("2024-10-28T20:19:32.824Z"),
                idMotivoIngresoCasaMaterna = null,
                idCasaMaterna = null,               
                idEstado = null,
                esObservado = null
            };

            control control = new control()
            {
                idControl = "C1",
                idProceso = 1,
                idUsuarioCrea = 1,
                fecHoraCrea = DateTime.Now,
                observacion = "-"
            };

            atencion_uno.control = control;
                              
            var numeracion = 1;
            List<AtDiagnostico> lAtDiagnostico = new List<AtDiagnostico>();
            List<AtProcedimiento> lAtProcedimientos = new List<AtProcedimiento>();
            List<AtMedicamento> lAtMedicamento = new List<AtMedicamento>();
            List<AtInsumo> lAtInsumo = new List<AtInsumo>();

            foreach (var eval in atencionPaciente.evaluaciones)
            {
                var evaluacion = await _evaluacionAppService.ObtenerDetalleEvaluacionAsync(eval.evaluacion_eess_id);

                foreach (var diagnostico in evaluacion.Diagnosticos)
                {
                    AtDiagnostico atDiagnostico = new AtDiagnostico() {
                        codigo = diagnostico.codigo_diagnostico.ToString().Trim(),
                        nroDiagnostico = numeracion++,
                        tipoMovimiento = "I",
                        tipoDiagnostico = diagnostico.es_principal ? 1 : 2
                    };
                    diagnostico.numero_orden = atDiagnostico.nroDiagnostico;
                    lAtDiagnostico.Add(atDiagnostico);
                }

                foreach (var examenAuxiliar in evaluacion.ExamenesAuxiliares)
                {
                    var nro_orden_dx = evaluacion.Diagnosticos.Where(x => x.codigo_diagnostico.Equals(examenAuxiliar.codigo_diagnostico))?.FirstOrDefault().numero_orden;
                    AtProcedimiento atProcedimiento = new AtProcedimiento()
                    {
                        codigo = examenAuxiliar.codigo_procedimiento.ToString().Trim(),
                        nroDiagnostico = nro_orden_dx == null ? 1 : (int)nro_orden_dx,
                        cantPrescrita = 1,
                        cantEntregada = 1,
                        resultado = "--"
                    };
                    lAtProcedimientos.Add(atProcedimiento);
                }

                foreach (var med in evaluacion.Medicamentos)
                {
                    var nro_orden_dx = evaluacion.Diagnosticos.Where(x => x.codigo_diagnostico.Equals(med.codigo_diagnostico))?.FirstOrDefault().numero_orden;
                    AtMedicamento atMedicamentos = new AtMedicamento()
                    {
                        codigo = med.codigo_sismed.ToString().Trim(),
                        nroDiagnostico = nro_orden_dx == null ? 1 : (int)nro_orden_dx,
                        cantPrescrita = med.cantidad_prescrita,
                        cantEntregada = med.cantidad_prescrita,
                        fecPetitorio = DateTime.Now,
                        nroDocPetitorio = "-",  //"PET123",
                        lote = "-", //L12345",
                        nroSerie = "-", //NS12345",
                        registroSanitario = "-", //"RS002",
                        fecVencimiento = null, //DateTime.Now.AddMonths(12).ToString("yyyy-MM-dd"),
                        contieneOtrosDatos = ""
                    };
                    lAtMedicamento.Add(atMedicamentos);
                }
            }

            atencion_uno.atDiagnosticos = lAtDiagnostico;
            atencion_uno.atProcedimientos = lAtProcedimientos;
            atencion_uno.atMedicamentos = lAtMedicamento;
            atencion_uno.atInsumos = lAtInsumo;                      

            var atRecienNacidos = new List<AtRecienNacido>();
            var atServAdicionales = new List<AtServAdicionales>();
            var atServMatInfantiles = new List<AtServMatInfantil>();
            var atTransportes = new List<AtTransporte>();
            var atViaticos = new List<AtViatico>();
            var atOtrosGastos = new List<AtOtrosGasto>();

            atencion_uno.atRecienNacidos = atRecienNacidos;
            atencion_uno.atServAdicionales = atServAdicionales;
            atencion_uno.atServMatInfantiles = atServMatInfantiles;
            atencion_uno.atTransportes = atTransportes;
            atencion_uno.atViaticos = atViaticos;
            atencion_uno.atOtrosGastos = atOtrosGastos;


            var resRegFua = await _IFuaEmitidoService.RegistrarFuaAsync(atencion_uno);
            if (resRegFua == null)
            {
                return NotFound("No se actualizo la cuenta.");
            }

            if (tokenSIS == null)
            {
                return BadRequest(new { message = "No se pudo conectar al SIS, Usuario no encontrado o credenciales incorrectas" });
            }


            List<Atencion> atencion = new List<Atencion>();
            atencion.Add(atencion_uno);

          
            if (atencion == null)
                return NotFound();

            var client = _httpClientFactory.CreateClient();
            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenSIS);
            }
            client.BaseAddress = new Uri("https://desarrollo15.sis.gob.pe");
            var jsonContent = JsonConvert.SerializeObject(new { atencion });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            client.Timeout = TimeSpan.FromMinutes(5);
            try
            {
                var response = await client.PostAsync("/api/v1/recepcion/procesa-fua", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var resultado = System.Text.Json.JsonSerializer.Deserialize<RespuestaEmitirFua>(jsonResponse);

                    if (resultado != null && resultado.Success)
                    {
                        ActualizarRespuestaFUARequest ActualizarCuentaFUARequest = new ActualizarRespuestaFUARequest();
                        ActualizarCuentaFUARequest.AtencionId = id;
                        ActualizarCuentaFUARequest.FuaGuid = resultado.Data.Guid;
                        ActualizarCuentaFUARequest.FuaEstado = "R";
                        ActualizarCuentaFUARequest.FuaMensaje = resultado.Mensaje.FirstOrDefault();
                        ActualizarCuentaFUARequest.FuaAdvertencia = resultado.Data.Errores?.FirstOrDefault().Ruta + ": " + resultado.Data.Errores?.FirstOrDefault().Mensaje;
                        ActualizarCuentaFUARequest.AuditoriaUsuario = usuario;

                        var resultadoFUA = await _cuentaAppService.ActualizarCuentaFUA(ActualizarCuentaFUARequest);
                        if (resultadoFUA == null)
                        {
                            return NotFound("No se actualizo la cuenta.");
                        }

                        var token = GetAuthToken();
                        var resultadoEstadoFua = await _externoAppService.ActualizarEmergenciaEstadoFuaAsync(token, ActualizarCuentaFUARequest)
                                                                      .ConfigureAwait(false);

                        if (resultadoEstadoFua == null)
                        {
                            return NotFound("No se encontró la atención.");
                        }

                        return Ok(new
                        {
                            Message = "FUA emitida con éxito",
                            TrackingId = resultado.Data.Guid,
                            Advertencias = resultado.Data.Advertencias
                        });

                    }

                    return BadRequest(new
                    {
                        Message = "La respuesta fue exitosa, pero no contiene datos válidos."
                    });
                }

                // Manejar errores en la solicitud
                var errorResponse = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new
                {
                    Message = "Error al procesar la solicitud de FUA,  jsonContent: " + jsonContent,
                    Details = errorResponse + " tokenSIS:" + tokenSIS + ", response.RequestMessage: " + response.RequestMessage + " response.StatusCode:" + response.StatusCode
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

            

           
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarFua(int id, string guidFua)
        {            
            var tokenAppHce = "";
            try
            {
                tokenAppHce = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = this.GetUsuario();
            var tokenSIS = await _authService.AuthenticateAsync();

            if (tokenSIS == null)
            {
                return BadRequest(new { message = "No se pudo conectar al SIS, Usuario no encontrado o credenciales incorrectas" });
            }        

            var response = await _authService.ConsultarAsync(guidFua, tokenSIS);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var resultado = System.Text.Json.JsonSerializer.Deserialize<RespuestaConsultarFua>(jsonResponse);             

                if (resultado == null)
                {
                    return BadRequest(new {Message = "El resultado del envio del fua es nulo." });
                }

                var fuaData = resultado.Data?.FirstOrDefault();
                var error = fuaData?.Errores?.FirstOrDefault();

                var actualizarCuentaFUARequest = new ActualizarRespuestaFUARequest
                {
                    AtencionId = id,
                    FuaGuid = resultado.Success ? fuaData?.Guid ?? "" : "",
                    FuaEstado = resultado.Success ? fuaData?.IdEstado : null,
                    FuaMensaje = resultado.Mensaje?.FirstOrDefault(),
                    FuaAdvertencia = error != null ? $"{error.RutaError}: {error.MensajeError}" : null,
                    AuditoriaUsuario = usuario
                };

                var resultadoFUA = await _cuentaAppService.ActualizarCuentaFUA(actualizarCuentaFUARequest);
                if (resultadoFUA == null)
                {
                    return resultado.Success
                        ? NotFound("No se actualizó el estado del fua.")
                        : BadRequest(new
                        {
                            Message = "Error al actualizar el estado del fua.",
                            TrackingId = fuaData?.Guid,
                            Advertencias = error?.MensajeError
                        });
                }

                tokenAppHce = GetAuthToken();
                var resultadoEstadoFua = await _externoAppService.ActualizarEmergenciaEstadoFuaAsync(tokenAppHce, actualizarCuentaFUARequest)
                                                              .ConfigureAwait(false);

                if (resultadoEstadoFua == null)
                {
                    return NotFound("No se encontró la atención.");
                }

                return resultado.Success
                    ? Ok(new
                    {
                        Message = "FUA consultado con éxito",
                        TrackingId = fuaData?.Guid,
                        Advertencias = error?.MensajeError
                    })
                    : BadRequest(new
                    {
                        Message = "FUA consultado con errores",
                        TrackingId = fuaData?.Guid,
                        Advertencias = error?.MensajeError
                    });

            }

            // Manejar errores en la solicitud
            var errorResponse = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, new
            {
                Message = "Error al procesar la solicitud de FUA",
                Details = errorResponse
            });
        }


        #region Item Auditoria
        [HttpGet]        
        public async Task<IActionResult> DetalleItemAuditoriaAsync(int id, string puntoCarga)
        {
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var itemAuditoria = await _evaluacionAppService.ObtenerItemAuditoriaIdAsync(id, puntoCarga);

                if (itemAuditoria != null)
                {
                    return Json(new { success = true, data = itemAuditoria });
                }
                else
                {
                    return Json(new { success = false, message = "Error al obtener el ítem de auditoría." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio de ItemAuditoria.");
                return Json(new { success = false, message = "Ocurrió un error al comunicarse con el servicio de ItemAuditoria." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> DetalleExamenAuxiliarAsync(int id)
        {
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var examenAuxiliar = await _atencionHceService.ObtenerDetalleExamenAuxiliarAsync(id, token);
                if (examenAuxiliar != null)
                {
                    return Json(new { success = true, data = examenAuxiliar });
                }
                else
                {
                    return Json(new { success = false, message = "Error al obtener el examen auxiliar." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio ExamenAuxiliarService.");
                return Json(new { success = false, message = "Ocurrió un error al comunicarse con el servicio ExamenAuxiliarService." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AtencionCEAuditoriaAsync([FromBody] AtencionCEAuditoriaDto atencionCEAuditoriaDto)
        {
            var username = HttpContext.Request.Cookies["Usuario"];
            atencionCEAuditoriaDto.AuditoriaUsuario = username;
            atencionCEAuditoriaDto.Token = GetAuthToken();

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos." });
            }

            try
            {
                var resultado = await _atencionHceService.ActualizarAtencionAuditoriaAsync(atencionCEAuditoriaDto);
                return Json(new { success = resultado, message = resultado ? "Atención actualizada exitosamente." : "Error al actualizar la atención." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio AtencionCEAuditoriaService.");
                return Json(new { success = false, message = "Ocurrió un error al comunicarse con el servicio AtencionCEAuditoriaService." });
            }
        }

        [HttpPost("ActualizarAuditoriaItem")]
        public async Task<IActionResult> ActualizarAuditoriaItem([FromBody] ActualizarAuditoriaRequest itemAuditoriaDto)
        {
            var usuario = this.GetUsuario();
            itemAuditoriaDto.AuditoriaUsuario = usuario;
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var resultado = await _evaluacionAppService.ActualizarAuditoriaAsync(itemAuditoriaDto);
                if (resultado == null)
                {
                    return NotFound("No se actualizo la cuenta.");
                }
       
                var estadoAuditoriaModel = new ActualizarAuditoriaItemRequest
                {
                    PuntoCarga = itemAuditoriaDto.PuntoCarga,
                    EvaluacionEessId = itemAuditoriaDto.EvaluacionEessId,
                    CodigoItem = itemAuditoriaDto.CodigoItem,
                    AuditoriaCantidad = itemAuditoriaDto.AuditoriaCantidad,
                    AuditoriaObservacion = itemAuditoriaDto.AuditoriaObservacion,
                    IdEstadoItem = itemAuditoriaDto.IdEstadoItem,
                    AuditoriaUsuario = itemAuditoriaDto.AuditoriaUsuario
                };

                var resultadoAuditoria = await _externoAppService.ActualizarAuditoriaEmergenciaItemAsync(token, estadoAuditoriaModel)
                                                                   .ConfigureAwait(false);

                if (resultadoAuditoria == null)
                {
                    return NotFound("No se encontró la atención.");
                }


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }


        [HttpPost("AceptacionMasivaItemsEvaluacion")]
        public async Task<IActionResult> AceptacionMasivaItemsEvaluacion([FromBody] AceptarMasivaEvaluacionRequest itemAuditoriaDto)
        {
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (itemAuditoriaDto == null)            
                return NotFound("Debe seleccionar una evaluación");
            
            if (itemAuditoriaDto.EvaluacionEESSId == null)            
                return NotFound("Debe seleccionar una evaluación");
            
            var usuario = this.GetUsuario();
            itemAuditoriaDto.AuditoriaUsuario = usuario;

            try
            {
                var resultado = await _evaluacionAppService.AceptacionMasivaItemsEvaluacion(itemAuditoriaDto);
                if (resultado == null)                
                    return NotFound("No se actualizo la auditoria masiva.");

                var resultadoAuditoria = await _externoAppService.ActualizarMasivoAuditoriaEmergenciaItemAsync(token, itemAuditoriaDto)
                                                                   .ConfigureAwait(false);

                if (resultadoAuditoria == null)                
                    return NotFound("No se encontró la atención.");               

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }


        [HttpPost("RegistrarRecetaOrdenApoyoDx")]
        public async Task<IActionResult> AceptacionMasivaItemsEvaluacion([FromBody] EnviaOrdenRequest itemAuditoriaDto)
        {
            if (itemAuditoriaDto == null)
                return NotFound("Debe seleccionar una evaluación");


            if (itemAuditoriaDto.EvaluacionEESSId == null)
                return NotFound("Debe seleccionar una evaluación");

            var usuario = this.GetUsuario();
            //itemAuditoriaDto.AuditoriaUsuario = usuario;
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }


            try
            {
                var resultadoAuditoria = await _externoAppService.RegistrarRecetaOrden_ApoyoDx_Async(token, itemAuditoriaDto)
                                                                   .ConfigureAwait(false);

                if (resultadoAuditoria == null || resultadoAuditoria == false)
                    return NotFound("No se registro las ordenes y recetas");             

                var resultado = await _evaluacionAppService.ActualizarEnvioReceta(itemAuditoriaDto);
                if (resultado == null)
                    return NotFound("No se actualizo el envio de la receta.");

                //var resultadoAuditoria = await _externoAppService.ActualizarMasivoAuditoriaItem_Emergencia_Async(token, itemAuditoriaDto)
                //                                                   .ConfigureAwait(false);

                //if (resultadoAuditoria == null)
                //    return NotFound("No se encontró la atención.");

                if (resultadoAuditoria == null)
                    return NotFound("No se encontró la atención.");

                return Ok(resultadoAuditoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("ObtenerResultadosItemEvaluacion")]
        public async Task<IActionResult> ObtenerResultadosItemEvaluacion([FromBody] ObtenerResultadoRequest itemAuditoriaDto)
        {
            if (itemAuditoriaDto == null)
                return NotFound("Debe seleccionar una evaluación");

            if (itemAuditoriaDto.EvaluacionEESSId == null)
                return NotFound("Debe seleccionar una evaluación");

            var usuario = this.GetUsuario();
            var token = GetAuthToken();

            try
            {

                return Ok();


                //var resultadoAuditoria = await _externoAppService.RegistrarRecetaOrden_ApoyoDx_Async(token, itemAuditoriaDto)
                //                                                   .ConfigureAwait(false);

                //if (resultadoAuditoria == null || resultadoAuditoria == false)
                //    return NotFound("No se registro las ordenes y recetas");

                //var resultado = await _evaluacionAppService.ActualizarEnvioReceta(itemAuditoriaDto);
                //if (resultado == null)
                //    return NotFound("No se actualizo el envio de la receta.");



                //if (resultadoAuditoria == null)
                //    return NotFound("No se encontró la atención.");

                //return Ok(resultadoAuditoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        #endregion


        [HttpGet]
        public async Task<IActionResult> BuscarAfiliadoSIS(string s_tipobus, string s_numero)
        {
            try
            {
                var afiliado = await _externoAppService.BuscarAfiliadoSISAsync(s_tipobus, s_numero);

                if (afiliado != null)
                {
                    return Ok(afiliado); 
                }

                return BadRequest("No se encontraron datos para el afiliado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> detalleBoletaAtencion(string nroHistoria, string nroBoleta, string nombPaciente)
        {
            var token = "";
            try
            {
                token = GetAuthToken();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var boletaResultado = await _externoAppService.ObtenerBoletaAtencionAsync(nroHistoria, nroBoleta, nombPaciente, token);

                if (boletaResultado != null)
                {
                    return Json(new { success = true, data = boletaResultado });
                }
                else
                {
                    return Json(new { success = false, message = "No se encontró la boleta de atención." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio para obtener la boleta de atención.");
                return Json(new { success = false, message = "Ocurrió un error al comunicarse con el servicio de boletas." });
            }
        }



        private string GetUsuario()
        {
            var usuario = HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new UnauthorizedAccessException("Usuario no disponible.");
            }

            return usuario;
        }

        private string GetAuthToken()
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                throw new UnauthorizedAccessException("Token de autenticación no disponible o expirado.");
            }

            return token;
        }

        // Método para validar si el token ha expirado
        private bool TokenExpirado(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
                return true;  

            var exp = jsonToken.ValidTo; 
            return exp < DateTime.UtcNow; 
        }
    }
}
