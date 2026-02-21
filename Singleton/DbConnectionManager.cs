using System;
using Npgsql;

namespace WindowsFormsApp.Singleton
{
    /// <summary>
    /// Singleton Pattern:
    /// Centralized PostgreSQL connection manager.
    /// </summary>
    public sealed class DbConnectionManager
    {
        private static readonly Lazy<DbConnectionManager> _instance =
            new(() => new DbConnectionManager());

        // TODO: replace with your real values
        private readonly string _connectionString =
            "Host=localhost;Port=5433;Database=bookstore_db;Username=postgres;Password=2003;";

        private DbConnectionManager()
        {
        }

        public static DbConnectionManager Instance => _instance.Value;

        public NpgsqlConnection CreateConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);
            return conn;
        }
    }
}


