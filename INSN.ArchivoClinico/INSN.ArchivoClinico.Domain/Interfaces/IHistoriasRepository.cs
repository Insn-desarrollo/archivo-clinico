using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Domain.Interfaces
{
    public interface IHistoriasRepository
    {
        Task<List<ContadorPendientes>> ObtenerContadoresBandejaAsync();
        Task<IEnumerable<HistoriaClinicaDto>> ConsultarHistoriasAsync(HistoriaFiltro filtro);
        Task<HistoriaClinicaConsultaDto> ConsultarPacienteAsync(string historia);
        Task<IEnumerable<AtencionHcDto>> ConsultarAtencionesEmergenciaAsync(string historia);
        Task<IEnumerable<EvaluacionHcDto>> ConsultarEvaluacionesEmergenciaAsync(int atencion_id);
        Task<IEnumerable<DocumentosEvaluacionHC>> ConsultaDocumentoAtencion(int atencion_id);
        Task<IEnumerable<DocumentosEvaluacionHC>> ConsultaDocumentoEvaluacion(int evaluacion_id);

        Task<List<AuditorDto>> ObtenerListaUsuariosAsync();          
        Task<TriajeResponseDto> ActualizarTriajeAsync(ActualizarTriajeRequest request);      
        Task<bool> AsignarAtencionesAsync();      
        Task<long> RegistrarFuaAsync(Atencion atencion);

    }
}
