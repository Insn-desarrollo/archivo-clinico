using INSN.ArchivoClinico.Application.DTOs;
using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using INSN.ArchivoClinico.Domain.UtilFactory.Base;

namespace INSN.ArchivoClinico.Application.Interfaces
{
    public interface IAtencionHceService
    {
        Task<bool> SincronizarAtencionesAsignadasAsync(string token, string usuario);
        Task<bool> SincronizarEvaluacionesAsignadasAsync(string token, string usuario);
        Task<bool> ActualizarAuditCuentaAtencionAsync(TriajeResponseDto TriajeResponseDto, string Token);

        Task<PagedResponse<AtencionHceModel>> ObtenerAtencionesAsync(string ModuloAuditoria, int tipoConsulta, DateTime fecha, string historiaClinica, string nombre, int page, int pageSize, string token);
        Task<AtencionModel> ObtenerDetalleAtencionAsync(int id, string token);
        Task<ItemAuditoriaDto> ObtenerDetalleItemAuditoriaAsync(int id, string puntoCarga, string token);
        Task<ExamenAuxiliarDto> ObtenerDetalleExamenAuxiliarAsync(int id, string token);
        Task<bool> ActualizarAtencionAuditoriaAsync(AtencionCEAuditoriaDto atencionCEAuditoriaDto);
        Task<bool> ActualizarItemAuditoriaAsync(ItemAuditoriaDto itemAuditoriaDto);


    }
}
