using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Application.Interfaces
{
    public interface IHistoriasService
    {
        Task<List<ContadorPendientes>> ObtenerContadoresBandejaAsync();
        Task<AtencionDto> GetAtencionByIdAsync(string token, int atencionId);
        Task<HistoriaClinicaConsultaDto> ConsultarPacienteAsync(string historia);
        Task<IEnumerable<HistoriaClinicaDto>> ConsultarHistoriasAsync(HistoriaFiltro filtro);
        Task<IEnumerable<AtencionHcDto>> ConsultarAtencionesEmergenciaAsync(string historia);
        Task<IEnumerable<EvaluacionHcDto>> ConsultarEvaluacionesEmergenciaAsync(int atencion_id);

        Task<List<Atencion>> GetAtencionAsync(int idAtencion);
        Task<AtencionDto> GetAtencionOrdenesByIdAsync(string token, int atencionId);
        Task<AtencionDto> GetAtencionCuentasByIdAsync(string token, int atencionId);

        //Task<AtencionDto> ObtenerAtencionPorIdAsync(int atencionId);
        Task<List<AuditorDto>> ObtenerListaUsuariosAsync();       
        Task<TriajeResponseDto> ActualizarTriaje(ActualizarTriajeRequest request);
        Task<bool> AsignarAtencionesAsync();
      


    }
}
