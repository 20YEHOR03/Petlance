using Android.Graphics;
using System;
using System.Net;
using MailKit.Net.Smtp;
using Xamarin.Essentials;
using System.IO;
using System.Text.RegularExpressions;

namespace Petlance
{
    enum OrderType
    {
        Incomming,
        Outgoing
    }
    static class Petlance
    {
        public static IAccount Account { get; set; }
        public static User User
        {
            get
            {
                if (Account != null && Account is User)
                    return (User)Account;
                return null;
            }
        }

        public static Admin Admin
        {
            get
            {
                if (Account != null && Account is Admin)
                    return (Admin)Account;
                return null;
            }
        }
        public static void LogOut()
        {
            Account = null;
            Preferences.Remove("login");
            Preferences.Remove("password");
        }
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            try
            {
                using var webClient = new WebClient();
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            }
            catch
            {
                throw new Exception();
            }

            return imageBitmap;
        }



    }

}