using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindTaskHelperWebApplication.Models
{
    public class DatabaseConnection
    {
        private NpgsqlConnection _DatabaseConnection = new NpgsqlConnection("Server=127.0.0.1;User Id=postgres;" +
                                "Password=12345678;Database=FindTaskHelperWebApp;");

        public NpgsqlConnection GetDatabaseConnection()
        {
            return _DatabaseConnection;
        }
    }
}
