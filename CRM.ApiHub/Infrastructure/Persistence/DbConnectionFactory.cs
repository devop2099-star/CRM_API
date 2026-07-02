using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CRM.ApiHub.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public NpgsqlConnectionFactory(IConfiguration config)
    {
        var rawConnectionString = config.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException(nameof(config), "La cadena de conexión 'DefaultConnection' no está configurada.");
            
        // Desencripta si la cadena comienza con "Encrypted:"
        _connectionString = EncryptionHelper.Decrypt(rawConnectionString);
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}
