using MySqlConnector;
using System;
using Reader = MySqlConnector.MySqlDataReader;
using Command = MySqlConnector.MySqlCommand;

namespace Petlance
{
    class Database : IDisposable
    {
        public static MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder()
        {
            Server = "192.168.0.128",
            Database = "petlance",
            UserID = "petlance",
            Port = 3306,
            Password = "8UQV4geB5Qyk*aIZ"
        };

        public MySqlConnection Connection { get; set; }

        public Database()
        {
            Connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
            Open();
        }
        public void Open() => Connection.Open();
        public void Close() => Connection.Close();
        public Command Command(string query) => new Command(query, Connection);
        public Reader Reader(Command command) => command.ExecuteReader();
        public bool Delete(int id, string table)
        {
            Command command = Command($"DELETE FROM `{table}` WHERE `id`=@id");
            command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
            return command.ExecuteNonQuery() > 0;
        }
        public bool Check(int id, string table)
        {
            Command command = Command($"SELECT * FROM `{table}` WHERE `id`=@id");
            command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
            using Reader reader = command.ExecuteReader();
            return reader.HasRows;
        }
        public void Dispose() => Connection.Dispose();
    }
}