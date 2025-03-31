using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Application.Interfaces
{
    public interface ICuentaService
    {
        Task<List<FuenteFinanciamientoDto>> ObtenerFuentesFinanciamientoAsync();
        Task<List<TipoFinanciamientoDto>> ObtenerTiposFinanciamientoAsync(int fuenteFinanciamientoId);        
        Task<TriajeResponseDto> ActualizarCuenta(ActualizarCuentaRequest request);
        Task<TriajeResponseDto> ActualizarCuentaFUA(ActualizarRespuestaFUARequest request);
        
    }
}
