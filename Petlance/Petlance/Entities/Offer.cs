using Android.Graphics;
using System.Collections.Generic;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    public class Offer
    {
        public string UpdateQuery => "UPDATE `offer` SET `title`=@title," +
                        "`short_desc`=@short_desc," +
                        "`description`=@description," +
                        "`initial_price`=@initial_price," +
                        "`contacts`=@contacts," +
                        "`is_active`=@is_active," +
                        "`executor`=@executor," +
                        "`entopped`=@entopped " +
                        "WHERE `id`=@id";
        public string InsertQuery => "BEGIN;"
            + "INSERT INTO `offer` "
            + "(`title`, `short_desc`, `description`, `initial_price`, `contacts`, `is_active`, `executor`, `entopped`) VALUES "
            + "(@title,  @short_desc,  @description,  @initial_price,  @contacts,  @is_active,  @executor,  @entopped);"
            + "SELECT LAST_INSERT_ID();"
            + "COMMIT;";

        public Offer(int id,
                     string title,
                     string shortDescription,
                     string description,
                     int initialPrice,
                     string contacts,
                     bool isActive,
                     Executor executor,
                     bool entopped,
                     Animal[] animals,
                     byte[][] photos)
        {
            Title = title;
            ShortDescription = shortDescription;
            Description = description;
            InitialPrice = initialPrice;
            Contacts = contacts;
            IsActive = isActive;
            Executor = executor;
            Entopped = entopped;
            Animals = animals;
            Photos = photos;
            Id = id;
        }

        public Offer() { }
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int InitialPrice { get; set; }
        public string Contacts { get; set; }
        public bool IsActive { get; set; } = true;
        public Executor Executor { get; set; }
        public bool Entopped { get; set; } = false;
        public Animal[] Animals { get; set; }
        public byte[][] Photos { get; set; }
        public static Offer GetOfferById(int id, Executor executor = null)
        {
            Offer offer = null;
            using Database database = new Database();
            Command command = database.Command("SELECT * FROM `offer` WHERE `id`=@id");
            command.Parameters.Add("@id", SqlType.Int32).Value = id;
            using (Reader reader = command.ExecuteReader())
                if (reader.Read())
                    offer = new Offer(id: id,
                                      title: reader.GetString(reader.GetOrdinal("title")),
                                      shortDescription: reader.GetString(reader.GetOrdinal("short_desc")),
                                      description: reader.GetString(reader.GetOrdinal("description")),
                                      initialPrice: reader.GetInt32(reader.GetOrdinal("initial_price")),
                                      contacts: reader.GetString(reader.GetOrdinal("contacts")),
                                      isActive: reader.GetBoolean(reader.GetOrdinal("is_active")),
                                      executor: executor ?? Executor.GetExecutorById(reader.GetInt32(reader.GetOrdinal("executor"))),
                                      entopped: reader.GetBoolean(reader.GetOrdinal("entopped")),
                                      animals: null,
                                      photos: null);

            FillArrays(id, offer);

            return offer;
        }

        private static void FillArrays(int id, Offer offer)
        {
            using Database database = new Database();
            List<Animal> animals = new List<Animal>();
            Command command = database.Command("SELECT * FROM `animal` WHERE `offer`=@offer");
            command.Parameters.Add("@offer", SqlType.Int32).Value = id;
            using (Reader reader = command.ExecuteReader())
                while (reader.Read())
                    animals.Add(new Animal(
                        type: reader.GetInt32(reader.GetOrdinal("type")),
                        price: reader.GetInt32(reader.GetOrdinal("price"))));
            offer.Animals = animals.ToArray();
            List<byte[]> photos = new List<byte[]>();
            command = database.Command("SELECT `photo` FROM `offer_photo` WHERE `offer`=@offer");
            command.Parameters.Add("@offer", SqlType.Int32).Value = id;
            using (Reader reader = command.ExecuteReader())
                while (reader.Read())
                    photos.Add((byte[])reader[0]);
            offer.Photos = photos.ToArray();
        }

        public void Update()
        {
            using Database database = new Database();
            bool check = database.Check(Id, "offer");
            Command command = database.Command(check ? UpdateQuery : InsertQuery);
            command.Parameters.Add("@title", SqlType.String).Value = Title;
            command.Parameters.Add("@short_desc", SqlType.String).Value = ShortDescription;
            command.Parameters.Add("@description", SqlType.Text).Value = Description;
            command.Parameters.Add("@initial_price", SqlType.Int32).Value = InitialPrice;
            command.Parameters.Add("@contacts", SqlType.String).Value = Contacts;
            command.Parameters.Add("@is_active", SqlType.Bool).Value = IsActive;
            command.Parameters.Add("@executor", SqlType.Int32).Value = Executor.Id;
            command.Parameters.Add("@entopped", SqlType.Bool).Value = Entopped;
            if (check)
            {
                command.Parameters.Add("@id", SqlType.Int32).Value = Id;
                check = command.ExecuteNonQuery() > 0;
            }
            else
            {
                using Reader reader = command.ExecuteReader();
                if (reader.Read())
                    Id = reader.GetInt32(0);
            }
            command = database.Command("DELETE FROM `animal` WHERE `offer`=@offer");
            command.Parameters.Add("@offer", SqlType.Int32).Value = Id;
            command.ExecuteNonQuery();
            command = database.Command("INSERT INTO `animal`(`type`, `price`, `offer`) VALUES (@type, @price, @offer)");
            if (Animals != null)
                foreach (Animal animal in Animals)
                {
                    command.Parameters.Add("@offer", SqlType.Int32).Value = Id;
                    command.Parameters.Add("@type", SqlType.Int32).Value = animal.Type;
                    command.Parameters.Add("@price", SqlType.Int32).Value = animal.Price;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            command = database.Command("DELETE FROM `offer_photo` WHERE `offer`=@offer");
            command.Parameters.Add("@offer", SqlType.Int32).Value = Id;
            command.ExecuteNonQuery();
            command = database.Command("INSERT INTO `offer_photo`(`photo`, `offer`) VALUES (@photo, @offer)");
            if (Photos != null)
                foreach (byte[] bitmap in Photos)
                {
                    command.Parameters.Add("@offer", SqlType.Int32).Value = Id;
                    command.Parameters.Add("@photo", SqlType.Blob).Value = bitmap;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
        }
        public bool IsFavorite(User user)
        {
            using Database database = new Database();
            Command command = database.Command("SELECT * FROM `favorites` WHERE `offer`=@offer AND `user`=@user");
            command.Parameters.Add("@offer", SqlType.Int32).Value = Id;
            command.Parameters.Add("@user", SqlType.Int32).Value = user.Id;
            using Reader reader = command.ExecuteReader();
            return reader.HasRows;
        }
        public bool Delete()
        {
            using Database database = new Database();
            return database.Delete(Id, "offer");

        }
        public Dictionary<int, Animal> GetAnimalDictionary()
        {
            Dictionary<int, Animal> dict = new Dictionary<int, Animal>();
            foreach (Animal animal in Animals)
                dict.Add(animal.Type, animal);
            return dict;
        }
    }
}