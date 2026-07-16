using System;
using Npgsql;

namespace DecryptApp;

class Program
{
    static void Main()
    {
        string connStr = "Host=10.10.40.15;Port=5432;Database=nyx_crm;Username=usr_consultas;Password=Gs1#2099zx;Search Path=user_service,public;";
        try
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();
            Console.WriteLine("Connection opened successfully!");

            using var cmd = new NpgsqlCommand(@"
                SELECT table_schema, table_name 
                FROM information_schema.tables 
                WHERE table_schema NOT IN ('pg_catalog', 'information_schema', 'pg_toast')
                ORDER BY table_schema, table_name;", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"SCHEMA: {reader.GetString(0)} | TABLE: {reader.GetString(1)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
