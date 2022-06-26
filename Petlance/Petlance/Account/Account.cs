using System.Text.RegularExpressions;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    public interface IAccount
    {
        public static Regex passwordPattern = new Regex(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-z]).{8,}$");
        //public static Regex emailPattern = new Regex(@"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|" + '"' + @"(?:[\x01 -\x08\x0b\x0c\x0e -\x1f\x21\x23 -\x5b\x5d -\x7f] |\\[\x01 -\x09\x0b\x0c\x0e -\x7f])*" + '"' + @")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$");
        public int Id { get; set; }
        public string Password { get; set; }
        public static IAccount Log_in(string login, string password)
        {
            using Database db = new Database();
            IAccount account = null;
            Command command = db.Command($"SELECT * FROM `user` WHERE `tel`=@login OR `email`=@login");
            command.Parameters.Add("@login", SqlType.String).Value = login;
            using (Reader userReader = command.ExecuteReader())
                if (userReader.Read())
                {
                    string hash = userReader.GetString(userReader.GetOrdinal("password"));
                    if (SecurePasswordHasher.Verify(password, hash))
                        account = new User(name: userReader.GetString(userReader.GetOrdinal("name")),
                                           password: hash,
                                           phone: userReader.GetString(userReader.GetOrdinal("tel")),
                                           email: userReader.GetString(userReader.GetOrdinal("email")),
                                           paws: userReader.GetInt32(userReader.GetOrdinal("paws")),
                                           picture: (byte[])userReader["picture"],
                                           id: userReader.GetInt32(userReader.GetOrdinal("id")));
                }
            command = db.Command($"SELECT * FROM `admin` WHERE `login`=@login");
            command.Parameters.Add("@login", SqlType.String).Value = login;
            using (Reader adminReader = command.ExecuteReader())
                if (adminReader.Read())
                {
                    string hash = adminReader.GetString(adminReader.GetOrdinal("password"));
                    if (SecurePasswordHasher.Verify(password, hash))
                        account = new Admin(
                            login: adminReader.GetString(adminReader.GetOrdinal("login")),
                            password: hash)
                        { Id = adminReader.GetInt32(adminReader.GetOrdinal("id")) };
                }
            if (account is User)
            {
                command = db.Command($"SELECT * FROM `executor` WHERE `user`=@user");
                command.Parameters.Add("@user", SqlType.Int64).Value = account.Id;
                using (Reader executorReader = command.ExecuteReader())
                    if (executorReader.Read())
                    {
                        account = new Executor(account as User,
                                               document: (byte[])executorReader["document"],
                                               date: executorReader.GetDateTime(executorReader.GetOrdinal("date")));
                    }
            }
            return account;
        }
        public static bool CheckPassword(string password) => passwordPattern.IsMatch(password);
    }
}