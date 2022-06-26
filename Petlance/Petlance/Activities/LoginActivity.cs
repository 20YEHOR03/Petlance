using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using System;
using Xamarin.Essentials;

namespace Petlance
{
    [Activity(NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true, Theme = "@style/AppTheme")]
    public class LoginActivity : PetlanceActivity
    {
        TextView button;
        LinkTextView unauthorizedLink;
        LinkTextView registerLink;
        LinkTextView forgotLink;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_login;
            base.OnCreate(savedInstanceState);
            if (Preferences.ContainsKey("login") && Preferences.ContainsKey("password"))
            {
                string login = Preferences.Get("login", "");
                string password = Preferences.Get("password", "");
                Petlance.Account = IAccount.Log_in(login, password);
                if (Petlance.Account != null)
                {
                    ShowActivity<MainActivity>();
                    return;
                }
                else Petlance.LogOut();
            }
            button = FindViewById<TextView>(Resource.Id.enterButton);
            button.Click += loginButton_Click;
            unauthorizedLink = FindViewById<LinkTextView>(Resource.Id.unauthorizedLink);
            unauthorizedLink.Click += unauthorizedLink_Click;
            registerLink = FindViewById<LinkTextView>(Resource.Id.registerLink);
            registerLink.Click += registerLink_Click;
            forgotLink = FindViewById<LinkTextView>(Resource.Id.forgotLink);
            forgotLink.Click += forgotLink_Click;
        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            string login = FindViewById<TextView>(Resource.Id.login_prompt).Text;
            string password = FindViewById<TextView>(Resource.Id.password_prompt).Text;
            Petlance.Account = IAccount.Log_in(login, password);
            if (Petlance.Account != null)
            {
                Preferences.Set("login", login);
                Preferences.Set("password", password);
                ShowActivity<MainActivity>();
            }
            else
                FindViewById<TextView>(Resource.Id.errorTextView).Text = "Invalid login or/and password";
        }

        protected void unauthorizedLink_Click(object sender, EventArgs e)
        {
            Petlance.Account = null;
            ShowActivity<MainActivity>();
            Finish();
        }

        protected void registerLink_Click(object sender, EventArgs e)
        {
            ShowActivity<RegisterActivity>();
        }
        protected void forgotLink_Click(object sender, EventArgs e)
        {

        }

    }
}