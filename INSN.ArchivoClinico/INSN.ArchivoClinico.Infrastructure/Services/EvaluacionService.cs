using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Interfaces;
using INSN.ArchivoClinico.Domain.Models;

namespace INSN.ArchivoClinico.Infrastructure.Services
{
    public class EvaluacionService : IEvaluacionService
    {
        private readonly IEvaluacionRepository _evaluacionRepository;

        public EvaluacionService(IEvaluacionRepository evaluacionRepository)
        {
            _evaluacionRepository = evaluacionRepository;
        }

        public async Task<IEnumerable<AtencionConsultaDto>> ConsultarOrdenesAsync(AtencionFiltro filtro)
        {
            var atenciones = await _evaluacionRepository.ConsultarOrdenesAsync(filtro);
            return atenciones;
        }     

        public async Task<List<EvaluacionMinDto>> ObtenerEvaluacionesPorAtencionAsync(int atencionId)
        {
            return await _evaluacionRepository.ObtenerEvaluacionesPorAtencionAsync(atencionId);
        }        

        public async Task<EvaluacionDto> ObtenerDetalleEvaluacionAsync(long evaluacionEessId)
        {           
            try
            {
                var evaluacion = await _evaluacionRepository.ObtenerEvaluacionPorIdAsync(evaluacionEessId);
                int _EvaluacionEESSId = 0;
                if (int.TryParse(evaluacion.evaluacion_eess_id.ToString(), out _EvaluacionEESSId))
                {
                    var diagnosticos = await _evaluacionRepository.ObtenerDiagnosticosPorEvaluacionAsync(_EvaluacionEESSId);
                    var medicamentos = await _evaluacionRepository.ObtenerMedicamentosPorEvaluacionAsync(_EvaluacionEESSId);
                    var procedimientos = await _evaluacionRepository.ObtenerProcedimientosPorEvaluacionAsync(_EvaluacionEESSId);
                    evaluacion.Diagnosticos = diagnosticos;
                    evaluacion.Medicamentos = medicamentos;
                    evaluacion.ExamenesAuxiliares = procedimientos;
                }                

                return evaluacion;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ItemAuditoriaDto> ObtenerItemAuditoriaIdAsync(int evaluacionId, string puntoCarga)
        {
            var itemAuditoria = await _evaluacionRepository.ObtenerItemAuditoriaIdAsync(evaluacionId, puntoCarga);
            return itemAuditoria;
        }

        public async Task<bool> ActualizarAuditoriaAsync(ActualizarAuditoriaRequest request)
        {
            return await _evaluacionRepository.ActualizarAuditoriaAsync(request);
        }

        public async Task<bool> AceptacionMasivaItemsEvaluacion(AceptarMasivaEvaluacionRequest request)
        {
            return await _evaluacionRepository.AceptacionMasivaItemsEvaluacion(request);
        }

        public async Task<bool> ActualizarEnvioReceta(EnviaOrdenRequest request)
        {
            return await _evaluacionRepository.ActualizarEnvioReceta(request);
        }

    }
       

}
