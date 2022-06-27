using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    public class Report
    {
        public Report(int id, Offer offer, string reason, byte[][] photos)
        {
            Id = id;
            Offer = offer;
            Reason = reason;
            Photos = photos;
        }
        public int Id { get; set; }
        public Offer Offer { get; set; }
        public string Reason { get; set; }
        public byte[][] Photos { get; set; }
        public void Send()
        {
            using Database database = new Database();
            Command command = database.Command("BEGIN; INSERT INTO `report`(`offer`, `reason`) VALUES (@offer, @reason); SELECT LAST_INSERT_ID(); COMMIT;");
            command.Parameters.Add("@offer", SqlType.Int32).Value = Offer.Id;
            command.Parameters.Add("@reason", SqlType.String).Value = Reason;
            using (Reader reader = command.ExecuteReader())
                if (reader.Read())
                    Id = reader.GetInt32(0);
            command = database.Command("INSERT INTO `report_photo`(`report`, `photo`) VALUES (@report, @photo)");
            foreach (var photo in Photos)
            {
                command.Parameters.Add("@report", SqlType.Int32).Value = Id;
                command.Parameters.Add("@photo", SqlType.String).Value = photo;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
        }
    }
}