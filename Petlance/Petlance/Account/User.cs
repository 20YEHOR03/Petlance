using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using System.IO;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    class User : IAccount, Entity
    {
        public User()
        { }
        public User(string name, string password, string phone, string email, uint? paws, Bitmap picture, int id)
        {
            Name = name;
            Password = password;
            Phone = phone;
            Email = email;
            Paws = paws;
            Picture = picture;
            Id = id;
        }
        public User(User user) : this(user.Name, user.Email, user.Phone, user.Password, user.Paws, user.Picture, user.Id)
        { }

        public string Name { get; set; } = null;
        public string Password { get; set; }
        public uint? Paws { get; set; } = null;
        public string Email { get; set; }
        public string Phone { get; set; } = null;
        public int Id { get; set; }
        public Bitmap Picture { get; set; }

        public string InsertQuery => "INSERT INTO `user` (`name`, `tel`, `email`, `password`, `paws`, `picture`) "
            + "VALUES (@name, @tel, @email, @password, @paws, @picture)";

        public string UpdateQuery => "UPDATE `user` "
            + "SET `name`=@name, "
            + "`tel`=@tel, "
            + "`email`=@email, "
            + "`password`=@password, "
            + "`paws`=@paws, "
            + "`picture`=@picture "
            + "WHERE `id`=@id";

        public bool Delete()
        {
            using Database database = new Database();
            return database.Delete(Id, "user");
        }
        public void Update() => TryUpdate();
        public bool TryUpdate()
        {
            using Database database = new Database();
            bool check = database.Check(Id, "user");
            Command command = database.Command(check ? UpdateQuery : InsertQuery);
            command.Parameters.Add("@name", SqlType.String).Value = Name;
            command.Parameters.Add("@tel", SqlType.String).Value = Phone;
            command.Parameters.Add("@email", SqlType.String).Value = Email;
            command.Parameters.Add("@password", SqlType.String).Value = Password;
            command.Parameters.Add("@paws", SqlType.Int32).Value = Paws;
            command.Parameters.Add("@picture", SqlType.Blob).Value = Images.GetBytesFromBitmap(Picture);
            if (check)
                command.Parameters.Add("@id", SqlType.Int32).Value = Id;
            return command.ExecuteNonQuery() > 0;
        }
        public bool Register() => TryUpdate();

        public bool AddFavorite(Offer offer)
        {
            using Database database = new Database();
            Command command = database.Command(
                "INSERT INTO `favorites`(`user`, `offer`) " +
                "VALUES (@user, @offer)");
            command.Parameters.Add("@user", SqlType.Int32).Value = Id;
            command.Parameters.Add("@offer", SqlType.Int32).Value = offer.Id;
            return command.ExecuteNonQuery() > 0;
        }

        public bool RemoveFavorite(Offer offer)
        {
            using Database database = new Database();
            Command command = database.Command(
                "DELETE FROM `favorites` " +
                "WHERE `user`=@user AND `offer`=@offer ");
            command.Parameters.Add("@user", SqlType.Int32).Value = Id;
            command.Parameters.Add("@offer", SqlType.Int32).Value = offer.Id;
            return command.ExecuteNonQuery() > 0;
        }

        public static User GetUserById(int id)
        {
            User user = null;
            using Database database = new Database();
            Command command = database.Command("SELECT * FROM `user` WHERE `id`=@id");
            command.Parameters.Add("@id", SqlType.Int32).Value = id;
            using (Reader reader = command.ExecuteReader())
                if (reader.Read())
                {
                    user = new User(name: reader.GetString(reader.GetOrdinal("name")),
                                    password: reader.GetString(reader.GetOrdinal("password")),
                                    phone: reader.GetString(reader.GetOrdinal("tel")),
                                    email: reader.GetString(reader.GetOrdinal("email")),
                                    paws: (uint)reader.GetInt32(reader.GetOrdinal("paws")),
                                    picture: Images.GetBitmapFromBytes((byte[])reader["picture"]),
                                    id: id);
                }
            return user;
        }
        public static Bitmap GetDefaultAvatar(Context context) => BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.profile);
        public bool SendVerification(byte[] document)
        {
            try
            {
                using Database db = new Database();
                Command command = db.Command("INSERT INTO `verification`(`user`, `document`) VALUES (@user, @document)");
                command.Parameters.Add("@user", SqlType.Int32).Value = Id;
                command.Parameters.Add("@document", SqlType.Blob).Value = document;
                return command.ExecuteNonQuery() > 0;
            }
            catch { }
            return false;
        }

    }
}