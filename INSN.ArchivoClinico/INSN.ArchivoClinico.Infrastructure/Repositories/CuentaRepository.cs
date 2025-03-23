using System;
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
//using INSN.ArchivoClinico.Application.DTOs;

namespace INSN.ArchivoClinico.Infrastructure.Repositories
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;
        public CuentaRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<List<TipoFinanciamientoDto>> ObtenerTiposFinanciamientoAsync(int fuenteFinanciamientoId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @" SELECT 
            tf.tipo_financiamiento_id, 
            tf.descripcion
            FROM maestro.tipo_financiamiento tf
	        INNER JOIN maestro.fuente_financiamiento ff on ff.tipo_financiamiento_id = tf.tipo_financiamiento_id
            WHERE ff.fuente_financiamiento_id = @fuente_financiamiento_id;            
            ";

            var parameters = new { fuente_financiamiento_id = fuenteFinanciamientoId };

            var result = await connection.QueryAsync<TipoFinanciamientoDto>(query, parameters);

            return result.ToList();

            //var tiposFinanciamiento = new List<TipoFinanciamientoDto>();

            //try
            //{
            //    using (var connection = new NpgsqlConnection(_connectionString))
            //    {
            //        await connection.OpenAsync();

            //        string sql = "SELECT * FROM public.sp_obtener_tipos_financiamiento(@FuenteFinanciamientoId);";

            //        using (var command = new NpgsqlCommand(sql, connection))
            //        {
            //            command.Parameters.AddWithValue("@FuenteFinanciamientoId", fuenteFinanciamientoId);

            //            using (var reader = await command.ExecuteReaderAsync())
            //            {
            //                while (await reader.ReadAsync())
            //                {
            //                    var tipo = new TipoFinanciamientoDto
            //                    {
            //                        tipo_financiamiento_id = reader.GetInt32(reader.GetOrdinal("TipoFinanciamientoId")),
            //                        descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion"))
            //                            ? null
            //                            : reader.GetString(reader.GetOrdinal("Descripcion"))
            //                    };

            //                    tiposFinanciamiento.Add(tipo);
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Manejo de excepciones
            //    Console.WriteLine($"Error en ObtenerTiposFinanciamientoAsync: {ex.Message}");
            //    throw;
            //}

            //return tiposFinanciamiento;
        }


        public async Task<IEnumerable<AtencionConsultaDto>> ConsultarCuentasAsync(AtencionFiltro filtro)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = @"SELECT * FROM public.fn_auditoria_bandeja_cuentas(
                     @p_fecha::date,
                     @p_nombre,
                     @p_historia_clinica,
                     @p_documento_identidad,
                     @p_nro_cuenta,
                     @p_codigo_estado,
                     @p_tipo_servicio,
                     @p_usuario,
                     @p_habilitar_fecha,
                     @p_page_number,
                     @p_page_size
                   );";

                var parametros = new
                {
                    p_fecha = filtro.Fecha ?? (object)DBNull.Value,
                    p_nombre = filtro.Nombre ?? string.Empty,
                    p_historia_clinica = filtro.HistoriaClinica ?? string.Empty,
                    p_documento_identidad = filtro.DocumentoIdentidad ?? string.Empty,
                    p_nro_cuenta = filtro.NroCuenta ?? string.Empty,
                    p_codigo_estado = filtro.CodigoEstado ?? (object)DBNull.Value,
                    p_tipo_servicio = filtro.TipoServicio ?? (object)DBNull.Value,
                    p_usuario = filtro.Usuario ?? string.Empty,
                    p_habilitar_fecha = filtro.HabilitarFecha,
                    p_page_number = filtro.Page,
                    p_page_size = filtro.PageSize
                };

                var resultados = await connection.QueryAsync<AtencionConsultaDto>(sql, parametros);

                return resultados;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        public async Task<IEnumerable<AtencionConsultaDto>> ConsultarSISAsync(AtencionFiltro filtro)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = @"SELECT * FROM public.fn_auditoria_bandeja_sis(
                     @p_fecha::date,
                     @p_nombre,
                     @p_historia_clinica,
                     @p_documento_identidad,
                     @p_nro_cuenta,
                     @p_codigo_estado,
                     @p_tipo_servicio,
                     @p_usuario,
                     @p_habilitar_fecha,
                     @p_page_number,
                     @p_page_size
                   );";

                var parametros = new
                {
                    p_fecha = filtro.Fecha ?? (object)DBNull.Value,
                    p_nombre = filtro.Nombre ?? string.Empty,
                    p_historia_clinica = filtro.HistoriaClinica ?? string.Empty,
                    p_documento_identidad = filtro.DocumentoIdentidad ?? string.Empty,
                    p_nro_cuenta = filtro.NroCuenta ?? string.Empty,
                    p_codigo_estado = filtro.CodigoEstado ?? (object)DBNull.Value,
                    p_tipo_servicio = filtro.TipoServicio ?? (object)DBNull.Value,
                    p_usuario = filtro.Usuario ?? string.Empty,
                    p_habilitar_fecha = filtro.HabilitarFecha,
                    p_page_number = filtro.Page,
                    p_page_size = filtro.PageSize
                };

                var resultados = await connection.QueryAsync<AtencionConsultaDto>(sql, parametros);

                return resultados;
            }
            catch (Exception ex)
            { 
                // Manejo de excepciones
                Console.WriteLine($"Error en ObtenerFuentesFinanciamientoAsync: {ex.Message}");
                throw;
            }
            
        }

        public async Task<List<FuenteFinanciamientoDto>> ObtenerFuentesFinanciamientoAsync()
        {
            var fuentes = new List<FuenteFinanciamientoDto>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM public.sp_obtener_fuentes_financiamiento();";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var fuente = new FuenteFinanciamientoDto
                                {
                                    fuente_financiamiento_id = reader.GetInt32(reader.GetOrdinal("FuenteFinanciamientoId")),
                                    descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("Descripcion"))
                                };

                                fuentes.Add(fuente);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en ObtenerFuentesFinanciamientoAsync: {ex.Message}");
                throw;
            }

            return fuentes;
        }              
        public async Task<TriajeResponseDto> ActualizarCuentaAsync(ActualizarCuentaRequest request)
        {
            TriajeResponseDto resultado = null;
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM public.actualizar_cuenta(@_AtencionId, @_AuditoriaCuentaCodigoEstado, @_AuditoriaObservacion, @_AuditoriaUsuario);";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        // Agregar parámetros
                        command.Parameters.AddWithValue("@_AtencionId", request.AtencionId);
                        command.Parameters.AddWithValue("@_AuditoriaCuentaCodigoEstado", request.AuditoriaCuentaCodigoEstado);
                        command.Parameters.AddWithValue("@_AuditoriaObservacion", (object)request.AuditoriaObservacion ?? "");                      
                        command.Parameters.AddWithValue("@_AuditoriaUsuario", request.AuditoriaUsuario);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                resultado = new TriajeResponseDto
                                {
                                    atencion_id_eess = reader.GetInt32(reader.GetOrdinal("atencion_id_eess")),
                                    cuenta_atencion_id = reader.GetInt32(reader.GetOrdinal("id_cuenta_atencion"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en ActualizarAtencionAsync: {ex.Message}");
                throw;
            }

            return resultado;
        }
        public async Task<TriajeResponseDto> ActualizarCuentaFUAAsync(ActualizarRespuestaFUARequest request)
        {
            TriajeResponseDto resultado = null;
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM public.actualizar_cuenta_fua(@_atencionEessid, @_fuaguid, @_fuaestado, @_fuamensaje, @_fuaadvertencia, @_AuditoriaUsuario);";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        // Agregar parámetros
                        command.Parameters.AddWithValue("@_atencionEessid", request.AtencionId);
                        command.Parameters.AddWithValue("@_fuaguid", request.FuaGuid);
                        command.Parameters.AddWithValue("@_fuaestado", request.FuaEstado);
                        command.Parameters.AddWithValue("@_fuamensaje", (object)request.FuaMensaje ?? "");
                        command.Parameters.AddWithValue("@_fuaadvertencia", (object)request.FuaAdvertencia ?? "");
                        command.Parameters.AddWithValue("@_AuditoriaUsuario", request.AuditoriaUsuario);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                resultado = new TriajeResponseDto
                                {
                                    atencion_id_eess = reader.GetInt32(reader.GetOrdinal("atencion_id_eess")),
                                    cuenta_atencion_id = reader.GetInt32(reader.GetOrdinal("id_cuenta_atencion"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en ActualizarAtencionAsync: {ex.Message}");
                throw;
            }

            return resultado;
        }     
       
    }


}
