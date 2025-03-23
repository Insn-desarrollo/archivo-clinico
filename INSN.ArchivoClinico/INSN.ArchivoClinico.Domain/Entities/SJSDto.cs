using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Domain.Entities
{
    public class Atencion
    {
        public long? idAtencion { get; set; }
        public string loteFua { get; set; }
        public string nroFua { get; set; }
        public string renipress { get; set; }
        public string idCategoria { get; set; }
        public string nivel { get; set; }
        public int? idPuntoDigitacion { get; set; }
        public string idComponente { get; set; }
        public string idDisaAsegurado { get; set; }
        public string idLoteAsegurado { get; set; }
        public string idCorrelativoAsegurado { get; set; }
        public string idSecuenciaAsegurado { get; set; }
        public string idTablaAsegurado { get; set; }
        public string idContratoAsegurado { get; set; }
        public string idPlan { get; set; }
        public string idGrupoPoblacional { get; set; }
        public string idTipoDocAsegurado { get; set; }
        public string numDocAsegurado { get; set; }
        public string apePaterno { get; set; }
        public string apeMaterno { get; set; }
        public string nombres { get; set; }
        public DateTime? fecNac { get; set; }
        public string idSexo { get; set; }
        public string idUbigeo { get; set; }
        public string historiaClinica { get; set; }
        public string idTipoAtencion { get; set; }
        public string idCondicionMaterna { get; set; }
        public string idModalidadAtencion { get; set; }
        public string nroAutorizacion { get; set; }
        public int? montoAutorizado { get; set; }
        public DateTime? fecHoraAtencion { get; set; }
        public string renipressReferencia { get; set; }
        public string nroHojaReferencia { get; set; }
        public string idServicio { get; set; }
        public string idOrigenPersonal { get; set; }
        public string idLugarAtencion { get; set; }
        public string idDestinoAsegurado { get; set; }
        public DateTime? fecIngresoHospitalizacion { get; set; }
        public DateTime? fecAltaHospitalizacion { get; set; }
        public string renipressContraReferencia { get; set; }
        public string nroHojaContraReferencia { get; set; }
        public DateTime? fecParto { get; set; }
        public string idGrupoRiesgo { get; set; }
        public DateTime? fecFallecimiento { get; set; }
        public string renipressOfertaFlexible { get; set; }
        public string idEtnia { get; set; }
        public string idIafas { get; set; }
        public string idCodigoIafas { get; set; }
        public string idUps { get; set; }
        public DateTime? fecCorteAdministrativo { get; set; }
        public string idUdrAutorizaVinculado { get; set; }
        public string loteAutorizaVinculado { get; set; }
        public string nroAutorizaVinculado { get; set; }
        public string disaFuaVinculado { get; set; }
        public string loteFuaVinculado { get; set; }
        public string nroFuaVinculado { get; set; }
        public string idTipoDocRespAte { get; set; }
        public string numDocRespAte { get; set; }
        public string idTipoPersonalSalud { get; set; }
        public string idEspecialidadRespAte { get; set; }
        public string esEgresadoRespAte { get; set; }
        public string colegiaturaRespAte { get; set; }
        public string rneRespAte { get; set; }
        public string idTipoDocDigitador { get; set; }
        public string numDocDigitador { get; set; }
        public DateTime? fecHoraRegistro { get; set; }
        public string observacion { get; set; }
        public string versionAplicativo { get; set; }
        public string codigoAcreditacion { get; set; }
        public DateTime? fecHoraIniFuaAdm { get; set; }
        public DateTime? fecHoraFinFuaAdm { get; set; }
        public int? idMotivoIngresoCasaMaterna { get; set; }
        public int? idCasaMaterna { get; set; }
        public int? idEstado { get; set; }
        public bool? esObservado { get; set; }
        public control control { get; set; }
        public List<AtDiagnostico> atDiagnosticos { get; set; }
        public List<AtInsumo> atInsumos { get; set; }
        public List<AtMedicamento> atMedicamentos { get; set; }
        public List<AtProcedimiento> atProcedimientos { get; set; }
        public List<AtRecienNacido> atRecienNacidos { get; set; }
        public List<AtServAdicionales> atServAdicionales { get; set; }
        public List<AtServMatInfantil> atServMatInfantiles { get; set; }
        public List<AtTransporte> atTransportes { get; set; }
        public List<AtViatico> atViaticos { get; set; }
        public List<AtOtrosGasto> atOtrosGastos { get; set; }
    }

    public class control
    {
        public string idControl { get; set; }
        public int idProceso { get; set; }
        public DateTime? fecHoraCrea { get; set; }
        public int idUsuarioCrea { get; set; }
        public string observacion { get; set; }
    }

    public class AtDiagnostico
    {
        public string codigo { get; set; }
        public int nroDiagnostico { get; set; }
        public string tipoMovimiento { get; set; }
        public int tipoDiagnostico { get; set; }
    }

    public class AtInsumo
    {
        public string codigo { get; set; }
        public int nroDiagnostico { get; set; }
        public int cantPrescrita { get; set; }
        public int cantEntregada { get; set; }
        public string lote { get; set; }
        public string nroSerie { get; set; }
        public string registroSanitario { get; set; }
        public string fecVencimiento { get; set; }
        public string contieneOtrosDatos { get; set; }
    }

    public class AtMedicamento
    {
        public string codigo { get; set; }
        public int nroDiagnostico { get; set; }
        public int cantPrescrita { get; set; }
        public int cantEntregada { get; set; }
        public DateTime fecPetitorio { get; set; }
        public string nroDocPetitorio { get; set; }
        public string lote { get; set; }
        public string nroSerie { get; set; }
        public string registroSanitario { get; set; }
        public string fecVencimiento { get; set; }
        public string contieneOtrosDatos { get; set; }
    }

    public class AtProcedimiento
    {
        public string codigo { get; set; }
        public int nroDiagnostico { get; set; }
        public int cantPrescrita { get; set; }
        public int cantEntregada { get; set; }
        public string resultado { get; set; }
    }

    public class AtRecienNacido
    {
        public int nroRN { get; set; }
        public string tipoDocumento { get; set; }
        public string nroDocumento { get; set; }
        public string disaAfiliacion { get; set; }
        public string formatoContratoAfil { get; set; }
        public string nroContratoAfil { get; set; }
        public string secuenciaContAfil { get; set; }
        public string apePaterno { get; set; }
        public string apeMaterno { get; set; }
        public string primerNombre { get; set; }
        public string segundoNombre { get; set; }
        public int identificadorRegAfil { get; set; }
        public string identificadorTabla { get; set; }
    }

    public class AtServAdicionales
    {
        public string codigo { get; set; }
    }

    public class AtServMatInfantil
    {
        public string codigo { get; set; }
        public string resultado { get; set; }
    }

    public class AtTransporte
    {
        public int codigo { get; set; }
        public int nroPasajeros { get; set; }
        public int cantidad { get; set; }
        public int precio { get; set; }
        public int total { get; set; }
    }

    public class AtViatico
    {
        public int codigo { get; set; }
        public int nroDias { get; set; }
        public int nroComisionados { get; set; }
        public int total { get; set; }
    }

    public class AtOtrosGasto
    {
        public int codigo { get; set; }
        public int cantidad { get; set; }
        public int nroDias { get; set; }
        public int precio { get; set; }
        public int total { get; set; }
    }


    public class ResponseData
    {
        public bool Success { get; set; }
        public List<string> Mensaje { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string Token { get; set; }
        public string Fecha { get; set; }
    }
    public class RespuestaEmitirFua
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("mensaje")]
        public List<string> Mensaje { get; set; }

        [JsonPropertyName("data")]
        public DataFua Data { get; set; }
    }

    public class DataFua
    {
        [JsonPropertyName("guid")]
        public string? Guid { get; set; }

        [JsonPropertyName("errores")]
        public List<DetalleMensaje> Errores { get; set; }

        [JsonPropertyName("advertencias")]
        public List<DetalleMensaje> Advertencias { get; set; }
    }

    public class DetalleMensaje
    {
        [JsonPropertyName("ruta")]
        public string Ruta { get; set; }

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }
    }

    public class consultaFua
    {
        public string guid { get; set; }
    }


    public class RespuestaConsultarFua
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("mensaje")]
        public List<string> Mensaje { get; set; }

        [JsonPropertyName("data")]
        public List<DataItem> Data { get; set; }
    }

    public class DataItem
    {
        [JsonPropertyName("guid")]
        public string? Guid { get; set; }

        [JsonPropertyName("idAtencion")]
        public int? IdAtencion { get; set; }

        [JsonPropertyName("idEstado")]
        public string? IdEstado { get; set; }

        [JsonPropertyName("fecHoraCrea")]
        public string? FecHoraCrea { get; set; }

        [JsonPropertyName("errores")]
        public List<ErrorItem> Errores { get; set; }
    }

    public class ErrorItem
    {
        //[JsonPropertyName("idProceso")]
        //public string IdProceso { get; set; }

        [JsonPropertyName("rutaError")]
        public string RutaError { get; set; }

        [JsonPropertyName("mensajeError")]
        public string MensajeError { get; set; }
    }

}
