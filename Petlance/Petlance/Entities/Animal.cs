
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    internal class Animal
    {

        public Animal(int type, int price = 0)
        {
            Type = type;
            Price = price;
        }
        public int Type { get; set; }
        public int Price { get; set; }

        public int GetResourceType()
        {
            switch (Type)
            {
                case 0:
                    return Resource.Drawable.amphibian;
                case 1:
                    return Resource.Drawable.bird;
                case 2:
                    return Resource.Drawable.cat;
                case 3:
                    return Resource.Drawable.dog;
                case 4:
                    return Resource.Drawable.fish;
                case 5:
                    return Resource.Drawable.rodent;
                default:
                    return Resource.Drawable.FAQ;
            }
        }
        public bool CheckAnimal(Offer offer)
        {
            using Database database = new Database();
            Command command = database.Command($"SELECT * FROM `animal` WHERE `offer`=@offer AND `type`=@type");
            command.Parameters.Add("@offer", SqlType.Int32).Value = offer.Id;
            command.Parameters.Add("@type", SqlType.Int32).Value = Type;
            return command.ExecuteNonQuery() > 0;
        }
    }
}