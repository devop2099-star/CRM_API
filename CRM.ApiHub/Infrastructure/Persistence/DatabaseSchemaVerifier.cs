using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Logging;

namespace CRM.ApiHub.Infrastructure.Persistence;

public interface IDatabaseSchemaVerifier
{
    Task<bool> VerifySchemaAsync();
}

public class DatabaseSchemaVerifier : IDatabaseSchemaVerifier
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseSchemaVerifier> _logger;

    public DatabaseSchemaVerifier(IDbConnectionFactory connectionFactory, ILogger<DatabaseSchemaVerifier> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<bool> VerifySchemaAsync()
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("    NYX CRM - DATABASE & SCHEMA VERIFICATION SYSTEM     ");
        Console.WriteLine("========================================================\n");

        IDbConnection connection;
        try
        {
            connection = _connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                // NpgsqlConnection doesn't open on creation, we need to open it
                if (connection is System.Data.Common.DbConnection dbConn)
                {
                    await dbConn.OpenAsync();
                }
                else
                {
                    connection.Open();
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Connection status: CONNECTED SUCCESSFULLY");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✗ Connection status: CONNECTION FAILED!");
            Console.WriteLine($"Error detail: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\n========================================================\n");
            return false;
        }

        using (connection)
        {
            // Find all entity types with [Table] attribute in the assembly of Campaign
            var entityTypes = typeof(Campaign).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TableAttribute>() != null)
                .OrderBy(t => t.Name)
                .ToList();

            Console.WriteLine($"Found {entityTypes.Count} domain entities to verify:\n");

            int passedTablesCount = 0;
            int failedTablesCount = 0;

            foreach (var type in entityTypes)
            {
                var tableAttr = type.GetCustomAttribute<TableAttribute>()!;
                var tableName = tableAttr.Name;
                var schemaName = tableAttr.Schema;
                var fullTableName = string.IsNullOrEmpty(schemaName) 
                    ? $"\"{tableName}\"" 
                    : $"\"{schemaName}\".\"{tableName}\"";

                // Get all properties with [Column] attribute
                var propertiesWithColumns = type.GetProperties()
                    .Select(p => new { Property = p, ColumnAttr = p.GetCustomAttribute<ColumnAttribute>() })
                    .Where(x => x.ColumnAttr != null)
                    .ToList();

                if (!propertiesWithColumns.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠ [Entity: {type.Name}] No columns mapped via [Column] attribute. Skipping schema check.");
                    Console.ResetColor();
                    continue;
                }

                // We verify the table structure by querying the columns
                var columnNames = propertiesWithColumns.Select(x => $"\"{x.ColumnAttr!.Name}\"").ToList();
                var selectColumnsString = string.Join(", ", columnNames);
                var testQuery = $"SELECT {selectColumnsString} FROM {fullTableName} LIMIT 0;";

                try
                {
                    await connection.ExecuteAsync(testQuery);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ Table {fullTableName,-45} | Status: VALID (All {propertiesWithColumns.Count} columns match)");
                    Console.ResetColor();
                    passedTablesCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ Table {fullTableName,-45} | Status: ERROR!");
                    Console.ResetColor();
                    
                    // Detail error message to guide developer
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"  └─ Error: {ex.Message.Trim()}");
                    Console.ResetColor();
                    failedTablesCount++;
                }
            }

            Console.WriteLine("\n--------------------------------------------------------");
            Console.WriteLine($"VERIFICATION SUMMARY:");
            Console.WriteLine($"- Total Entities Mapped: {entityTypes.Count}");
            
            if (passedTablesCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"- Successful Tables   : {passedTablesCount}");
                Console.ResetColor();
            }
            if (failedTablesCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[DIAGNOSTIC] Listing all non-system tables in the database:");
                Console.ResetColor();
                try
                {
                    const string listTablesQuery = @"
                        SELECT table_schema, table_name 
                        FROM information_schema.tables 
                        WHERE table_schema NOT IN ('pg_catalog', 'information_schema') 
                        ORDER BY table_schema, table_name;";
                    
                    var existingTables = (await connection.QueryAsync(listTablesQuery))
                        .Select(t => new { Schema = (string)((dynamic)t).table_schema, Name = (string)((dynamic)t).table_name })
                        .ToList();

                    if (existingTables.Any())
                    {
                        foreach (var t in existingTables)
                        {
                            Console.WriteLine($"  - \"{t.Schema}\".\"{t.Name}\"");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  (No tables found in this database database/catalog)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Failed to list tables: {ex.Message}");
                }
            }
            Console.WriteLine("========================================================\n");

            return failedTablesCount == 0;
        }
    }
}
