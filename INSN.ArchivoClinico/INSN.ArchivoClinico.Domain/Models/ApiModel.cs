namespace INSN.ArchivoClinico.Domain.Models;

public class AtencionFiltro
{
    public string? ModuloAuditoria { get; set; }
    public int? TipoServicio { get; set; }
    public DateTime? Fecha { get; set; }
    public string? HistoriaClinica { get; set; }
    public string? NroCuenta { get; set; }
    public string? DocumentoIdentidad { get; set; }
    public string? Nombre { get; set; }
    public int? CodigoEstado { get; set; } = 0;
    public string? Usuario { get; set; }
    public bool? HabilitarFecha { get; set; } = false; 
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int? Asignado { get; set; } = 0;
}


#region Cargar Pantalla

public class AtencionModel
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
    public IEnumerable<DiagnosticoModel>? Diagnosticos { get; set; }
    public IEnumerable<ExamenAuxiliarModel>? ExamenesAuxiliares { get; set; }
    public IEnumerable<MedicamentoModel>? Medicamentos { get; set; }
    public IEnumerable<ExamenProcedimientoModel>? ExamenesProcedimientos { get; set; }
}


public class DiagnosticoModel
{
    public long IdAtencionDiagnostico { get; set; }
    public int NumeroDiagnostico { get; set; }
    public string? Cie10 { get; set; }
    public string? Diagnostico { get; set; }
    public string? TipoDiagnostico { get; set; }
    public string? CategoriaDiagnostico { get; set; }
}

public class ExamenAuxiliarModel
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

public class MedicamentoModel
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

public class ExamenProcedimientoModel
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

public class AtencionHceModel
{
    public int AtencionId { get; set; }
    public string? NroHistoria { get; set; }
    public long? NroCuenta { get; set; }
    public int? CodMedico { get; set; }
    public string? PacNomb { get; set; }
    public string? NombreMedico { get; set; }
    public string? NombreServicio { get; set; }
    public string? Fecha { get; set; }
    public int IdEstado { get; set; }
    public string? Estado { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    
}

public class ItemAuditoriaModel
{
    public string? AuditoriaPuntoCarga { get; set; }
    public int IdItem { get; set; }
    public int? CodigoGrupoItem { get; set; }
    public string? CodigoItem { get; set; }
    public string? Item { get; set; }
    public string? Cie10 { get; set; }
    public int? AuditoriaCantidad { get; set; }
    public DateTime? AuditoriaFecha { get; set; }
    public string? AuditoriaObservacion { get; set; }
    public int IdEstadoAuditoria { get; set; }
    public string? EstadoAuditoria { get; set; }
    public string? AuditoriaUsuario { get; set; }

}

public class ResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; } // Puedes cambiar el tipo si necesitas datos específicos
}

public class AtencionCEAuditoriaModel
{
    public long AtencionId { get; set; }
    public string? AuditoriaUsuario { get; set; }
}

public class AtencionesInformacionPaginadoCEModel
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