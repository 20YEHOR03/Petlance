using Android.App;
using Android.Content;
using Android.Graphics;
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
    class Report
    {
        public Report(Offer offer, string reason, string type, Bitmap[] photos)
        {
            Offer = offer;
            Reason = reason;
            Type = type;
            Photos = photos;
        }
        public int Id { get; set; }
        public Offer Offer { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; }
        public Bitmap[] Photos { get; set; }
        public bool Send()
        {
            using Database database = new Database();
            Command command = database.Command("INSERT INTO `report`(`offer`, `reason`, `type`) VALUES (@offer, @reason, @type)");
            command.Parameters.Add("@offer", SqlType.Int32).Value = Offer.Id;
            command.Parameters.Add("@reason", SqlType.String).Value = Reason;
            command.Parameters.Add("@type", SqlType.String).Value = Type;
            if(command.ExecuteNonQuery() > 0)
            {
                foreach (var photo in Photos)
                {
                    command = database.Command("INSERT INTO `report_photo`(`report`, `photo`) VALUES (@report, @photo)");
                    command.Parameters.Add("@report", SqlType.Int32).Value = Id;
                    command.Parameters.Add("@photo", SqlType.String).Value = Images.GetBytesFromBitmap(photo);
                }
                return true;
            }
            return false;
        }
    }
}