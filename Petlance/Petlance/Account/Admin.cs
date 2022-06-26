using MySqlConnector;
using Reader = MySqlConnector.MySqlDataReader;
using Command = MySqlConnector.MySqlCommand;

namespace Petlance
{
    class Admin : IAccount
    {
        private string login;

        public Admin(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get => login; set => login = value; }
        public int Id { get; set; }
        public string Password { get; set; }
    }
}