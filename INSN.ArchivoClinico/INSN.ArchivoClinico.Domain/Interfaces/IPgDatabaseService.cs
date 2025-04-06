using System.Data;

namespace INSN.ArchivoClinico.Domain.Interfaces;

public interface IPgDatabaseService
{
    Task<bool> SaveAsync(); // opcional, según tu uso
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
    Task<T?> QueryFirstAsync<T>(string sql, object? param = null);
    IDbConnection GetConnection(); // usamos IDbConnection directamente
}
