using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Domain.Interfaces
{
    public interface IAtencionRepository
    {
        Task<List<ContadorPendientes>> ObtenerContadoresBandejaAsync();
        Task<IEnumerable<AtencionConsultaDto>> ConsultarTriajesAsync(AtencionFiltro filtro);      
        Task<IEnumerable<AtencionConsultaDto>> ConsultarAdminAsync(AtencionFiltro filtro);
        //Task<AtencionDto> ObtenerAtencionPorIdAsync(int atencionId);    
        Task<List<AuditorDto>> ObtenerListaAuditoresAsync();          
        Task<TriajeResponseDto> ActualizarTriajeAsync(ActualizarTriajeRequest request);      
        Task<bool> AsignarAtencionesAsync();      
        Task<long> RegistrarFuaAsync(Atencion atencion);

    }
}
