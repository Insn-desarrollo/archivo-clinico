using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Interfaces;
using INSN.ArchivoClinico.Domain.Models;

namespace INSN.ArchivoClinico.Infrastructure.Services
{
    public class CuentaService : ICuentaService
    {
        private readonly ICuentaRepository _cuentaRepository;

        public CuentaService(ICuentaRepository cuentaRepository)
        {
            _cuentaRepository = cuentaRepository;
        }

        public async Task<List<FuenteFinanciamientoDto>> ObtenerFuentesFinanciamientoAsync()
        {
            return await _cuentaRepository.ObtenerFuentesFinanciamientoAsync();
        }

        public async Task<List<TipoFinanciamientoDto>> ObtenerTiposFinanciamientoAsync(int fuenteFinanciamientoId)
        {
            return await _cuentaRepository.ObtenerTiposFinanciamientoAsync(fuenteFinanciamientoId);
        }

        public async Task<TriajeResponseDto> ActualizarCuenta(ActualizarCuentaRequest request)
        {
            return await _cuentaRepository.ActualizarCuentaAsync(request);
        }

        public async Task<TriajeResponseDto> ActualizarCuentaFUA(ActualizarRespuestaFUARequest request)
        {
            return await _cuentaRepository.ActualizarCuentaFUAAsync(request);
        }

    }
       

}
