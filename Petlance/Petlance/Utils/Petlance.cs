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
        public static User User => (Account != null && Account is User) ? (User)Account : null;

        public static Admin Admin => (Account != null && Account is Admin) ? (Admin)Account : null;
        public static void LogOut()
        {
            Account = null;
            Preferences.Remove("login");
            Preferences.Remove("password");
        }
    }

}