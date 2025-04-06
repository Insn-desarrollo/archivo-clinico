using Dapper;
using INSN.ArchivoClinico.Domain.Interfaces;
using System.Data;

namespace INSN.ArchivoClinico.Infrastructure.Services;

public class PgDatabaseService : IPgDatabaseService
{
    private readonly IDbConnection _connection;

    public PgDatabaseService(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        return await _connection.QueryAsync<T>(sql, param) ?? [];
    }

    public async Task<T?> QueryFirstAsync<T>(string sql, object? param = null)
    {
        return await _connection.QueryFirstOrDefaultAsync<T>(sql, param);
    }

    public Task<bool> SaveAsync()
    {
        // Dapper no tiene SaveChanges, solo ejecutas el SQL directamente.
        // Si usas transacciones o INSERT/UPDATE, debes implementarlo.
        return Task.FromResult(true);
    }

    public IDbConnection GetConnection() => _connection;
}
