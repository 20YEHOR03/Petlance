using System;
using System.Collections.Generic;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;
namespace Petlance
{
    public class Review
    {
        public Review(int id, User user, Executor executor, string description, float rate, Animal[] animals, DateTime date)
        {
            Id = id;
            User = user;
            Executor = executor;
            Description = description;
            Rate = rate;
            Animals = animals;
            Date = date;
        }

        public int Id { get; set; }
        public User User { get; set; }
        public Executor Executor { get; set; }
        public string Description { get; set; }
        public float Rate { get; set; }
        public Animal[] Animals { get; set; }
        public DateTime Date { get; set; }

        public string InsertQuery => "INSERT INTO `review`(`id`, `user`, `executor`, `description`, `rate`, `date`) " +
            "VALUES (@id, @user, @executor, @description, @rate, @date)";

        public string UpdateQuery => "UPDATE `review` SET " +
            "`user`=@user," +
            "`executor`=@executor," +
            "`description`=@description," +
            "`rate`=@rate," +
            "`date`=@date " +
            "WHERE `order`=@id";

        public bool Delete()
        {
            using Database database = new Database();
            return database.Delete(Id, "review");
        }

        public void Update()
        {
            using Database database = new Database();
            bool check = database.Check(Id, "review");
            Command command = database.Command(check ? UpdateQuery : InsertQuery);
            command.Parameters.Add("@user", SqlType.Int32).Value = User.Id;
            command.Parameters.Add("@executor", SqlType.Int32).Value = Executor.Id;
            command.Parameters.Add("@description", SqlType.String).Value = Description;
            command.Parameters.Add("@rate", SqlType.Float).Value = Rate;
            command.Parameters.Add("@date ", SqlType.DateTime).Value = Date;
            command.Parameters.Add("@id", SqlType.Int32).Value = Id;
            command.ExecuteNonQuery();
            command = database.Command("DELETE FROM `review_animal` WHERE `review`=@id");
            command.Parameters.Add("@id", SqlType.Int32).Value = Id;
            command.ExecuteNonQuery();
            command = database.Command("INSERT INTO `review_animal`(`review`, `animal`) VALUES (@review, @animal)");
            if (Animals != null)
                foreach (Animal animal in Animals)
                {
                    command.Parameters.Add("@review", SqlType.Int32).Value = Id;
                    command.Parameters.Add("@animal", SqlType.Int32).Value = animal.Type;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
        }

        public static Review[] GetReviewsByExecutor(Executor executor, int start = 0, int n = 10)
        {
            List<Review> reviews = new List<Review>();
            List<Animal> animals = new List<Animal>();
            using Database database = new Database();
            Command command = database.Command("SELECT * "
                                               + "FROM `review` "
                                               + "WHERE `executor`=@executor "
                                               + "ORDER BY `date` DESC "
                                               + $"LIMIT {start}, {n}");
            command.Parameters.Add("@executor", SqlType.Int32).Value = executor.Id;
            using (Reader reader = command.ExecuteReader())
                while (reader.Read())
                    reviews.Add(new Review(
                        (int)reader["id"],
                        User.GetUserById((int)reader["user"]),
                        executor,
                        (string)reader["description"],
                        (float)reader["rate"],
                        null,
                        (DateTime)reader["date"]));
            command = database.Command("SELECT `animal` FROM `review_animal` WHERE `review`=@review");
            foreach(Review review in reviews)
            {
                command.Parameters.Add("@review", SqlType.Int32).Value = review.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        animals.Add(new Animal(reader.GetInt32(0)));
                command.Parameters.Clear();
                review.Animals = animals.ToArray();
            }
            return reviews.ToArray();
        }
    }
}