using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Application.Interfaces
{
    public interface IAtencionService
    {
        Task<List<ContadorPendientes>> ObtenerContadoresBandejaAsync();
        Task<AtencionDto> GetAtencionByIdAsync(string token, int atencionId);
        Task<IEnumerable<AtencionConsultaDto>> ConsultarTriajesAsync(AtencionFiltro filtro);
        Task<IEnumerable<AtencionConsultaDto>> ConsultarAdminAsync(AtencionFiltro filtro);
        Task<List<Atencion>> GetAtencionAsync(int idAtencion);
        Task<AtencionDto> GetAtencionOrdenesByIdAsync(string token, int atencionId);
        Task<AtencionDto> GetAtencionCuentasByIdAsync(string token, int atencionId);

        //Task<AtencionDto> ObtenerAtencionPorIdAsync(int atencionId);
        Task<List<AuditorDto>> ObtenerListaAuditoresAsync();       
        Task<TriajeResponseDto> ActualizarTriaje(ActualizarTriajeRequest request);
        Task<bool> AsignarAtencionesAsync();
      


    }
}
