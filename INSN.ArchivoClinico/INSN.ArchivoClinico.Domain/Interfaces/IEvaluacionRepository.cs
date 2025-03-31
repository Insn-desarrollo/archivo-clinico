using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Domain.Interfaces
{
    public interface IEvaluacionRepository
    {          
        Task<List<EvaluacionMinDto>> ObtenerEvaluacionesPorAtencionAsync(long atencionId);   
        Task<bool> ActualizarAuditoriaAsync(ActualizarAuditoriaRequest request);
        Task<bool> AceptacionMasivaItemsEvaluacion(AceptarMasivaEvaluacionRequest request);
        Task<bool> ActualizarEnvioReceta(EnviaOrdenRequest request);

        Task<List<EvaluacionDiagnosticoDto>> ObtenerDiagnosticosPorEvaluacionAsync(long evaluacionEessId);
        Task<List<EvaluacionMedicamentoDto>> ObtenerMedicamentosPorEvaluacionAsync(long evaluacionEessId);
        Task<List<EvaluacionProcedimientoDto>> ObtenerProcedimientosPorEvaluacionAsync(long evaluacionEessId);
        Task<EvaluacionDto> ObtenerEvaluacionPorIdAsync(long evaluacionEessId);
        Task<ItemAuditoriaDto> ObtenerItemAuditoriaIdAsync(long evaluacionId, string puntoCarga);

    }
}
