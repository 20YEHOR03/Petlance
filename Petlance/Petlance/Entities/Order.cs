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
    class Order : Entity
    {
        public Order(int id, User user, Offer offer, int price, bool isPaid)
        {
            Id = id;
            User = user;
            Offer = offer;
            Price = price;
            IsPaid = isPaid;
        }

        public string InsertQuery => "BEGIN; INSERT INTO `order`(`user`, `offer`, `price`, `is_paid`) VALUES (@user, @offer, @price, @is_paid); SELECT LAST_INSERT_ID(); COMMIT;";

        public string UpdateQuery => "UPDATE `order` SET `user`=@user,`offer`=@offer,`price`=@price,`is_paid`=@is_paid WHERE `id`=@id";

        public int Id { get; set; }
        public User User { get; set; }
        public Offer Offer { get; set; }
        public int Price { get; set; }
        public bool IsPaid { get; set; }
        public bool Delete()
        {
            using Database database = new Database();
            return database.Delete(Id, "order");
        }

        public void Update()
        {
            using Database database = new Database();
            bool check = database.Check(Id, "order");
            Command command = database.Command(check ? UpdateQuery : InsertQuery);
            command.Parameters.Add("@user", SqlType.Int32).Value = User.Id;
            command.Parameters.Add("@offer", SqlType.Int32).Value = Offer.Id;
            command.Parameters.Add("@price", SqlType.Int32).Value = Price;
            command.Parameters.Add("@is_paid", SqlType.Bool).Value = IsPaid;
            if (check)
            {
                command.Parameters.Add("@id", SqlType.Int32).Value = Id;
                command.ExecuteNonQuery();
            }
            else using (Reader reader = command.ExecuteReader())
                    if(reader.Read())
                        Id = reader.GetInt32(0);

        }

        public void Send()
        {
            Update();
        }
    }
}