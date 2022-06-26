using Android.Graphics;
using System;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    class Executor : User
    {
        public Executor(string name,
                        string password,
                        string tel,
                        string email,
                        int paws,
                        byte[] picture,
                        int id,
                        byte[] document,
                        DateTime date)
            : base(name, password, tel, email, paws, picture, id)
        {
            Document = document;
            Date = date;
        }
        public Executor(User user,
                        byte[] document,
                        DateTime date)
            : base(user.Name, user.Password, user.Phone, user.Email, user.Paws, user.Picture, user.Id)
        {
            Document = document;
            Date = date;
        }

        public byte[] Document { get; set; }
        public Review[] Reviews { get; set; }
        public DateTime Date { get; set; }
        internal Offer[] Offers { get; set; }
        public float GetRating()
        {
            using Database db = new Database();
            Command command = db.Command("SELECT AVG(`rate`) FROM `review` WHERE `executor`=@id");
            command.Parameters.Add("@id", SqlType.Int32).Value = Id;
            using Reader reader = command.ExecuteReader();
            if (reader.Read() && !reader.IsDBNull(0))
                return reader.GetFloat(0);
            else return 0;
        }
        public static Executor GetExecutorById(int id)
        {
            using Database database = new Database();
            Command command = database.Command("SELECT * FROM `user`, `executor` WHERE `user`.`id`=@id AND `user`.`id`=`executor`.`user`");
            command.Parameters.Add("@id", SqlType.Int32).Value = id;
            using Reader reader = command.ExecuteReader();
            return (reader.Read()) ? new Executor(name: reader.GetString(reader.GetOrdinal("name")),
                                                  password: reader.GetString(reader.GetOrdinal("password")),
                                                  tel: reader.GetString(reader.GetOrdinal("tel")),
                                                  email: reader.GetString(reader.GetOrdinal("email")),
                                                  paws: reader.GetInt32(reader.GetOrdinal("paws")),
                                                  id: reader.GetInt32(reader.GetOrdinal("id")),
                                                  picture: (byte[])reader["picture"],
                                                  document: (byte[])reader["document"],
                                                  date: reader.GetDateTime(reader.GetOrdinal("date"))) : null;
        }
        public bool VerifyDocument(byte[] document)
        {
            Document = document;
            using Database db = new Database();
            Command command = db.Command("SELECT `user` FROM `executor` WHERE `user`=@id");
            command.Parameters.Add("@id", SqlType.Int32).Value = Id;
            bool check;
            using (Reader reader = command.ExecuteReader())
                check = reader.HasRows;
            command = db.Command(check ? "UPDATE `executor` SET `document`=@document WHERE `user`=@id"
                : "INSERT INTO `executor` (`user`, `document`, `date`) VALUES (@id, @document, @date)");
            command.Parameters.Add("@id", SqlType.Int32).Value = Id;
            command.Parameters.Add("@document", SqlType.Blob).Value = Document;
            if (!check)
                command.Parameters.Add("@date", SqlType.Date).Value = Date = DateTime.Now;
            return command.ExecuteNonQuery() > 0;
        }
    }
}