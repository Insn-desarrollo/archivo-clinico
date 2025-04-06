using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Domain.Entities
{
    public class UsuarioAdmin
    {
        public string Usuario { get; set; }
        public string Nombre { get; set; }
    }

    public class HistoriaClinicaDto
    {
        public int historia_clinica_id { get; set; }
        public int paciente_id { get; set; }
        public string? numero_historia { get; set; }
        public int codigo_tipo_documento { get; set; }
        public string? tipos_documento { get; set; }
        public string? numero_documento { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? nombres { get; set; }
        public string? fecha_nacimiento { get; set; } // Formateado como DD/MM/YYYY
        public string? codigo_tipo_sexo { get; set; }
        public string? tipo_sexo { get; set; }
        public string? direccion { get; set; } = string.Empty;
        public string? correo { get; set; } = string.Empty;
        public int codigo_estado_historia { get; set; }
        public string? estado_historia { get; set; }
        public int total_records { get; set; }
        public int total_pages { get; set; }
    }

    public class HistoriaClinicaConsultaDto
    {
        public int historia_clinica_id { get; set; }
        public int paciente_id { get; set; }
        public string? numero_historia { get; set; }
        public int codigo_tipo_documento { get; set; }
        public string? tipos_documento { get; set; }
        public string? numero_documento { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? nombres { get; set; }
        public string? fecha_nacimiento { get; set; } 
        public string? codigo_tipo_sexo { get; set; }
        public string? tipo_sexo { get; set; }
        public string? direccion { get; set; } = string.Empty;
        public string? correo { get; set; } = string.Empty;
        public int codigo_estado_historia { get; set; }
        public string? estado_historia { get; set; }
        public IEnumerable<AtencionHcDto>? atenciones { get; set; }
    }

    public class AtencionHcDto
    {
        public int atencion_id { get; set; }
        public string? historia_clinica { get; set; }
        public string? numero_documento { get; set; }
        public string? nombre_paciente { get; set; }
        public string? servicio { get; set; }
        public string? fecha_hora_atencion { get; set; }
        public IEnumerable<EvaluacionHcDto>? evaluaciones { get; set; }
        public IEnumerable<DocumentosEvaluacionHC>? documentosAtencion { get; set; }
    }

    public class EvaluacionHcDto
    {
        public int evaluacion_id { get; set; }
        public string? fecha_hora_evaluacion { get; set; }
        public IEnumerable<DocumentosEvaluacionHC>? documentos { get; set; }
    }

    public class DocumentosEvaluacionHC
    {
        public int? triaje_id { get; set; }
        public int? atencion_id { get; set; }
        public int? evaluacion_id { get; set; }
        public string? historia_clinica { get; set; }
        public string? paciente { get; set; }
        public string? tipo_documento { get; set; }
        public string? documento { get; set; }
        public string? estado { get; set; }
        public string? fecha_registro { get; set; }
        public string? fecha_firma { get; set; }
        public string? profesional_id { get; set; }

    }


    public class AtencionDto
    {
        public int atencion_id { get; set; }
        public int cuenta_atencion_id { get; set; }
        public int codigo_tipo_servicio { get; set; }
        public string? tipo_servicio { get; set; }
        public string? historia_clinica { get; set; }
        public int codigo_tipo_documento { get; set; }
        public string? tipos_documento { get; set; }
        public string? numero_documento { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? nombres { get; set; }
        public string? fecha_nacimiento { get; set; }
        public int? codigo_tipo_sexo { get; set; }
        public string? tipo_sexo { get; set; }
        public DateTime? fecha_ingreso_atencion { get; set; }
        public string? fecha_ingreso_atencion_str { get; set; }
        public string? hora_ingreso_atencion { get; set; }
        public int? servicio_ingreso_eess_id { get; set; }
        public string? servicio_ingreso_eess { get; set; }
        public string? codigo_medico_ingreso_eess { get; set; }
        public string? medico_ingreso_eess { get; set; }
        public int? auditoria_codigo_estado { get; set; }
        public string? auditoria_estado { get; set; }
        public int? auditoria_triaje_codigo_estado { get; set; }
        public string? auditoria_triaje_estado { get; set; }
        public string? auditoria_observacion { get; set; }
        public int? fuente_financiamiento_id { get; set; }
        public int? tipo_financiamiento_id { get; set; }
        public string? sis_asegurado_componente { get; set; }
        public string? sis_asegurado_disa { get; set; }
        public string? sis_asegurado_lote { get; set; }
        public string? sis_asegurado_numero { get; set; }
        public string? sis_asegurado_correlativo { get; set; }
        public string? sis_asegurado_tipo_tabla { get; set; }
        public string? sis_asegurado_identificador { get; set; }
        public string? sis_asegurado_plan_cobertura { get; set; }
        public string? sis_asegurado_grp_poblacional { get; set; }
        public string? sis_asegurado_fecha_afiliacion { get; set; }
        public string? sis_asegurado_tipo_seguro { get; set; }
        public string? sis_asegurado_desc_tipo_seguro { get; set; }
        public string? sis_asegurado_estado { get; set; }
        public int? auditoria_cuenta_codigo_estado { get; set; }
        public string? auditoria_cuenta_observacion { get; set; }
        public string? antecedentes { get; set; }
        public string? prioridad { get; set; }
        public string? motivo { get; set; }
        public string? origen_atencion_eess { get; set; }
        public string? particular_boleta_apertura { get; set; }
        public string? particular_concepto_boleta { get; set; }
        public string? auditoria_triaje_observacion { get; set; }
        public bool? auditoria_triaje_subsana_obs { get; set; }
        public string? auditoria_triaje_subsana_obs_texto { get; set; }
        public string? lote_fua { get; set; }
        public string? nro_formato_fua { get; set; }
        public List<FuenteFinanciamientoDto>? fuentes_financiamiento { get; set; }
        public List<TipoFinanciamientoDto>? tipos_financiamiento { get; set; }
        public List<EvaluacionMinDto>? evaluaciones { get; set; }
        public List<EvaluacionCuentaDto>? evaluaciones_cuenta { get; set; }
        public List<EvaluacionCuentaDiagnosticoDto>? evaluaciones_cuenta_diagnosticos { get; set; }
        public List<EvaluacionCuentaProcedimientoDto>? evaluaciones_cuenta_procedimientos { get; set; }
        public List<EvaluacionCuentaMedicamentoDto>? evaluaciones_cuenta_medicamentos { get; set; }
        public List<ObservacionTriaje>? observaciones_triaje { get; set; }
    }
    public class ObservacionTriaje
    {
        public int observacion_triaje_id { get; set; }
        public int indice { get; set; }
        public string observacion { get; set; }
        public int codigo_estado_observacion { get; set; }
        public string estado_observacion { get; set; }
    }
    public class FuenteFinanciamientoDto
    {
        public int fuente_financiamiento_id { get; set; }
        public string? descripcion { get; set; }
    }

    public class TipoFinanciamientoDto
    {
        public int tipo_financiamiento_id { get; set; }
        public string? descripcion { get; set; }
    }

    public class EvaluacionMinDto
    {
        public long secuencia { get; set; }
        public int evaluacion_id { get; set; }
        public int evaluacion_eess_id { get; set; }
        public string? fecha_evaluacion { get; set; }
        public string? hora_evaluacion { get; set; }
        public string? auditoria_estado { get; set; }
        public string receta_estado { get; set; }
        public string orden_estado { get; set; }
    }

    public class EvaluacionCuentaDto
    {
        public decimal evaluacion_id { get; set; }
        public decimal? evaluacion_eess_id { get; set; }
        public DateTime? fecha_evaluacion { get; set; }
        public string? servicio_eess { get; set; }
        public string? hora_evaluacion { get; set; }
        public int? auditoria_codigo_estado { get; set; }
        public string? auditoria_estado { get; set; }
        public string? auditoria_observacion { get; set; }
    }

    public class EvaluacionCuentaDiagnosticoDto
    {
        public DateTime? fecha_evaluacion { get; set; }
        public string? hora_evaluacion { get; set; }
        public int codigo_tipo_diagnostico { get; set; }
        public string? tipo_diagnostico { get; set; }
        public string? codigo_diagnostico { get; set; }
        public string? diagnostico { get; set; }
        public bool es_principal { get; set; }
    }

    public class EvaluacionCuentaProcedimientoDto
    {
        public DateTime? fecha_evaluacion { get; set; }
        public string? hora_evaluacion { get; set; }
        public string servicio { get; set; }
        public string grupo { get; set; }
        public int evaluacion_procedimiento_id { get; set; }
        public string codigo_procedimiento { get; set; }
        public string procedimiento { get; set; }
        public int cantidad_prescrita { get; set; }
        public int cantidad_entregada { get; set; }
        public int auditoria_cantidad { get; set; }
        public string codigo_diagnostico { get; set; }
        public int auditoria_codigo_estado { get; set; }
        public string auditoria_estado { get; set; }
        public string auditoria_observacion { get; set; }
        public decimal precio { get; set; }
    }

    public class EvaluacionCuentaMedicamentoDto
    {
        public DateTime? fecha_evaluacion { get; set; }
        public string? hora_evaluacion { get; set; }
        public int evaluacion_medicamento_id { get; set; }
        public string codigo_medicamento { get; set; }
        public string medicamento { get; set; } = string.Empty;
        public string dosis { get; set; } = string.Empty;
        public string frecuencia { get; set; } = string.Empty;
        public string dias { get; set; } = string.Empty;
        public string indicaciones { get; set; } = string.Empty;
        public int cantidad_prescrita { get; set; }
        public int cantidad_entregada { get; set; }
        public int auditoria_cantidad { get; set; }
        public string codigo_diagnostico { get; set; } = string.Empty;
        public int auditoria_codigo_estado { get; set; }
        public string auditoria_estado { get; set; } = string.Empty;
        public string auditoria_observacion { get; set; } = string.Empty;
        public decimal precio { get; set; }
    }  

    // falta cambios

    public class AuditorDto
    {
        public string Usuario { get; set; }
        public int Cargo { get; set; }
    }

    public class ActualizarTriajeRequest
    {
        public int AtencionId { get; set; }
        public DateTime FechaIngresoAtencion { get; set; }
        public string? HoraIngresoAtencion { get; set; }
        public int FuenteFinanciamientoId { get; set; }
        public int TipoFinanciamientoId { get; set; }
        public string? SisAseguradoComponente { get; set; }
        public string? SisAseguradoDisa { get; set; }
        public string? SisAseguradoLote { get; set; }
        public string? SisAseguradoNumero { get; set; }
        public string? SisAseguradoCorrelativo { get; set; }
        public string? SisAseguradoTipoTabla { get; set; }
        public string? SisAseguradoIdentificador { get; set; }

        public string? SisAseguradoPlanCobertura { get; set; }
        public string? SisAseguradoGrpPoblacional { get; set; }
        public string? SisAseguradoFechaAfiliacion { get; set; }
        public string? SisAseguradoTipoSeguro { get; set; }
        public string? SisAseguradoDescTipoSeguro { get; set; }
        public string? SisAseguradoEstado { get; set; }

        public string? ParticularBoletaApertura { get; set; }
        public string? ParticularConceptoBoleta { get; set; }

        public string? AuditoriaObservacion { get; set; }
        public int AuditoriaTriajeCodigoEstado { get; set; }
        public string? AuditoriaUsuario { get; set; }
        public string? AuditoriaTriajeObservacion { get; set; }
        public bool? AuditoriaTriajeSubsanaObs { get; set; }
        public string? AuditoriaTriajeSubsanaObsTexto { get; set; }

    }

    public class ActualizarCuentaRequest
    {
        public int AtencionId { get; set; }    
        public string? AuditoriaObservacion { get; set; }
        public int AuditoriaCuentaCodigoEstado { get; set; }
        public string? AuditoriaUsuario { get; set; }
    }

    public class ActualizarRespuestaFUARequest
    {
        public int AtencionId { get; set; }
        public string? FuaGuid { get; set; }
        public string? FuaMensaje { get; set; }
        public string? FuaAdvertencia { get; set; }
        public string? FuaEstado { get; set; }
        public string? AuditoriaUsuario { get; set; }
    }

    public class ActualizarAuditoriaRequest
    {
        public string? PuntoCarga { get; set; }
        public long EvaluacionEessId { get; set; }        
        public int IdItem { get; set; }        
        public string? CodigoItem { get; set; }
        public int? AuditoriaCantidad { get; set; }
        public string? AuditoriaObservacion { get; set; }
        public int? IdEstadoItem { get; set; }
        public string? AuditoriaUsuario { get; set; }
        public bool? AuditoriaOrdenSubsanaObs { get; set; }
        public string? AuditoriaOrdenSubsanaObsTexto { get; set; }
    }

    public class ActualizarAuditoriaItemRequest
    {
        public string? PuntoCarga { get; set; }
        public long EvaluacionEessId { get; set; }
        public string? CodigoItem { get; set; }
        public int? AuditoriaCantidad { get; set; }
        public string? AuditoriaObservacion { get; set; }
        public int? IdEstadoItem { get; set; }
        public string? AuditoriaUsuario { get; set; }
    }

    public class AceptarMasivaEvaluacionRequest
    {
        public long EvaluacionEESSId { get; set; }      
        public string? AuditoriaUsuario { get; set; }
    }

    public class EnviaOrdenRequest
    {
        public long EvaluacionEESSId { get; set; }
    }


    public class ObtenerResultadoRequest
    {
        public long EvaluacionEESSId { get; set; }
    }


    public class TriajeResponseDto
    {
        public long atencion_id_eess { get; set; }
        public long? cuenta_atencion_id { get; set; }
        public string? formato_fua { get; set; }        
    }

    public class Diagnostico
    {
        public int Id { get; set; }
        public required string CIE10 { get; set; }
        public required string TipoDiagnostico { get; set; }
        public required string Descripcion { get; set; }
    }

    public class Examen
    {
        public int Id { get; set; }
        public required string PtoCarga { get; set; }
        public required string Receta { get; set; }
        public required string Codigo { get; set; }
        public required string Producto { get; set; }
        public required string CIE10 { get; set; }
        public int Cantidad { get; set; }
    }

    public class Medicamento
    {
        public int Id { get; set; }
        public required string Receta { get; set; }
        public required string Codigo { get; set; }
        public required string Producto { get; set; }
        public required string Presenta { get; set; }
        public required string CIE10 { get; set; }
        public int Dosis { get; set; }
        public int Frecuencia { get; set; }
        public int Dias { get; set; }
        public int Cantidad { get; set; }
        public required string Indicaciones { get; set; }
    }


    public class EvaluacionDto
    {
        public long evaluacion_id { get; set; }
        public long? evaluacion_eess_id { get; set; }
        public DateTime? fecha_evaluacion { get; set; }
        public string? servicio_eess { get; set; }
        public string? hora_evaluacion { get; set; }
        public int? auditoria_codigo_estado { get; set; }
        public string? auditoria_observacion { get; set; }
        public int? codigo_estado_receta { get; set; }
        public int? codigo_estado_orden { get; set; }
        public string? receta_estado { get; set; }
        public string? orden_estado { get; set; }

        public IEnumerable<EvaluacionDiagnosticoDto>? Diagnosticos { get; set; }
        public IEnumerable<EvaluacionProcedimientoDto>? ExamenesAuxiliares { get; set; }
        public IEnumerable<EvaluacionMedicamentoDto>? Medicamentos { get; set; }
    }

    public class EvaluacionDiagnosticoDto
    {
        public int numero_orden { get; set; }
        public int codigo_tipo_diagnostico { get; set; }
        public string? tipo_diagnostico { get; set; }
        public string? codigo_diagnostico { get; set; }
        public string? diagnostico { get; set; }
        public bool es_principal { get; set; }
    }

    public class EvaluacionProcedimientoDto
    {
        public int numero_orden { get; set; }
        public string servicio { get; set; }
        public string grupo { get; set; }
        public int evaluacion_procedimiento_id { get; set; }
        public string codigo_procedimiento { get; set; }
        public string procedimiento { get; set; }
        public int cantidad_prescrita { get; set; }
        public int cantidad_entregada { get; set; }
        public int auditoria_cantidad { get; set; }
        public string? codigo_diagnostico { get; set; }
        public int auditoria_codigo_estado { get; set; }
        public string auditoria_estado { get; set; }
        public string auditoria_observacion { get; set; }
        public bool? auditoria_orden_subsana_obs { get; set; }
        public string? auditoria_orden_subsana_obs_texto { get; set; } = string.Empty;
        public decimal precio { get; set; }
        public bool entregado { get; set; }        
    }

    public class EvaluacionMedicamentoDto
    {
        public int numero_orden { get; set; }
        public int evaluacion_medicamento_id { get; set; }
        public string codigo_medicamento { get; set; }
        public string codigo_sismed { get; set; }        
        public string medicamento { get; set; } = string.Empty;
        public string dosis { get; set; } = string.Empty;
        public string frecuencia { get; set; } = string.Empty;
        public string dias { get; set; } = string.Empty;
        public string indicaciones { get; set; } = string.Empty;
        public int cantidad_prescrita { get; set; }
        public int cantidad_entregada { get; set; }
        public int auditoria_cantidad { get; set; }
        public string codigo_diagnostico { get; set; } = string.Empty;
        public int auditoria_codigo_estado { get; set; }
        public string auditoria_estado { get; set; } = string.Empty;
        public string auditoria_observacion { get; set; } = string.Empty;
        public bool? auditoria_orden_subsana_obs { get; set; }
        public string? auditoria_orden_subsana_obs_texto { get; set; } = string.Empty;
        public decimal precio { get; set; }        
        public bool tiene_resultado { get; set; }
    }


    public class ItemAuditoriaDto
    {
        public long IdItem { get; set; }
        public int CodigoGrupoItem { get; set; }
        public string? AuditoriaPuntoCarga { get; set; }
        public string? Grupo { get; set; }
        public string? CodigoItem { get; set; }
        public string? Item { get; set; }
        public string? Cie10 { get; set; }
        public string? Diagnostico { get; set; }        
        public string? AuditoriaUsuario { get; set; }
        public DateTime? AuditoriaFecha { get; set; }
        public string? AuditoriaObservacion { get; set; }
        public decimal? CantidadPrescrita { get; set; }
        public decimal? CantidadEntregada { get; set; }
        public decimal? AuditoriaCantidad { get; set; }
        public int? IdEstadoAuditoria { get; set; }
        public string? EstadoAuditoria { get; set; }

        public bool? AuditoriaOrdenSubsanaObs { get; set; }
        public string? AuditoriaOrdenSubsanaObsTexto { get; set; }      

    }


    public class AfiliadoSISDto
    {
        public string? IdError { get; set; }
        public string? Resultado { get; set; }
        public string? TipoDocumento { get; set; }
        public string? NroDocumento { get; set; }
        public string? ApePaterno { get; set; }
        public string? ApeMaterno { get; set; }
        public string? Nombres { get; set; }
        public string? FecAfiliacion { get; set; }
        public string? EESS { get; set; }
        public string? DescEESS { get; set; }
        public string? EESSUbigeo { get; set; }
        public string? DescEESSUbigeo { get; set; }
        public string? Regimen { get; set; }
        public string? TipoSeguro { get; set; }
        public string? DescTipoSeguro { get; set; }
        public string? Contrato { get; set; }
        public string? FecCaducidad { get; set; }
        public string? Estado { get; set; }
        public string? Tabla { get; set; }
        public string? IdNumReg { get; set; }
        public string? Genero { get; set; }
        public string? FecNacimiento { get; set; }
        public string? IdUbigeo { get; set; }
        public string? Direccion { get; set; }
        public string? Disa { get; set; }
        public string? TipoFormato { get; set; }
        public string? NroContrato { get; set; }
        public string? Correlativo { get; set; }
        public string? IdPlan { get; set; }
        public string? IdGrupoPoblacional { get; set; }
        public string? MsgConfidencial { get; set; }
    }

    public class EstadoAuditoriaModel
    {
        public long? AtencionIdEESS { get; set; }
        public long? CuentaAtencionId { get; set; }
        public string Observacion { get; set; }
        public int? CodigoEstado { get; set; }
        public string? FormatoFua { get; set; }
        public string? FuenteFinanciamiento { get; set; }
    }

    public class BoletaResultado
    {
        public string cmbol_nume { get; set; }
        public DateTime cmbol_femi { get; set; }
        public decimal cmbol_totp { get; set; }
        public string hc { get; set; }
        public string cmbol_nomb { get; set; }
        public string codi_tari { get; set; }
        public string cmta_desc { get; set; }
        public decimal cdbol_cant { get; set; }
        public decimal cdbol_prec { get; set; }
    }

    public class BoletaConsulta
    {
        public string? nro_boleta { get; set; }
        public string? nro_historia { get; set; }
    }

    public class ContadorPendientes
    {
        public int? cantidad_por_asignar { get; set; }
        public int? cantidad_triaje_pendiente { get; set; }
        public int? cantidad_evaluaciones_pendiente { get; set; }
        public int? cantidad_cuentas_pendientes { get; set; }
        public int? cantidad_sis_pendientes { get; set; }
    }

}
