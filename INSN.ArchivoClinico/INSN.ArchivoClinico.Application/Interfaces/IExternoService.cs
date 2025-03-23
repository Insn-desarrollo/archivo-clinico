using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Application.Interfaces
{
    public interface IExternoService
    {
        Task<bool> SincronizarEvaluacionesPorIdAsync(string token, int atencionId);
        Task<bool> SincronizarAtencionesSinCuentaAsync(string token);
        Task<bool> ActualizarAuditoriaEmergenciaAsync(string token, EstadoAuditoriaModel model);
        Task<bool> ActualizarAuditoriaEmergenciaItemAsync(string token, ActualizarAuditoriaItemRequest model);
        Task<bool> ActualizarMasivoAuditoriaEmergenciaItemAsync(string token, AceptarMasivaEvaluacionRequest itemAuditoriaDto);
        Task<bool> RegistrarRecetaOrden_ApoyoDx_Async(string token, EnviaOrdenRequest itemAuditoriaDto);
        Task<AfiliadoSISDto> BuscarAfiliadoSISAsync(string s_tipobus, string s_numero);
        Task<List<BoletaResultado>> ObtenerBoletaAtencionAsync(string nroHistoria, string nroBoleta, string nombPaciente, string token);
        Task<bool> ActualizarEmergenciaEstadoFuaAsync(string token, ActualizarRespuestaFUARequest model);

    }
}
