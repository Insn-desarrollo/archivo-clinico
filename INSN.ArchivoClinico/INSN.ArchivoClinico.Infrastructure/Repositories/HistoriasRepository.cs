﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using NpgsqlTypes;
using Newtonsoft.Json.Linq;
using Dapper;
using INSN.ArchivoClinico.Infrastructure.Services;
//using INSN.ArchivoClinico.Application.DTOs;

namespace INSN.ArchivoClinico.Infrastructure.Repositories
{
    public class HistoriasRepository : IHistoriasRepository
    {
        private readonly string _connectionString;
        private readonly IPgDatabaseService _pgDatabaseService;
        public HistoriasRepository(IConfiguration configuration, IPgDatabaseService pgDatabaseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _pgDatabaseService = pgDatabaseService;
        }

        public async Task<List<ContadorPendientes>> ObtenerContadoresBandejaAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                       WITH 
                        por_asignar AS (
                            SELECT COUNT(*) AS cantidad_por_asignar 
                            FROM transaccional.atencion 
                            WHERE auditoria_usuario_asignado = 'Por asignar'
                        ),
                        triaje_pendiente AS (
                            SELECT COUNT(*) AS cantidad_triaje_pendiente
                            FROM transaccional.atencion 
                            WHERE auditoria_triaje_codigo_estado = 1
                        ),
                        evaluaciones_pendientes AS (
                            SELECT COUNT(*) AS cantidad_evaluaciones_pendiente
                            FROM transaccional.atencion 
                            WHERE auditoria_codigo_estado = 1 AND not cuenta_atencion_id is null
                        ),
                        cuentas_pendientes AS (
                            SELECT COUNT(*) AS cantidad_cuentas_pendientes
                            FROM transaccional.atencion a
                            INNER JOIN transaccional.cuentas_atencion c ON a.cuenta_atencion_id = c.cuenta_atencion_id
                            WHERE c.auditoria_codigo_estado = 1
                            AND (
	  	                        EXISTS (
		                           SELECT 1 FROM transaccional.evaluacion_procedimiento ea
		                           WHERE ea.evaluacion_eess_id IN (SELECT evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)    
		                           )
   	   	                        AND NOT EXISTS (
	                               SELECT 1 FROM transaccional.evaluacion_procedimiento ea
	                               WHERE ea.evaluacion_eess_id IN (SELECT e.evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)
	                               AND (ea.auditoria_codigo_estado IS NULL OR ea.auditoria_codigo_estado IN (1,3))
	   		                        )
   	                          )
	                          AND (
	  	                        EXISTS (
		                           SELECT 1 FROM transaccional.evaluacion_medicamento ea
		                           WHERE ea.evaluacion_eess_id IN (SELECT e.evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)    
		                           )
   	   	                        AND NOT EXISTS (
	                               SELECT 1 FROM transaccional.evaluacion_medicamento ea
	                               WHERE ea.evaluacion_eess_id IN (SELECT e.evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)
	                               AND (ea.auditoria_codigo_estado IS NULL OR ea.auditoria_codigo_estado IN (1,3))
	   		                        )
   	                          )
                        ),
                        sis_pendientes AS (
                            SELECT COUNT(*) AS cantidad_sis_pendientes
                            FROM transaccional.atencion a
                            INNER JOIN transaccional.atencion_sis s ON a.atencion_id = s.atencion_id
                            WHERE s.fua_guid IS NULL AND not a.cuenta_atencion_id is null
                            AND (
	  	                        EXISTS (
		                           SELECT 1 FROM transaccional.evaluacion_procedimiento ea
		                           WHERE ea.evaluacion_eess_id IN (SELECT evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)    
		                           )
   	   	                        AND NOT EXISTS (
	                               SELECT 1 FROM transaccional.evaluacion_procedimiento ea
	                               WHERE ea.evaluacion_eess_id IN (SELECT e.evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)
	                               AND (ea.auditoria_codigo_estado IS NULL OR ea.auditoria_codigo_estado IN (1,3))
	   		                        )
   	                          )
	                          AND (
	  	                        EXISTS (
		                           SELECT 1 FROM transaccional.evaluacion_medicamento ea
		                           WHERE ea.evaluacion_eess_id IN (SELECT e.evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)    
		                           )
   	   	                        AND NOT EXISTS (
	                               SELECT 1 FROM transaccional.evaluacion_medicamento ea
	                               WHERE ea.evaluacion_eess_id IN (SELECT e.evaluacion_eess_id FROM transaccional.evaluacion e WHERE e.atencion_id = a.atencion_id)
	                               AND (ea.auditoria_codigo_estado IS NULL OR ea.auditoria_codigo_estado IN (1,3))
	   		                        )
   	                          )
                        )
                        SELECT 
                            pa.cantidad_por_asignar, 
                            tp.cantidad_triaje_pendiente, 
                            ep.cantidad_evaluaciones_pendiente, 
                            cp.cantidad_cuentas_pendientes, 
                            sp.cantidad_sis_pendientes
                        FROM por_asignar pa
                        JOIN triaje_pendiente tp ON TRUE
                        JOIN evaluaciones_pendientes ep ON TRUE
                        JOIN cuentas_pendientes cp ON TRUE
                        JOIN sis_pendientes sp ON TRUE;"
            ;
            var result = await connection.QueryAsync<ContadorPendientes>(query);
            var resultados = result.ToList();
            return resultados;
        }

        public async Task<IEnumerable<HistoriaClinicaDto>> ConsultarHistoriasAsync(HistoriaFiltro filtro)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = @"SELECT * FROM public.fn_historias_clinicas(
                     @p_historia_clinica,
                     @p_documento_identidad,
                     @p_nombre,
                     @p_codigo_estado,
                     @p_page_number,
                     @p_page_size
                   );";

                var parametros = new
                {
                    p_historia_clinica = filtro.HistoriaClinica ?? string.Empty,
                    p_documento_identidad = filtro.DocumentoIdentidad ?? string.Empty,
                    p_nombre = filtro.Nombre ?? string.Empty,
                    p_codigo_estado = filtro.CodigoEstado ?? (object)DBNull.Value,
                    p_page_number = filtro.Page,
                    p_page_size = filtro.PageSize
                };

                var resultados = await connection.QueryAsync<HistoriaClinicaDto>(sql, parametros);

                return resultados;
            }
            catch (Exception ex)
            {

                throw ex;
            }          
        }       

        public async Task<HistoriaClinicaConsultaDto> ConsultarPacienteAsync(string historia)
        {

            var query = @"SELECT A.paciente_id as historia_clinica_id,
		            A.paciente_id,
		            A.historia_clinica as numero_historia,
		            1 as codigo_tipo_documento,
		            A.tipo_documento AS tipos_documento,
                    A.numero_documento numero_documento,
		            A.apellido_paterno,
                    A.apellido_materno,
                    A.nombres,
		            COALESCE(TO_CHAR(A.fecha_nacimiento, 'DD/MM/YYYY')::character varying, 'Sin fecha') AS fecha_nacimiento,
		            1 as codigo_tipo_sexo,
		            A.sexo AS tipo_sexo,
		            COALESCE(A.direccion, '') as direccion,
		            '-' as correo,
		            1 as codigo_estado_historia,       
                    'Activa' AS estado_historia
                from public.emr_paciente A
                where A.historia_clinica = @historia;
        ";

            var parameters = new { historia = historia };

            var paciente = await _pgDatabaseService.GetConnection().QueryFirstOrDefaultAsync<HistoriaClinicaConsultaDto>(query, parameters);

            return paciente;
        }

        public async Task<IEnumerable<AtencionHcDto>> ConsultarAtencionesEmergenciaAsync(string historia)
        {
            var query = @"select 
	                   a.atencion_id,	   
	                   c.historia_clinica,
	                   c.numero_documento, 
	                   COALESCE(c.apellido_paterno, '') || ' ' || COALESCE(c.apellido_materno, '') || ' ' || COALESCE(c.nombres, '') AS nombre_paciente,      
	                   d.nombre as servicio,	   
	                   TO_CHAR(a.created_at, 'DD/MM/YYYY HH24:MI:SS') AS fecha_hora_atencion
                from public.hce_atencion a
                inner join public.emr_triaje b on a.triaje_id = b.triaje_id
                inner join public.emr_paciente c on b.paciente_id = c.paciente_id 
                inner join public.emr_destino_atencion d on d.destino_atencion_id = a.destino_atencion_id
                where c.historia_clinica = @historia
                order by a.atencion_id asc;
        ";

            var parameters = new { historia = historia };

            var atencion = await _pgDatabaseService.GetConnection().QueryAsync<AtencionHcDto>(query, parameters);

            return atencion;
        }

        public async Task<IEnumerable<EvaluacionHcDto>> ConsultarEvaluacionesEmergenciaAsync(int atencion_id)
        {
            var query = @"select 
            ev.evaluacion_id, 
            TO_CHAR(ev.created_at, 'DD/MM/YYYY HH24:MI:SS') AS fecha_hora_evaluacion
            from public.emr_evaluacion ev
            where ev.atencion_id = @atencion_id
            order by ev.evaluacion_id asc;";

            var parameters = new { atencion_id = atencion_id };

            var evaluacion = await _pgDatabaseService.GetConnection().QueryAsync<EvaluacionHcDto>(query, parameters);

            return evaluacion;
        }

        public async Task<IEnumerable<DocumentosEvaluacionHC>> ConsultaDocumentoAtencion(int atencion_id)
        {
            try
            {
                var query = @"
            WITH datos AS (                
               SELECT 
                    t.triaje_id,
                    a.atencion_id,    
                    NULL AS evaluacion_id,
                    t.paciente_hc AS historia_clinica, 
                    COALESCE(t.pa_nombres, '') || ' ' || 
                    COALESCE(t.pa_apellido_paterno, '') || ' ' || 
                    COALESCE(t.pa_apellido_materno, '') AS paciente,
                    'Formato Fua' AS tipo_documento,
                    a.url_documento_fua_firma AS documento,
                    CASE 
                        WHEN a.estado = 'R' AND a.url_documento_fua_firma IS NULL THEN 'En espera de alta'
                        WHEN a.estado = 'A' AND a.url_documento_fua_firma IS NULL THEN 'Para firma'
                        WHEN a.estado = 'A' AND a.url_documento_fua_firma IS NOT NULL THEN 'Documento firmado'
                        ELSE 'Pendiente'
                    END AS estado,
                    TO_CHAR(t.fusu, 'DD/MM/YYYY') AS fecha_registro,
                    TO_CHAR(a.fecha_firma_documento_fua, 'DD/MM/YYYY') AS fecha_firma,
                    a.profesional_id
                FROM public.emr_triaje t 
                INNER JOIN public.emr_paciente p ON t.paciente_id = p.paciente_id
                INNER JOIN public.hce_atencion a ON a.triaje_id = t.triaje_id
                WHERE a.cuenta_atencion_id IS NOT NULL
                AND a.fuente_financimiento_descripcion = 'SIS'
                AND a.atencion_id = @atencion_id
                AND a.fua_estado in ('P', 'F')
                
                UNION ALL	 

                -- Quinta consulta (Papeleta Egreso)
                SELECT 
                    t.triaje_id,
                    a.atencion_id,    
                    NULL AS evaluacion_id,
                    t.paciente_hc AS historia_clinica, 
                    COALESCE(t.pa_nombres, '') || ' ' || 
                    COALESCE(t.pa_apellido_paterno, '') || ' ' || 
                    COALESCE(t.pa_apellido_materno, '') AS paciente,
                    'Papeleta Egreso' AS tipo_documento,
                    e.url_documento_egreso_firma AS documento,
                    CASE 
                        WHEN a.estado = 'R' AND e.url_documento_egreso_firma IS NULL THEN 'En espera de alta'
                        WHEN a.estado = 'A' AND e.url_documento_egreso_firma IS NULL AND COALESCE(e.flag_documento_firmado, false) IS false THEN 'Para firma'
                        WHEN a.estado = 'A' AND e.url_documento_egreso_firma IS NOT NULL AND COALESCE(e.flag_documento_firmado, false) IS true THEN 'Documento firmado'
                        ELSE 'Pendiente'
                    END AS estado,
                    TO_CHAR(t.fusu, 'DD/MM/YYYY') AS fecha_registro,
                    TO_CHAR(e.fecha_firma_documento_egreso, 'DD/MM/YYYY') AS fecha_firma,
                    a.profesional_id
                FROM public.emr_triaje t 
                INNER JOIN public.emr_paciente p ON t.paciente_id = p.paciente_id
                INNER JOIN public.hce_atencion a ON a.triaje_id = t.triaje_id
                INNER JOIN public.emr_egreso e ON e.atencion_id = a.atencion_id
                WHERE a.cuenta_atencion_id IS NOT NULL
                AND a.atencion_id = @atencion_id
            )
            SELECT distinct * FROM datos;";

                var parameters = new { atencion_id = atencion_id };
                var documentos = await _pgDatabaseService.GetConnection().QueryAsync<DocumentosEvaluacionHC>(query, parameters);

                return documentos;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }


        public async Task<IEnumerable<DocumentosEvaluacionHC>> ConsultaDocumentoEvaluacion(int evaluacion_id)
        {
            var query = @"
            WITH datos AS (
                -- Primera consulta (Historia Clínica)
                SELECT 
                    t.triaje_id,
                    a.atencion_id,    
                    e.evaluacion_id,
                    t.paciente_hc AS historia_clinica, 
                    COALESCE(t.pa_nombres, '') || ' ' || 
                    COALESCE(t.pa_apellido_paterno, '') || ' ' || 
                    COALESCE(t.pa_apellido_materno, '') AS paciente,
                    'Historia Clinica' AS tipo_documento,
                    e.url_documento_evaluacion_firma AS documento,
	                CASE
                        WHEN (SELECT COUNT(*) 
                            FROM public.emr_evaluacion_examen_auxiliar em 
                            WHERE em.evaluacion_id = e.evaluacion_id 
                            AND em.auditoria_codigo_estado IN (1,3)
                            ) > 0 AND (SELECT COUNT(*) 
                                FROM public.emr_evaluacion_medicamento em 
                                WHERE em.evaluacion_id = e.evaluacion_id 
                                AND em.auditoria_codigo_estado IN (1,3)
                            ) > 0 AND e.url_documento_evaluacion_firma IS NULL THEN 'En espera de orden y receta'
                        WHEN (SELECT COUNT(*) 
                            FROM public.emr_evaluacion_examen_auxiliar em 
                            WHERE em.evaluacion_id = e.evaluacion_id 
                            AND em.auditoria_codigo_estado IN (1,3)
                            ) = 0 AND (SELECT COUNT(*) 
                                FROM public.emr_evaluacion_medicamento em 
                                WHERE em.evaluacion_id = e.evaluacion_id 
                                AND em.auditoria_codigo_estado IN (1,3)
                            ) = 0 AND e.url_documento_evaluacion_firma IS NULL THEN 'Para firma'
                        WHEN e.url_documento_evaluacion_firma IS NOT NULL THEN 'Documento firmado'
                        ELSE 'Pendiente'
                    END AS estado,
                    TO_CHAR(t.fusu, 'DD/MM/YYYY') AS fecha_registro,
                    TO_CHAR(e.fecha_firma_documento_evaluacion, 'DD/MM/YYYY') AS fecha_firma,
                    a.profesional_id
                FROM public.emr_triaje t 
                INNER JOIN public.emr_paciente p ON t.paciente_id = p.paciente_id
                INNER JOIN public.hce_atencion a ON a.triaje_id = t.triaje_id
                INNER JOIN public.emr_evaluacion e ON e.atencion_id = a.atencion_id
                WHERE a.cuenta_atencion_id IS NOT NULL
                AND (@evaluacion_id IS NULL OR e.evaluacion_id = @evaluacion_id)

                UNION ALL

                -- Segunda consulta (Ordenes)
                SELECT 
                    t.triaje_id,
                    a.atencion_id,    
                    e.evaluacion_id,
                    t.paciente_hc AS historia_clinica, 
                    COALESCE(t.pa_nombres, '') || ' ' || 
                    COALESCE(t.pa_apellido_paterno, '') || ' ' || 
                    COALESCE(t.pa_apellido_materno, '') AS paciente,
                    'Orden' AS tipo_documento,
                    e.url_documento_orden_firma AS documento,
                    CASE 
		                WHEN (SELECT COUNT(*) 
                            FROM public.emr_evaluacion_examen_auxiliar em 
                            WHERE em.evaluacion_id = e.evaluacion_id 
                            AND em.auditoria_codigo_estado IN (1,3)
                        ) > 0 THEN 'En auditoria'
                        WHEN (SELECT COUNT(*) 
				                FROM public.emr_evaluacion_examen_auxiliar em 
				                WHERE em.evaluacion_id = e.evaluacion_id 
				                AND em.auditoria_codigo_estado IN (1,3)
        	                ) = 0 AND e.url_documento_orden_firma IS NULL THEN 'Para firma'
                        ELSE 'Documento firmado'
                    END AS estado,
                    TO_CHAR(e.created_at, 'DD/MM/YYYY') AS fecha_registro,
                    TO_CHAR(e.fecha_firma_documento_orden, 'DD/MM/YYYY') AS fecha_firma,
                    a.profesional_id
                FROM public.emr_triaje t 
                INNER JOIN public.emr_paciente p ON t.paciente_id = p.paciente_id
                INNER JOIN public.hce_atencion a ON a.triaje_id = t.triaje_id
                INNER JOIN public.emr_evaluacion e ON e.atencion_id = a.atencion_id
                WHERE a.cuenta_atencion_id IS NOT NULL
                AND (@evaluacion_id IS NULL OR e.evaluacion_id = @evaluacion_id)
	            AND EXISTS (
                    SELECT 1 FROM public.emr_evaluacion_examen_auxiliar ea
                    WHERE ea.evaluacion_id IN (
                        SELECT evaluacion_id FROM public.emr_evaluacion WHERE atencion_id = a.atencion_id
                    )    
                )

                UNION ALL

                -- Tercera consulta (recetas)
                SELECT 
                    t.triaje_id,
                    a.atencion_id,    
                    e.evaluacion_id,
                    t.paciente_hc AS historia_clinica, 
                    COALESCE(t.pa_nombres, '') || ' ' || 
                    COALESCE(t.pa_apellido_paterno, '') || ' ' || 
                    COALESCE(t.pa_apellido_materno, '') AS paciente,
                    'Receta' AS tipo_documento,
                    e.url_documento_receta_firma AS documento,
                    CASE 
		                    WHEN (SELECT COUNT(*) 
                                FROM public.emr_evaluacion_medicamento em 
                                WHERE em.evaluacion_id = e.evaluacion_id 
                                AND em.auditoria_codigo_estado IN (1,3)
                            ) > 0 THEN 'En auditoria'
                            WHEN (SELECT COUNT(*) 
				                    FROM public.emr_evaluacion_medicamento em 
				                    WHERE em.evaluacion_id = e.evaluacion_id 
				                    AND em.auditoria_codigo_estado IN (1,3)
        	                    ) = 0 AND e.url_documento_receta_firma IS NULL THEN 'Para firma'
                            ELSE 'Documento firmado'
                     END AS estado,
                    TO_CHAR(e.created_at, 'DD/MM/YYYY') AS fecha_registro,
                    TO_CHAR(e.fecha_firma_documento_receta, 'DD/MM/YYYY') AS fecha_firma,
                    a.profesional_id
                FROM public.emr_triaje t 
                INNER JOIN public.emr_paciente p ON t.paciente_id = p.paciente_id
                INNER JOIN public.hce_atencion a ON a.triaje_id = t.triaje_id
                INNER JOIN public.emr_evaluacion e ON e.atencion_id = a.atencion_id
                WHERE a.cuenta_atencion_id IS NOT NULL
                AND (@evaluacion_id IS NULL OR e.evaluacion_id = @evaluacion_id)
	            AND EXISTS (
                    SELECT 1 FROM public.emr_evaluacion_medicamento ea
                    WHERE ea.evaluacion_id IN (
                        SELECT evaluacion_id FROM public.emr_evaluacion WHERE atencion_id = a.atencion_id
                    )    
                )
            )
            SELECT distinct * FROM datos;";

            var parameters = new { evaluacion_id = evaluacion_id };
            var documentos = await _pgDatabaseService.GetConnection().QueryAsync<DocumentosEvaluacionHC>(query, parameters);

            return documentos;
        }



        public async Task<TriajeResponseDto> ActualizarTriajeAsync(ActualizarTriajeRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.AtencionId <= 0) throw new ArgumentException("Atención ID inválido.", nameof(request));

            TriajeResponseDto resultado = null;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (var command = new NpgsqlCommand("public.sp_actualizar_atencion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.AddWithValue("_atencionid", request.AtencionId);
                        command.Parameters.AddWithValue("_auditoriatriajecodigoestado", request.AuditoriaTriajeCodigoEstado);
                        command.Parameters.AddWithValue("_auditoriaobservacion", (object)request.AuditoriaObservacion ?? "");
                        command.Parameters.AddWithValue("_fechaingresoatencion", FormatDate(request.FechaIngresoAtencion));
                        command.Parameters.AddWithValue("_horaingresoatencion", request.HoraIngresoAtencion ?? "");
                        command.Parameters.AddWithValue("_fuentefinanciamientoid", request.FuenteFinanciamientoId);
                        command.Parameters.AddWithValue("_tipofinanciamientoid", request.TipoFinanciamientoId);
                        command.Parameters.AddWithValue("_sisaseguradocomponente", request.SisAseguradoComponente ?? "");
                        command.Parameters.AddWithValue("_sisaseguradodisa", request.SisAseguradoDisa ?? "");
                        command.Parameters.AddWithValue("_sisaseguradolote", request.SisAseguradoLote ?? "");
                        command.Parameters.AddWithValue("_sisaseguradonumero", request.SisAseguradoNumero ?? "");
                        command.Parameters.AddWithValue("_sisaseguradocorrelativo", request.SisAseguradoCorrelativo ?? "");
                        command.Parameters.AddWithValue("_sisaseguradotipotabla", request.SisAseguradoTipoTabla ?? "");
                        command.Parameters.AddWithValue("_sisaseguradoidentificador", request.SisAseguradoIdentificador ?? "");
                        command.Parameters.AddWithValue("_sisaseguradoplancobertura", request.SisAseguradoPlanCobertura ?? "");
                        command.Parameters.AddWithValue("_sisaseguradogrpoblacional", request.SisAseguradoGrpPoblacional ?? "");
                        command.Parameters.AddWithValue("_sisaseguradofechaafiliacion", request.SisAseguradoFechaAfiliacion ?? "");
                        command.Parameters.AddWithValue("_sisaseguradotiposeguro", request.SisAseguradoTipoSeguro ?? "");
                        command.Parameters.AddWithValue("_sisaseguradodesctiposeguro", request.SisAseguradoDescTipoSeguro ?? "");
                        command.Parameters.AddWithValue("_sisaseguradoestado", request.SisAseguradoEstado ?? "");
                        command.Parameters.AddWithValue("_particularboletaapertura", request.ParticularBoletaApertura ?? "");
                        command.Parameters.AddWithValue("_particularconceptoboleta", request.ParticularConceptoBoleta ?? "");
                        command.Parameters.AddWithValue("_auditoriatriajeobservacion", request.AuditoriaTriajeObservacion ?? "");
                        command.Parameters.AddWithValue("_auditoriatriajesubsanaobs", request.AuditoriaTriajeSubsanaObs ?? false);
                        command.Parameters.AddWithValue("_auditoriatriajesubsanaobstexto", request.AuditoriaTriajeSubsanaObsTexto ?? "");
                        command.Parameters.AddWithValue("_auditoriausuario", request.AuditoriaUsuario ?? "");
                        // Continúa agregando los demás parámetros...

                        // Parámetros de salida
                        var atencionIdEESSParam = new NpgsqlParameter("_atencionideess", NpgsqlTypes.NpgsqlDbType.Integer)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(atencionIdEESSParam);

                        var cuentaAtencionIdParam = new NpgsqlParameter("_cuentaatencionid", NpgsqlTypes.NpgsqlDbType.Integer)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cuentaAtencionIdParam);

                        var formatoFuaParam = new NpgsqlParameter("_formatofua", NpgsqlTypes.NpgsqlDbType.Text)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(formatoFuaParam);

                        // Ejecutar
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        // Resultado
                        resultado = new TriajeResponseDto
                        {
                            atencion_id_eess = Convert.ToInt64(atencionIdEESSParam.Value),
                            cuenta_atencion_id = Convert.ToInt64(cuentaAtencionIdParam.Value),
                            formato_fua = (string)formatoFuaParam.Value
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al actualizar el triaje.", ex);
            }

            return resultado;
        }

        public async Task<List<AuditorDto>> ObtenerListaUsuariosAsync()
        {
            var auditores = new List<AuditorDto>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT * FROM public.sp_obtener_lista_auditores();";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var auditor = new AuditorDto
                                {
                                    Usuario = reader.GetString(reader.GetOrdinal("Usuario")),
                                    Cargo = reader.GetInt32(reader.GetOrdinal("Cargo"))
                                };
                                auditores.Add(auditor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en ObtenerListaAuditoresAsync: {ex.Message}");
                throw;
            }

            return auditores;
        }              
        
        public async Task<bool> AsignarAtencionesAsync()
        {
            bool resultado = false;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // SQL para ejecutar la función asignar_atenciones_a_usuarios
                    string sql = "SELECT asignar_atenciones_a_usuarios();";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        // Ejecuta la función
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        resultado = rowsAffected > 0; // Si hay registros actualizados
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en AsignarAtencionesAsync: {ex.Message}");
                throw;
            }
            return resultado;
        }      
        public static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public async Task<long> RegistrarFuaAsync(Atencion request)
        {
            
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (var command = new NpgsqlCommand("public.sp_registrar_fua", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.AddWithValue("p_idatencion", request.idAtencion);
                        command.Parameters.AddWithValue("p_lotefua", request.loteFua ?? "");
                        command.Parameters.AddWithValue("p_nrofua", request.nroFua ?? "");
                        command.Parameters.AddWithValue("p_renipress", request.renipress ?? "");
                        command.Parameters.AddWithValue("p_idcategoria", request.idCategoria ?? "");
                        command.Parameters.AddWithValue("p_nivel", request.nivel ?? "");
                        command.Parameters.AddWithValue("p_idpuntodigitacion", request.idPuntoDigitacion);
                        command.Parameters.AddWithValue("p_idcomponente", request.idComponente ?? "");
                        command.Parameters.AddWithValue("p_iddisaasegurado", request.idDisaAsegurado ?? "");
                        command.Parameters.AddWithValue("p_idloteasegurado", request.idLoteAsegurado ?? "");
                        command.Parameters.AddWithValue("p_idcorrelativoasegurado", request.idCorrelativoAsegurado ?? "");
                        command.Parameters.AddWithValue("p_idsecuenciaasegurado", request.idSecuenciaAsegurado ?? "");
                        command.Parameters.AddWithValue("p_idtablaasegurado", request.idTablaAsegurado ?? "");
                        command.Parameters.AddWithValue("p_idcontratoasegurado", request.idContratoAsegurado ?? "");
                        command.Parameters.AddWithValue("p_idplan", request.idPlan ?? "");
                        command.Parameters.AddWithValue("p_idgrupopoblacional", request.idGrupoPoblacional ?? "");
                        command.Parameters.AddWithValue("p_idtipodocasegurado", request.idTipoDocAsegurado ?? "");
                        command.Parameters.AddWithValue("p_numdocasegurado", request.numDocAsegurado ?? "");
                        command.Parameters.AddWithValue("p_apepaterno", request.apePaterno ?? "");
                        command.Parameters.AddWithValue("p_apematerno", request.apeMaterno ?? "");
                        command.Parameters.AddWithValue("p_nombres", request.nombres ?? "");
                        command.Parameters.AddWithValue("p_fecnac", request.fecNac.HasValue ? request.fecNac.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_idsexo", request.idSexo ?? "");
                        command.Parameters.AddWithValue("p_idubigeo", request.idUbigeo ?? "");
                        command.Parameters.AddWithValue("p_historiaclinica", request.historiaClinica ?? "");
                        command.Parameters.AddWithValue("p_idtipoatencion", request.idTipoAtencion ?? "");
                        command.Parameters.AddWithValue("p_idcondicionmaterna", request.idCondicionMaterna ?? "");
                        command.Parameters.AddWithValue("p_idmodalidadatencion", request.idModalidadAtencion ?? "");
                        command.Parameters.AddWithValue("p_nroautorizacion", request.nroAutorizacion ?? "");
                        command.Parameters.AddWithValue("p_montoautorizado",  request.montoAutorizado == null ? request.montoAutorizado : Convert.ToDecimal(request.montoAutorizado) );
                        command.Parameters.AddWithValue("p_fechoraatencion", request.fecHoraAtencion.HasValue ? request.fecHoraAtencion.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_renipressreferencia", request.renipressReferencia ?? "");
                        command.Parameters.AddWithValue("p_nrohojareferencia", request.nroHojaReferencia ?? "");
                        command.Parameters.AddWithValue("p_idservicio", request.idServicio ?? "");
                        command.Parameters.AddWithValue("p_idorigenpersonal", request.idOrigenPersonal ?? "");
                        command.Parameters.AddWithValue("p_idlugaratencion", request.idLugarAtencion ?? "");
                        command.Parameters.AddWithValue("p_iddestinoasegurado", request.idDestinoAsegurado ?? "");
                        command.Parameters.AddWithValue("p_fecingresohospitalizacion", request.fecIngresoHospitalizacion.HasValue ? request.fecIngresoHospitalizacion.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_fecaltahospitalizacion", request.fecAltaHospitalizacion.HasValue ? request.fecAltaHospitalizacion.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_renipresscontrareferencia", request.renipressContraReferencia ?? "");
                        command.Parameters.AddWithValue("p_nrohojacontrareferencia", request.nroHojaContraReferencia ?? "");
                        command.Parameters.AddWithValue("p_fecparto", request.fecParto.HasValue ? request.fecParto.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_idgruporiesgo", request.idGrupoRiesgo ?? "");
                        command.Parameters.AddWithValue("p_fecfallecimiento", request.fecFallecimiento.HasValue ? request.fecFallecimiento.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_renipressofertaflexible", request.renipressOfertaFlexible ?? "");
                        command.Parameters.AddWithValue("p_idetnia", request.idEtnia ?? "");
                        command.Parameters.AddWithValue("p_idiafas", request.idIafas ?? "");
                        command.Parameters.AddWithValue("p_idcodigoiafas", request.idCodigoIafas ?? "");
                        command.Parameters.AddWithValue("p_idups", request.idUps ?? "");
                        command.Parameters.AddWithValue("p_feccorteadministrativo", request.fecCorteAdministrativo.HasValue ? request.fecCorteAdministrativo.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_idudrautorizavinculado", request.idUdrAutorizaVinculado ?? "");
                        command.Parameters.AddWithValue("p_loteautorizavinculado", request.loteAutorizaVinculado ?? "");
                        command.Parameters.AddWithValue("p_nroautorizavinculado", request.nroAutorizaVinculado ?? "");
                        command.Parameters.AddWithValue("p_disafuavinculado", request.disaFuaVinculado ?? "");
                        command.Parameters.AddWithValue("p_lotefuavinculado", request.loteFuaVinculado ?? "");
                        command.Parameters.AddWithValue("p_nrofuavinculado", request.nroFuaVinculado ?? "");
                        command.Parameters.AddWithValue("p_idtipodocrespate", request.idTipoDocRespAte ?? "");
                        command.Parameters.AddWithValue("p_numdocrespate", request.numDocRespAte ?? "");
                        command.Parameters.AddWithValue("p_idtipopersonalsalud", request.idTipoPersonalSalud ?? "");
                        command.Parameters.AddWithValue("p_idespecialidadrespate", request.idEspecialidadRespAte ?? "");
                        command.Parameters.AddWithValue("p_esegresadorespate", request.esEgresadoRespAte ?? "");
                        command.Parameters.AddWithValue("p_colegiaturarespate", request.colegiaturaRespAte ?? "");
                        command.Parameters.AddWithValue("p_rnerespate", request.rneRespAte ?? "");
                        command.Parameters.AddWithValue("p_idtipodocdigitador", request.idTipoDocDigitador ?? "");
                        command.Parameters.AddWithValue("p_numdocdigitador", request.numDocDigitador ?? "");
                        command.Parameters.AddWithValue("p_fechoraregistro", DateTime.Now.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("p_observacion", request.observacion ?? "");
                        command.Parameters.AddWithValue("p_versionaplicativo", request.versionAplicativo ?? "");
                        command.Parameters.AddWithValue("p_codigoacreditacion", request.codigoAcreditacion ?? "");
                        command.Parameters.AddWithValue("p_fechorainifuaadm", request.fecHoraIniFuaAdm.HasValue ? request.fecHoraIniFuaAdm.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_fechorafinfuaadm", request.fecHoraFinFuaAdm.HasValue ? request.fecHoraFinFuaAdm.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_idmotivoingresocasamaterna", request.idMotivoIngresoCasaMaterna ?? 0);
                        command.Parameters.AddWithValue("p_idcasamaterna", request.idCasaMaterna == null ? "-" : request.idCasaMaterna?.ToString());
                        command.Parameters.AddWithValue("p_fechoracrea", request.control.fecHoraCrea.HasValue ? request.control.fecHoraCrea.Value.ToString("dd/MM/yyyy") : "");
                        command.Parameters.AddWithValue("p_idestado", request.idEstado == null ? "-" : request.idEstado?.ToString());   

                        //// Parámetro de salida
                        //var idAtencionParam = new NpgsqlParameter("p_idatencion", NpgsqlDbType.Bigint)
                        //{
                        //    Direction = ParameterDirection.InputOutput,
                        //    Value = request.idAtencion
                        //};
                        //command.Parameters.Add(idAtencionParam);

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        return (long)request.idAtencion; // Convert.ToInt64(idAtencionParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al registrar el FUA.", ex);
            }
        }

        //public async Task<decimal> RegistrarFuaAsync(Atencion atencion)
        //{
        //    try
        //    {

        //        using (var connection = new NpgsqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            var parameters = new DynamicParameters();
        //            parameters.Add("p_idatencion", atencion.idAtencion.HasValue ? (object)atencion.idAtencion.Value : DBNull.Value, DbType.Int64, ParameterDirection.InputOutput);
        //            parameters.Add("p_lotefua", string.IsNullOrEmpty(atencion.loteFua) ? DBNull.Value : (object)atencion.loteFua);
        //            parameters.Add("p_nrofua", string.IsNullOrEmpty(atencion.nroFua) ? DBNull.Value : (object)atencion.nroFua);
        //            parameters.Add("p_renipress", string.IsNullOrEmpty(atencion.renipress) ? DBNull.Value : (object)atencion.renipress);
        //            parameters.Add("p_idcategoria", string.IsNullOrEmpty(atencion.idCategoria) ? DBNull.Value : (object)atencion.idCategoria);
        //            parameters.Add("p_nivel", string.IsNullOrEmpty(atencion.nivel) ? DBNull.Value : (object)atencion.nivel);
        //            parameters.Add("p_idpuntodigitacion", atencion.idPuntoDigitacion.HasValue ? (object)atencion.idPuntoDigitacion.Value : 0);
        //            parameters.Add("p_idcomponente", string.IsNullOrEmpty(atencion.idComponente) ? DBNull.Value : (object)atencion.idComponente);
        //            parameters.Add("p_iddisaasegurado", string.IsNullOrEmpty(atencion.idDisaAsegurado) ? DBNull.Value : (object)atencion.idDisaAsegurado);
        //            parameters.Add("p_idloteasegurado", string.IsNullOrEmpty(atencion.idLoteAsegurado) ? DBNull.Value : (object)atencion.idLoteAsegurado);
        //            parameters.Add("p_idcorrelativoasegurado", string.IsNullOrEmpty(atencion.idCorrelativoAsegurado) ? DBNull.Value : (object)atencion.idCorrelativoAsegurado);
        //            parameters.Add("p_idsecuenciaasegurado", string.IsNullOrEmpty(atencion.idSecuenciaAsegurado) ? DBNull.Value : (object)atencion.idSecuenciaAsegurado);
        //            parameters.Add("p_idtablaasegurado", string.IsNullOrEmpty(atencion.idTablaAsegurado) ? DBNull.Value : (object)atencion.idTablaAsegurado);
        //            parameters.Add("p_idcontratoasegurado", string.IsNullOrEmpty(atencion.idContratoAsegurado) ? DBNull.Value : (object)atencion.idContratoAsegurado);
        //            parameters.Add("p_idplan", string.IsNullOrEmpty(atencion.idPlan) ? DBNull.Value : (object)atencion.idPlan);
        //            parameters.Add("p_idgrupopoblacional", string.IsNullOrEmpty(atencion.idGrupoPoblacional) ? DBNull.Value : (object)atencion.idGrupoPoblacional);
        //            parameters.Add("p_idtipodocasegurado", string.IsNullOrEmpty(atencion.idTipoDocAsegurado) ? DBNull.Value : (object)atencion.idTipoDocAsegurado);
        //            parameters.Add("p_numdocasegurado", string.IsNullOrEmpty(atencion.numDocAsegurado) ? DBNull.Value : (object)atencion.numDocAsegurado);
        //            parameters.Add("p_apepaterno", string.IsNullOrEmpty(atencion.apePaterno) ? DBNull.Value : (object)atencion.apePaterno);
        //            parameters.Add("p_apematerno", string.IsNullOrEmpty(atencion.apeMaterno) ? DBNull.Value : (object)atencion.apeMaterno);
        //            parameters.Add("p_nombres", string.IsNullOrEmpty(atencion.nombres) ? DBNull.Value : (object)atencion.nombres);                   
        //            parameters.Add("p_fecnac", atencion.fecNac.HasValue ? atencion.fecNac.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_idsexo", string.IsNullOrEmpty(atencion.idSexo) ? DBNull.Value : (object)atencion.idSexo);
        //            parameters.Add("p_idubigeo", string.IsNullOrEmpty(atencion.idUbigeo) ? DBNull.Value : (object)atencion.idUbigeo);
        //            parameters.Add("p_historiaclinica", string.IsNullOrEmpty(atencion.historiaClinica) ? DBNull.Value : (object)atencion.historiaClinica);
        //            parameters.Add("p_idtipoatencion", string.IsNullOrEmpty(atencion.idTipoAtencion) ? DBNull.Value : (object)atencion.idTipoAtencion);
        //            parameters.Add("p_idcondicionmaterna", string.IsNullOrEmpty(atencion.idCondicionMaterna) ? DBNull.Value : (object)atencion.idCondicionMaterna);
        //            parameters.Add("p_idmodalidadatencion", string.IsNullOrEmpty(atencion.idModalidadAtencion) ? DBNull.Value : (object)atencion.idModalidadAtencion);
        //            parameters.Add("p_nroautorizacion", string.IsNullOrEmpty(atencion.nroAutorizacion) ? "-" : (object)atencion.nroAutorizacion);
        //            parameters.Add("p_montoautorizado", atencion.montoAutorizado == null ? 0 : (object)atencion.montoAutorizado);
        //            parameters.Add("p_fechoraatencion", atencion.fecHoraAtencion.HasValue ? atencion.fecHoraAtencion.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_renipressreferencia", string.IsNullOrEmpty(atencion.renipressReferencia) ? "-" : (object)atencion.renipressReferencia);
        //            parameters.Add("p_nrohojareferencia", string.IsNullOrEmpty(atencion.nroHojaReferencia) ? "-" : (object)atencion.nroHojaReferencia);
        //            parameters.Add("p_idservicio", string.IsNullOrEmpty(atencion.idServicio) ? DBNull.Value : (object)atencion.idServicio);
        //            parameters.Add("p_idorigenpersonal", string.IsNullOrEmpty(atencion.idOrigenPersonal) ? DBNull.Value : (object)atencion.idOrigenPersonal);
        //            parameters.Add("p_idlugaratencion", string.IsNullOrEmpty(atencion.idLugarAtencion) ? DBNull.Value : (object)atencion.idLugarAtencion);
        //            parameters.Add("p_iddestinoasegurado", string.IsNullOrEmpty(atencion.idDestinoAsegurado) ? DBNull.Value : (object)atencion.idDestinoAsegurado);                 
        //            parameters.Add("p_fecingresohospitalizacion", atencion.fecIngresoHospitalizacion.HasValue ? atencion.fecIngresoHospitalizacion.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_fecaltahospitalizacion", atencion.fecAltaHospitalizacion.HasValue ? atencion.fecAltaHospitalizacion.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_renipresscontrareferencia", string.IsNullOrEmpty(atencion.renipressContraReferencia) ? "-" : (object)atencion.renipressContraReferencia);
        //            parameters.Add("p_nrohojacontrareferencia", string.IsNullOrEmpty(atencion.nroHojaContraReferencia) ? "-" : (object)atencion.nroHojaContraReferencia);
        //            parameters.Add("p_fecparto", atencion.fecParto.HasValue ? atencion.fecParto.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_idgruporiesgo", string.IsNullOrEmpty(atencion.idGrupoRiesgo) ? "-" : (object)atencion.idGrupoRiesgo);
        //            parameters.Add("p_fecfallecimiento", atencion.fecFallecimiento.HasValue ? atencion.fecFallecimiento.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_renipressofertaflexible", string.IsNullOrEmpty(atencion.renipressOfertaFlexible) ? "-" : (object)atencion.renipressOfertaFlexible);
        //            parameters.Add("p_idetnia", string.IsNullOrEmpty(atencion.idEtnia) ? "-" : (object)atencion.idEtnia);
        //            parameters.Add("p_idiafas", string.IsNullOrEmpty(atencion.idIafas) ? "-" : (object)atencion.idIafas);
        //            parameters.Add("p_idcodigoiafas", string.IsNullOrEmpty(atencion.idCodigoIafas) ? "-" : (object)atencion.idCodigoIafas);
        //            parameters.Add("p_idups", string.IsNullOrEmpty(atencion.idUps) ? "-" : (object)atencion.idUps);                    
        //            parameters.Add("p_feccorteadministrativo", atencion.fecCorteAdministrativo.HasValue ? atencion.fecCorteAdministrativo.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_idudrautorizavinculado", string.IsNullOrEmpty(atencion.idUdrAutorizaVinculado) ? "-" : (object)atencion.idUdrAutorizaVinculado);
        //            parameters.Add("p_loteautorizavinculado", string.IsNullOrEmpty(atencion.loteAutorizaVinculado) ? "-" : (object)atencion.loteAutorizaVinculado);
        //            parameters.Add("p_nroautorizavinculado", string.IsNullOrEmpty(atencion.nroAutorizaVinculado) ? "-" : (object)atencion.nroAutorizaVinculado);
        //            parameters.Add("p_disafuavinculado", string.IsNullOrEmpty(atencion.disaFuaVinculado) ? "-" : (object)atencion.disaFuaVinculado);
        //            parameters.Add("p_lotefuavinculado", string.IsNullOrEmpty(atencion.loteFuaVinculado) ? "-" : (object)atencion.loteFuaVinculado);
        //            parameters.Add("p_nrofuavinculado", string.IsNullOrEmpty(atencion.nroFuaVinculado) ? "-" : (object)atencion.nroFuaVinculado);
        //            parameters.Add("p_idtipodocrespate", string.IsNullOrEmpty(atencion.idTipoDocRespAte) ? "-" : (object)atencion.idTipoDocRespAte);
        //            parameters.Add("p_numdocrespate", string.IsNullOrEmpty(atencion.numDocRespAte) ? "-" : (object)atencion.numDocRespAte);
        //            parameters.Add("p_idtipopersonalsalud", string.IsNullOrEmpty(atencion.idTipoPersonalSalud) ? "-" : (object)atencion.idTipoPersonalSalud);
        //            parameters.Add("p_idespecialidadrespate", string.IsNullOrEmpty(atencion.idEspecialidadRespAte) ? "-" : (object)atencion.idEspecialidadRespAte);
        //            parameters.Add("p_esegresadorespate", string.IsNullOrEmpty(atencion.esEgresadoRespAte) ? "-" : (object)atencion.esEgresadoRespAte);
        //            parameters.Add("p_colegiaturarespate", string.IsNullOrEmpty(atencion.colegiaturaRespAte) ? "-" : (object)atencion.colegiaturaRespAte);
        //            parameters.Add("p_rnerespate", string.IsNullOrEmpty(atencion.rneRespAte) ? "-" : (object)atencion.rneRespAte);
        //            parameters.Add("p_idtipodocdigitador", string.IsNullOrEmpty(atencion.idTipoDocDigitador) ? "-" : (object)atencion.idTipoDocDigitador);
        //            parameters.Add("p_numdocdigitador", string.IsNullOrEmpty(atencion.numDocDigitador) ? "-" : (object)atencion.numDocDigitador);
        //            parameters.Add("p_fechoraregistro", atencion.fecHoraRegistro.HasValue ? atencion.fecHoraRegistro.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_observacion", string.IsNullOrEmpty(atencion.observacion) ? "-" : (object)atencion.observacion);
        //            parameters.Add("p_versionaplicativo", string.IsNullOrEmpty(atencion.versionAplicativo) ? "-" : (object)atencion.versionAplicativo);
        //            parameters.Add("p_codigoacreditacion", string.IsNullOrEmpty(atencion.codigoAcreditacion) ? "-" : (object)atencion.codigoAcreditacion);
        //            parameters.Add("p_fechorainifuaadm", atencion.fecHoraIniFuaAdm.HasValue ? atencion.fecHoraIniFuaAdm.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_fechorafinfuaadm", atencion.fecHoraFinFuaAdm.HasValue ? atencion.fecHoraFinFuaAdm.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_idmotivoingresocasamaterna", atencion.idMotivoIngresoCasaMaterna.HasValue ? (object)atencion.idMotivoIngresoCasaMaterna.Value : 0);
        //            parameters.Add("p_idcasamaterna", atencion.idCasaMaterna == null ? 0 : (object)atencion.idCasaMaterna);
        //            parameters.Add("p_fechoracrea", atencion.control.fecHoraCrea.HasValue ? atencion.control.fecHoraCrea.Value.ToString("dd/MM/yyyy") : "");
        //            parameters.Add("p_idestado", atencion.idEstado == null ? 0 : (object)atencion.idEstado);

        //            await connection.ExecuteAsync(
        //                "public.sp_registrar_fua",
        //                parameters,
        //                commandType: CommandType.StoredProcedure
        //            );

        //            return (long)atencion?.idAtencion; // parameters.Get<int>("p_idatencion");
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //}



    }


}
