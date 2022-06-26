
using System;
using System.Collections.Generic;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;
namespace Petlance
{
    class NoRequest
    {
        public NoRequest(int id, User user, Offer offer, DateTime time, Dictionary<Animal, int> animals, string other, string desc)
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
    }
}