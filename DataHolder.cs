using Enumeration;

using System.Collections.Generic;

using MYSQLCONNECTION = MySql.Data.MySqlClient.MySqlConnection;

namespace Utilities.MySQL
{
    public static unsafe class DataHolder
    {
        public static string ConnectionString;
        public static List<string> TablesName = new List<string>();
        public static bool CreateConnection(string password = "ahmed010930", string database = "tq")
        {
            ConnectionString = "Server=localhost" + ";Port=3306;Database=" + database + ";Uid=root" + ";Password=" + password + ";Persist Security Info=True;Pooling=true; Min Pool Size = 32;  Max Pool Size = 300;"; return true;
        }


        public static void ExcuteDB(ProjType type)
        {
            string myConnectionString = "SERVER=localhost;" + "DATABASE=tq;" + "UID=root;" + "PASSWORD=ahmed010930;";
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"select TABLE_NAME from information_schema.TABLES where TABLE_SCHEMA='tq'";
            MySql.Data.MySqlClient.MySqlDataReader Reader;
            connection.Open();
            Reader = command.ExecuteReader();

            string row = "";
            while (Reader.Read())
            {
                for (int i = 0; i < Reader.FieldCount; i++)
                {
                    row = Reader.GetValue(i).ToString();
                }
                if (row == "accounts")
                    continue;
                if (type == ProjType.Game && row == "cq_servers")
                    continue;
                if (type == ProjType.Auth && row != "cq_servers")
                    continue;
                TablesName.Add(row);
            }
            connection.Close();
        }
        public static MYSQLCONNECTION MySqlConnection
        {
            get
            {
                MYSQLCONNECTION conn = new MYSQLCONNECTION
                {
                    ConnectionString = ConnectionString
                };
                return conn;
            }
        }
    }
}