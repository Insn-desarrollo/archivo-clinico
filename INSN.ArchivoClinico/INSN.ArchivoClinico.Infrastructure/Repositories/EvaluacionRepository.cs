using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Interfaces;
using INSN.ArchivoClinico.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql; // Cliente para PostgreSQL
using Microsoft.Extensions.Configuration;
using INSN.ArchivoClinico.Domain.Models;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
//using INSN.ArchivoClinico.Application.DTOs;

namespace INSN.ArchivoClinico.Infrastructure.Repositories
{
    public class EvaluacionRepository : IEvaluacionRepository
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;
        public EvaluacionRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
           


        public async Task<EvaluacionDto> ObtenerEvaluacionPorIdAsync(long evaluacionEessId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"   SELECT 
                    ev.evaluacion_id,
                    ev.evaluacion_eess_id,
		            ev.fecha_evaluacion,
		            ev.hora_evaluacion,
                    ev.servicio_eess,
                    COALESCE(ev.auditoria_codigo_estado, 0) AS auditoria_codigo_estado,
                    COALESCE(ev.auditoria_observacion, '') AS auditoria_observacion,
                    COALESCE(ev.codigo_estado_receta, 1) AS codigo_estado_receta,
                    COALESCE(ev.codigo_estado_orden, 1) AS codigo_estado_orden,
                    COALESCE(TB1.tablamaestradetalle, 'Pendiente') AS receta_estado,
                    COALESCE(TB2.tablamaestradetalle, 'Pendiente') AS orden_estado
                FROM transaccional.evaluacion ev
                LEFT JOIN (
                    SELECT TMD.codigo, TMD.tablamaestradetalle
                    FROM sistema.tabla_maestra_detalle TMD
                    INNER JOIN sistema.tabla_maestra TM ON TM.codigo = TMD.codigotablamaestra
                    WHERE TM.tablamaestra = 'EstadoReceta'
                ) AS TB1 ON ev.codigo_estado_receta = TB1.codigo
                LEFT JOIN (
                    SELECT TMD.codigo, TMD.tablamaestradetalle
                    FROM sistema.tabla_maestra_detalle TMD
                    INNER JOIN sistema.tabla_maestra TM ON TM.codigo = TMD.codigotablamaestra
                    WHERE TM.tablamaestra = 'EstadoOrden'
                ) AS TB2 ON ev.codigo_estado_orden = TB2.codigo
                WHERE ev.evaluacion_eess_id = @evaluacion_eess_id;"
            ;
            var parameters = new { evaluacion_eess_id = evaluacionEessId };

            var atencion = await connection.QueryFirstOrDefaultAsync<EvaluacionDto>(query, parameters);

            //var resultados = await connection.QueryAsync<AtencionConsultaDto>(sql, parametros);

            return atencion;
        }


        public async Task<List<EvaluacionDiagnosticoDto>> ObtenerDiagnosticosPorEvaluacionAsync(long evaluacioneessId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                       SELECT 
                            ed.numero_orden,
                            ed.codigo_tipo_diagnostico,
                            TB.tablamaestradetalle AS tipo_diagnostico,
                            ed.codigo_diagnostico,
                            ed.diagnostico,
                            ed.es_principal
                        FROM 
                            transaccional.evaluacion_diagnostico ed
                        LEFT JOIN (
                            SELECT 
                                TMD.codigo, 
                                TMD.tablamaestradetalle
                            FROM 
                                sistema.tabla_maestra_detalle TMD
                            INNER JOIN 
                                sistema.tabla_maestra TM ON TM.codigo = TMD.codigotablamaestra
                            WHERE 
                                TM.tablamaestra = 'TiposDiagnostico'
                        ) AS TB ON ed.codigo_tipo_diagnostico = TB.codigo
                        WHERE 
                            ed.evaluacion_eess_id = @evaluacion_id
                            AND ed.es_eliminado = FALSE
                        ORDER BY ed.numero_orden ASC;"
            ;
            var parameters = new { evaluacion_id = evaluacioneessId };
            var result = await connection.QueryAsync<EvaluacionDiagnosticoDto>(query, parameters);
            var resultados = result.ToList();
            return resultados;
        }

        public async Task<List<EvaluacionMedicamentoDto>> ObtenerMedicamentosPorEvaluacionAsync(long evaluacioneessId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                        SELECT 
                a.numero_orden,
                a.evaluacion_medicamento_id,
                a.codigo_medicamento,
                COALESCE(a.codigo_sismed, '') AS codigo_sismed,
                a.medicamento,
                COALESCE(a.dosis, '') AS dosis,
                COALESCE(a.frecuencia, '') AS frecuencia,
                COALESCE(a.dias, '') AS dias,
                COALESCE(a.indicaciones, '') AS indicaciones,
		        COALESCE(a.cantidad_entregada, 0) AS cantidad_entregada,
		        COALESCE(a.cantidad_prescrita, 0) AS cantidad_prescrita,
		        COALESCE(a.auditoria_cantidad, 0) AS auditoria_cantidad,	
		        COALESCE(a.auditoria_orden_subsana_obs, FALSE) AS auditoria_orden_subsana_obs,
		        a.auditoria_orden_subsana_obs_texto,	
                COALESCE(a.codigo_diagnostico, '') AS codigo_diagnostico,		
                a.auditoria_codigo_estado,
		        TB.tablamaestradetalle AS auditoria_estado,
                COALESCE(a.auditoria_observacion, '') AS auditoria_observacion,
                COALESCE(a.precio, 0) AS precio,
                COALESCE(a.tiene_resultado, FALSE) AS tiene_resultado                
            FROM 
                transaccional.evaluacion_medicamento a
            LEFT JOIN (
                SELECT 
                    TMD.codigo, 
                    TMD.tablamaestradetalle
                FROM 
                    sistema.tabla_maestra_detalle TMD
                INNER JOIN 
                    sistema.tabla_maestra TM 
                    ON TM.codigo = TMD.codigotablamaestra
                WHERE 
                    TM.tablamaestra = 'EstadoAuditoriaItem'
            ) AS TB 
            ON a.auditoria_codigo_estado = TB.codigo		
            WHERE a.evaluacion_eess_id = @evaluacion_id and a.es_eliminado = FALSE;"
            ;
            var parameters = new { evaluacion_id = evaluacioneessId };
            var result = await connection.QueryAsync<EvaluacionMedicamentoDto>(query, parameters);
            var resultados = result.ToList();
            return resultados;
        }

        public async Task<List<EvaluacionProcedimientoDto>> ObtenerProcedimientosPorEvaluacionAsync(long evaluacioneessId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                       SELECT 
                        P.numero_orden,
                        P.servicio,
                        P.grupo,
                        P.evaluacion_procedimiento_id,
                        P.codigo_procedimiento,
                        P.procedimiento,
                        COALESCE(P.cantidad_entregada, 0) AS cantidad_entregada,
		                COALESCE(P.cantidad_prescrita, 0) AS cantidad_prescrita,
		                COALESCE(P.auditoria_cantidad, 0) AS auditoria_cantidad,	
		                COALESCE(P.auditoria_orden_subsana_obs, FALSE) AS auditoria_orden_subsana_obs,
		                P.auditoria_orden_subsana_obs_texto,	
                        P.codigo_diagnostico,
                        P.auditoria_codigo_estado,
                        TB.tablamaestradetalle AS auditoria_estado,
                        COALESCE(P.auditoria_observacion, '') AS auditoria_observacion,
                        P.precio,
                        COALESCE(P.entregado, FALSE) AS entregado     
                    FROM 
                        transaccional.evaluacion_procedimiento P
                    LEFT JOIN (
                        SELECT 
                            TMD.codigo, 
                            TMD.tablamaestradetalle
                        FROM 
                            sistema.tabla_maestra_detalle TMD
                        INNER JOIN 
                            sistema.tabla_maestra TM 
                            ON TM.codigo = TMD.codigotablamaestra
                        WHERE 
                            TM.tablamaestra = 'EstadoAuditoriaItem'
                    ) AS TB 
                    ON P.auditoria_codigo_estado = TB.codigo
                    WHERE 
                        P.evaluacion_eess_id = @evaluacion_id 
                        AND P.es_eliminado = FALSE;";
            var parameters = new { evaluacion_id = evaluacioneessId };
            var result = await connection.QueryAsync<EvaluacionProcedimientoDto>(query, parameters);
            var resultados = result.ToList();
            return resultados;
        }

        public async Task<ItemAuditoriaDto> ObtenerItemAuditoriaIdAsync(long evaluacionId, string puntoCarga)
        {
            ItemAuditoriaDto evaluacion = null;
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = @"SELECT * FROM public.sp_auditoria_item_auditoria_id(@p_id_item_auditoria, @p_punto_carga);";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("p_id_item_auditoria", evaluacionId);
                        command.Parameters.AddWithValue("p_punto_carga", puntoCarga);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                evaluacion = new ItemAuditoriaDto
                                {
                                    IdItem = reader.GetInt32(reader.GetOrdinal("IdItem")),
                                    CodigoGrupoItem = reader.GetInt32(reader.GetOrdinal("CodigoGrupoItem")),
                                    AuditoriaPuntoCarga = reader.IsDBNull(reader.GetOrdinal("AuditoriaPuntoCarga")) ? null : reader.GetString(reader.GetOrdinal("AuditoriaPuntoCarga")),
                                    Grupo = reader.IsDBNull(reader.GetOrdinal("Grupo")) ? null : reader.GetString(reader.GetOrdinal("Grupo")),
                                    CodigoItem = reader.IsDBNull(reader.GetOrdinal("CodigoItem")) ? null : reader.GetString(reader.GetOrdinal("CodigoItem")),
                                    Item = reader.IsDBNull(reader.GetOrdinal("Item")) ? null : reader.GetString(reader.GetOrdinal("Item")),
                                    Cie10 = reader.IsDBNull(reader.GetOrdinal("Cie10")) ? null : reader.GetString(reader.GetOrdinal("Cie10")),
                                    Diagnostico = reader.IsDBNull(reader.GetOrdinal("Diagnostico")) ? null : reader.GetString(reader.GetOrdinal("Diagnostico")),
                                    AuditoriaUsuario = reader.IsDBNull(reader.GetOrdinal("AuditoriaUsuario")) ? null : reader.GetString(reader.GetOrdinal("AuditoriaUsuario")),
                                    AuditoriaFecha = reader.IsDBNull(reader.GetOrdinal("AuditoriaFecha")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("AuditoriaFecha")),
                                    AuditoriaObservacion = reader.IsDBNull(reader.GetOrdinal("AuditoriaObservacion")) ? null : reader.GetString(reader.GetOrdinal("AuditoriaObservacion")),
                                    CantidadPrescrita = reader.IsDBNull(reader.GetOrdinal("CantidadPrescrita")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("CantidadPrescrita")),
                                    CantidadEntregada = reader.IsDBNull(reader.GetOrdinal("CantidadEntregada")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("CantidadEntregada")),
                                    AuditoriaCantidad = reader.IsDBNull(reader.GetOrdinal("AuditoriaCantidad")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("AuditoriaCantidad")),
                                    IdEstadoAuditoria = reader.IsDBNull(reader.GetOrdinal("IdEstadoAuditoria")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdEstadoAuditoria")),
                                    EstadoAuditoria = reader.IsDBNull(reader.GetOrdinal("EstadoAuditoria")) ? null : reader.GetString(reader.GetOrdinal("EstadoAuditoria")),
                                    AuditoriaOrdenSubsanaObs = reader.GetBoolean(reader.GetOrdinal("AuditoriaOrdenSubsanaObs")),
                                    AuditoriaOrdenSubsanaObsTexto = reader.IsDBNull(reader.GetOrdinal("AuditoriaOrdenSubsanaObsTexto")) ? null : reader.GetString(reader.GetOrdinal("AuditoriaOrdenSubsanaObsTexto"))

                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en ObtenerItemAuditoriaIdAsync: {ex.Message}");
                throw;
            }

            return evaluacion;
        }

        public async Task<bool> ActualizarAuditoriaAsync(ActualizarAuditoriaRequest request)
        {
            bool resultado = false;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT public.sp_actualizar_auditoria(" +
                                 "@_id_item, @_punto_carga, @_auditoria_cantidad, " +
                                 "@_auditoria_observacion, @_id_estado_item, @_auditoria_usuario, @_auditoriaOrdenSubsanaObs, @_auditoriaOrdenSubsanaObsTexto);";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        // Agregar parámetros
                        command.Parameters.AddWithValue("@_id_item", request.IdItem);
                        command.Parameters.AddWithValue("@_punto_carga", request.PuntoCarga);
                        command.Parameters.AddWithValue("@_auditoria_cantidad",
                            request.AuditoriaCantidad.HasValue ? (object)request.AuditoriaCantidad.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@_auditoria_observacion", (object)request.AuditoriaObservacion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@_id_estado_item", request.IdEstadoItem);
                        command.Parameters.AddWithValue("@_auditoria_usuario", request.AuditoriaUsuario);
                        command.Parameters.AddWithValue("@_auditoriaOrdenSubsanaObs", request.AuditoriaOrdenSubsanaObs);
                        command.Parameters.AddWithValue("@_auditoriaOrdenSubsanaObsTexto", request.AuditoriaOrdenSubsanaObsTexto);



                        // Ejecutar comando y verificar resultado
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        resultado = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en ActualizarAuditoriaAsync: {ex.Message}");
                throw;
            }

            return resultado;
        }


        public async Task<bool> AceptacionMasivaItemsEvaluacion(AceptarMasivaEvaluacionRequest request)
        {
            bool resultado = false;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT public.sp_actualizar_auditoria_masiva(" +
                                 "@_evaluacion_id, @_auditoria_usuario);";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@_evaluacion_id", request.EvaluacionEESSId);
                        command.Parameters.AddWithValue("@_auditoria_usuario", request.AuditoriaUsuario);
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        resultado = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AceptacionMasivaItemsEvaluacion: {ex.Message}");
                throw;
            }

            return resultado;
        }


        public async Task<bool> ActualizarEnvioReceta(EnviaOrdenRequest request)
        {
            bool resultado = false;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT public.sp_actualizar_envio_receta(" +
                                 "@_evaluacion_id, @_auditoria_usuario);";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@_evaluacion_id", request.EvaluacionEESSId);
                        command.Parameters.AddWithValue("@_auditoria_usuario", "Admin");
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        resultado = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarEnvioReceta: {ex.Message}");
                throw;
            }

            return resultado;
        }


        public async Task<List<EvaluacionMinDto>> ObtenerEvaluacionesPorAtencionAsync(long atencionId)
        {
            var evaluaciones = new List<EvaluacionMinDto>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM public.sp_listar_evaluaciones(@AtencionId);";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AtencionId", atencionId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var evaluacion = new EvaluacionMinDto
                                {
                                    secuencia = reader.GetInt64(reader.GetOrdinal("Secuencia")),
                                    evaluacion_id = reader.GetInt32(reader.GetOrdinal("EvaluacionId")),
                                    fecha_evaluacion = reader.IsDBNull(reader.GetOrdinal("FechaEvaluacion"))
                                        ? "Sin fecha"
                                        : reader.GetString(reader.GetOrdinal("FechaEvaluacion")),
                                    hora_evaluacion = reader.IsDBNull(reader.GetOrdinal("HoraEvaluacion"))
                                        ? "Sin fecha"
                                        : reader.GetString(reader.GetOrdinal("HoraEvaluacion")),
                                    auditoria_estado = reader.IsDBNull(reader.GetOrdinal("AuditoriaEstado"))
                                        ? ""
                                        : reader.GetString(reader.GetOrdinal("AuditoriaEstado"))
                                };

                                evaluaciones.Add(evaluacion);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en ObtenerEvaluacionesPorAtencionAsync: {ex.Message}");
                throw;
            }

            return evaluaciones;
        }
            
   

        

    }


}
