using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Application.Interfaces
{
    public interface IEvaluacionService
    {
        Task<List<EvaluacionMinDto>> ObtenerEvaluacionesPorAtencionAsync(int atencionId);   
        Task<EvaluacionDto> ObtenerDetalleEvaluacionAsync(long EvaluacionId);
        Task<ItemAuditoriaDto> ObtenerItemAuditoriaIdAsync(int evaluacionId, string puntoCarga);
        Task<bool> ActualizarAuditoriaAsync(ActualizarAuditoriaRequest request);
        Task<bool> AceptacionMasivaItemsEvaluacion(AceptarMasivaEvaluacionRequest request);
        Task<bool> ActualizarEnvioReceta(EnviaOrdenRequest request);
    }
}
