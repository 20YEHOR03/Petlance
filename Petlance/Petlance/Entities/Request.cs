
using System;
using System.Collections.Generic;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;
namespace Petlance
{
    class Request
    {
        public Request(int id, User user, Offer offer, DateTime time, Dictionary<Animal, int> animals, string other, string desc)
        {
            Id = id;
            User = user;
            Offer = offer;
            Time = time;
            Animals = animals;
            Other = other;
            Desc = desc;
        }

        public int Id { get; set; }
        public User User { get; set; }
        public Offer Offer { get; set; }
        public DateTime Time { get; set; }
        public Dictionary<Animal, int> Animals { get; set; }
        public string Other { get; set; }
        public string Desc { get; set; }
        public bool Delete()
        {
            using Database database = new Database();
            return database.Delete(Id, "request");
        }
        public void Send()
        {
            using Database database = new Database();
            Command command = database.Command("BEGIN; INSERT INTO `request`(`offer`, `user`, `time`, `other`, `desc`) VALUES (@offer, @user, @time, @other, @desc); SELECT LAST_INSERT_ID(); COMMIT;");
            command.Parameters.Add("@offer", SqlType.Int32).Value = Offer.Id;
            command.Parameters.Add("@user", SqlType.Int32).Value = User.Id;
            command.Parameters.Add("@time", SqlType.DateTime).Value = Time;
            command.Parameters.Add("@other", SqlType.String).Value = Other;
            command.Parameters.Add("@desc", SqlType.String).Value = Desc;
            using (Reader reader = command.ExecuteReader())
                if (reader.Read())
                    Id = reader.GetInt32(0);
            command = database.Command("INSERT INTO `request_animal`(`request`,`animal`,`count`) VALUES (@request, @animal, @count)");
            foreach(var animal in Animals)
            {
                command.Parameters.Add("@request", SqlType.Int32).Value = Id;
                command.Parameters.Add("@animal", SqlType.Int32).Value = animal.Key.Type;
                command.Parameters.Add("@count", SqlType.Int32).Value = animal.Value;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }

        }
    }
}