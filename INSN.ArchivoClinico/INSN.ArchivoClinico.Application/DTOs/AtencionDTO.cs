using INSN.ArchivoClinico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Application.DTOs
{
    //public class AtencionConsultaDto
    //{
    //    public int AtencionId { get; set; }
    //    public int? CuentaAtencionId { get; set; }
    //    public int? CodigoTipoServicio { get; set; }
    //    public string? TipoServicio { get; set; }
    //    public string? HistoriaClinica { get; set; }
    //    public int? CodigoTipoDocumento { get; set; }
    //    public string? TiposDocumento { get; set; }
    //    public string? NumeroDocumento { get; set; }
    //    public string? ApellidoPaterno { get; set; }
    //    public string? ApellidoMaterno { get; set; }
    //    public string? Nombres { get; set; }
    //    public string? FechaNacimiento { get; set; }
    //    public int? CodigoTipoSexo { get; set; }
    //    public string? FechaIngresoAtencion { get; set; }
    //    public int? ServicioIngresoEESSId { get; set; }
    //    public string? ServicioIngresoEESS { get; set; }
    //    public int? AuditoriaCodigoEstado { get; set; }
    //    public string? AuditoriaEstado { get; set; }
    //    public int? AuditoriaTriajeCodigoEstado { get; set; }
    //    public string? AuditoriaTriajeEstado { get; set; }
    //    public string? AuditoriaObservacion { get; set; }
    //    public int? TotalRecords { get; set; }
    //    public int? TotalPages { get; set; }
    //}
    //public class Atencion
    //{
    //    public int AtencionId { get; set; }
    //    public int CuentaAtencionId { get; set; }
    //    public int CodigoTipoServicio { get; set; }
    //    public string? TipoServicio { get; set; }
    //    public string? HistoriaClinica { get; set; }
    //    public int CodigoTipoDocumento { get; set; }
    //    public string? TiposDocumento { get; set; }
    //    public string? NumeroDocumento { get; set; }
    //    public string? ApellidoPaterno { get; set; }
    //    public string? ApellidoMaterno { get; set; }
    //    public string? Nombres { get; set; }
    //    public string? FechaNacimiento { get; set; }
    //    public int? CodigoTipoSexo { get; set; }
    //    public string? TipoSexo { get; set; }
    //    public DateTime? FechaIngresoAtencion { get; set; }
    //    public string? FechaIngresoAtencionStr { get; set; }
    //    public string? HoraIngresoAtencion { get; set; }
    //    public int? ServicioIngresoEESSId { get; set; }
    //    public string? ServicioIngresoEESS { get; set; }
    //    public string? MedicoingresoEESS { get; set; }
    //    public int? AuditoriaCodigoEstado { get; set; }
    //    public string? AuditoriaEstado { get; set; }
    //    public int? AuditoriaTriajeCodigoEstado { get; set; }
    //    public string? AuditoriaTriajeEstado { get; set; }
    //    public string? AuditoriaObservacion { get; set; }
    //    public int? FuenteFinanciamientoId { get; set; }
    //    public int? TipoFinanciamientoId { get; set; }
    //    public string? SisAseguradoComponente { get; set; }
    //    public string? SisAseguradoDisa { get; set; }
    //    public string? SisAseguradoLote { get; set; }
    //    public string? SisAseguradoNumero { get; set; }
    //    public string? SisAseguradoCorrelativo { get; set; }
    //    public string? SisAseguradoTipoTabla { get; set; }
    //    public string? SisAseguradoIdentificador { get; set; }

    //    // Propiedades para listas
    //    public List<CmbIdDescripcion>? TiposFinanciamiento { get; set; }
    //    public List<FuenteFinanciamientoDto>? FuentesFinanciamiento { get; set; }
    //}

    //public class CmbIdDescripcion
    //{
    //    public int Id { get; set; }
    //    public string? Descripcion { get; set; }
    //}

    #region Cargar Pantalla

    public class AtencionDto
    {
        public int IdAtencion { get; set; }
        public string? HistClin { get; set; }
        public string? ApellidosNombres { get; set; }
        public string? DNI { get; set; }
        public string? Sexo { get; set; }
        public string? Edad { get; set; }
        public string? Fecha { get; set; }
        public string? Servicio { get; set; }
        public string? Medico { get; set; }
        public string? TipoFinanciamiento { get; set; }
        public string? Antecedentes { get; set; }
        public int? IdEstado { get; set; }
        public string? Estado { get; set; }
        public IEnumerable<DiagnosticoDto>? Diagnosticos { get; set; }
        public IEnumerable<ExamenAuxiliarDto>? ExamenesAuxiliares { get; set; }
        public IEnumerable<MedicamentoDto>? Medicamentos { get; set; }
        public IEnumerable<ExamenProcedimientoDto>? ExamenesProcedimientos { get; set; }
    }


    public class DiagnosticoDto
    {
        public long IdAtencionDiagnostico { get; set; }
        public int NumeroDiagnostico { get; set; }
        public string? Cie10 { get; set; }
        public string? Diagnostico { get; set; }
        public string? TipoDiagnostico { get; set; }
        public string? CategoriaDiagnostico { get; set; }
    }

    public class ExamenAuxiliarDto
    {
        public long IdAtencionExamenAux { get; set; }
        public int CodigoGrupoExamenAux { get; set; }
        public string? GrupoExamenAux { get; set; }
        public string? CodigoExamenAux { get; set; }
        public string? ExamenAux { get; set; }
        public string? Cie10 { get; set; }
        public int Cantidad { get; set; }
        public string? CodigoSis { get; set; }
        public decimal Costo { get; set; }
        public int IdEstadoAuditoria { get; set; }
        public string? EstadoAuditoria { get; set; }
        public string? Auditor { get; set; }
        public DateTime? AuditoriaFecha { get; set; }
        public string? AuditoriaObservacion { get; set; }
        public int AuditoriaCantidad { get; set; }
        public int IdEstadoAuditoriaAtencion { get; set; }
    }

    public class MedicamentoDto
    {
        public long IdAtencionMedicamento { get; set; }
        public string? CodigoMedicamento { get; set; }
        public string? CodigoSisMed { get; set; }
        public string? Medicamento { get; set; }
        public string? Presentacion { get; set; }
        public string? Cie10 { get; set; }
        public string? Dosis { get; set; }
        public int? Frecuencia { get; set; }
        public int? Dias { get; set; }
        public int Cantidad { get; set; }
        public decimal Costo { get; set; }
        public string? Indicaciones { get; set; }
        public int IdEstadoAuditoria { get; set; }
        public string? EstadoAuditoria { get; set; }
        public string? Auditor { get; set; }
        public DateTime? AuditoriaFecha { get; set; }
        public string? AuditoriaObservacion { get; set; }
        public int AuditoriaCantidad { get; set; }
        public int IdEstadoAuditoriaAtencion { get; set; }
    }

    public class ExamenProcedimientoDto
    {
        public long IdAtencionProcedimiento { get; set; }
        public string? CodigoSis { get; set; }
        public int CodigoGrupoExamenProcedimiento { get; set; }
        public string GrupoProcedimiento { get; set; } = "PROCEDIMIENTO";
        public string? CodigoExamenProcedimiento { get; set; }
        public string? Procedimiento { get; set; }
        public int Cantidad { get; set; }
        public decimal Costo { get; set; }
        public string? Cie10 { get; set; }
        public int AtencionId { get; set; }
        public int IdEstadoAuditoria { get; set; }
        public string? EstadoAuditoria { get; set; }
        public string? Auditor { get; set; }
        public DateTime? AuditoriaFecha { get; set; }
        public string? AuditoriaObservacion { get; set; }
        public int AuditoriaCantidad { get; set; }
        public int IdEstadoAuditoriaAtencion { get; set; }
    }

    #endregion

 

    

    public class ResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; } // Puedes cambiar el tipo si necesitas datos específicos
    }

    public class AtencionCEAuditoriaDto
    {
        public long AtencionId { get; set; }
        public string? AuditoriaUsuario { get; set; }
        public string? Token { get; set; }
    }

    public class AtencionesInformacionPaginadoCEDTO
    {
        public long AtencionId { get; set; }
        public long TriajeId { get; set; }
        public long PacienteId { get; set; }
        public long? CuentaAtencionId { get; set; }
        public string? HistoriaClinica { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? ServicioIngresoId { get; set; }
        public int? DestinoAtencionId { get; set; }
        public string? ServicioIngreso { get; set; }
        public int ProfesionalId { get; set; }
        public string? ProfesionalNombre { get; set; }
        public int AuditoriaCodigoEstado { get; set; }
        public string? AuditoriaEstado { get; set; }
        public string? AuditoriaUsuarioAsignado { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }


}
