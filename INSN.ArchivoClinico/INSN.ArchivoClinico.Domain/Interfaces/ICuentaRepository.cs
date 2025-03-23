using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;

namespace INSN.ArchivoClinico.Domain.Interfaces
{
    public interface ICuentaRepository
    {     
        Task<IEnumerable<AtencionConsultaDto>> ConsultarCuentasAsync(AtencionFiltro filtro);
        Task<IEnumerable<AtencionConsultaDto>> ConsultarSISAsync(AtencionFiltro filtro);
        Task<List<FuenteFinanciamientoDto>> ObtenerFuentesFinanciamientoAsync();    
        Task<List<TipoFinanciamientoDto>> ObtenerTiposFinanciamientoAsync(int fuenteFinanciamientoId);    
        Task<TriajeResponseDto> ActualizarCuentaAsync(ActualizarCuentaRequest request);
        Task<TriajeResponseDto> ActualizarCuentaFUAAsync(ActualizarRespuestaFUARequest request);   

    }
}
