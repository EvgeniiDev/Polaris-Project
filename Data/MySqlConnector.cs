using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceApiDataParser.Data
{
    class MySqlConnector
    {
        public static MySqlConnection
        GetDBConnection(string host, int port, string database, string username, string password)
        {
            String connString = $"Server={host};Database={database};port={port};User Id={username};password={password}";
            MySqlConnection conn = new MySqlConnection(connString);
            return conn;
        }
        public static MySqlConnection GetDBConnection()
        {
            string host = "127.0.0.1";
            int port = 3306;
            string database = "crypto";
            string username = "root";
            string password = "";

            return GetDBConnection(host, port, database, username, password);
        }
    }
}
