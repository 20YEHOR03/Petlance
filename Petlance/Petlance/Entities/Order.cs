using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    public class Order
    {

        public Order(int id,
                     User user,
                     Offer offer,
                     DateTime time,
                     int price,
                     bool isPaid,
                     Dictionary<Animal, int> animals,
                     string other,
                     string desc,
                     bool isAccepted)
        {
            Id = id;
            User = user;
            Offer = offer;
            Time = time;
            Price = price;
            IsPaid = isPaid;
            Animals = animals;
            Other = other;
            Desc = desc;
            IsAccepted = isAccepted;
        }

        public string InsertQuery => "BEGIN; " +
            "INSERT INTO `order`(`user`, `offer`, `time`, `other`, `desc`, `price`, `is_paid`, `is_accepted`) " +
            "VALUES             (@user,  @offer,  @time,  @other,  @desc,  @price,  @is_paid,  @is_accepted); " +
            "SELECT LAST_INSERT_ID(); " +
            "COMMIT;";

        public string UpdateQuery => "UPDATE `order` SET "
            + "`user` = @user, "
            + "`offer` = @offer, "
            + "`time` = @time, "
            + "`other` = @other, "
            + "`desc` = @desc, "
            + "`price` = @price, "
            + "`is_paid` = @is_paid, "
            + "`is_accepted` = @is_accepted "
            + "WHERE `id`=@id";

        public int Id { get; set; }
        public User User { get; set; }
        public Offer Offer { get; set; }
        public DateTime Time { get; set; }
        public int Price { get; set; }
        public bool IsPaid { get; set; }
        public bool IsAccepted { get; set; }
        public Dictionary<Animal, int> Animals { get; set; }
        public string Other { get; set; }
        public string Desc { get; set; }
        public bool Delete()
        {
            using Database database = new Database();
            return database.Delete(Id, "order");
        }

        public void Update()
        {
            //(@user, @offer, @time, @other, @desc, @price, @is_paid, @is_accepted)
            using Database database = new Database();
            bool check = database.Check(Id, "order");
            Command command = database.Command(check ? UpdateQuery : InsertQuery);
            command.Parameters.Add("@user", SqlType.Int32).Value = User.Id;
            command.Parameters.Add("@offer", SqlType.Int32).Value = Offer.Id;
            command.Parameters.Add("@time", SqlType.DateTime).Value = Time;
            command.Parameters.Add("@other", SqlType.String).Value = Other;
            command.Parameters.Add("@desc", SqlType.String).Value = Desc;
            command.Parameters.Add("@price", SqlType.Int32).Value = Price;
            command.Parameters.Add("@is_paid", SqlType.Bool).Value = IsPaid;
            command.Parameters.Add("@is_accepted", SqlType.Bool).Value = IsAccepted;
            if (check)
            {
                command.Parameters.Add("@id", SqlType.Int32).Value = Id;
                command.ExecuteNonQuery();
            }
            else using (Reader reader = command.ExecuteReader())
                    if(reader.Read())
                        Id = reader.GetInt32(0);
            command = database.Command("DELETE FROM `order_animal` WHERE `order`=@order");
            command.Parameters.Add("@order", SqlType.Int32).Value = Id;
            command.ExecuteNonQuery();
            command = database.Command("INSERT INTO `order_animal`(`order`,`animal`,`count`) VALUES (@order, @animal, @count)");
            foreach (var animal in Animals)
            {
                command.Parameters.Add("@order", SqlType.Int32).Value = Id;
                command.Parameters.Add("@animal", SqlType.Int32).Value = animal.Key.Type;
                command.Parameters.Add("@count", SqlType.Int32).Value = animal.Value;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }

        }

        public void Send() => Update();

        public void Apply()
        {
        }
    }
}