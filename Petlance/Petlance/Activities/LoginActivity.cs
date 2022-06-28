using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    [Activity(NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true, Theme = "@style/AppTheme")]
    public class LoginActivity : PetlanceActivity
    {
        TextView button;
        LinkTextView unauthorizedLink;
        LinkTextView registerLink;
        LinkTextView forgotLink;
        private Dialog emailDialog;
        private Dialog codeDialog;

        LinearLayout GoogleAuthButton { get; set; }
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
            GoogleAuthButton = FindViewById<LinearLayout>(Resource.Id.google_auth);
            GoogleAuthButton.Click += NotImplemented;
            button = FindViewById<TextView>(Resource.Id.enterButton);
            button.Click += LoginButton_Click;
            unauthorizedLink = FindViewById<LinkTextView>(Resource.Id.unauthorizedLink);
            unauthorizedLink.Click += UnauthorizedLink_Click;
            registerLink = FindViewById<LinkTextView>(Resource.Id.registerLink);
            registerLink.Click += RegisterLink_Click;
            forgotLink = FindViewById<LinkTextView>(Resource.Id.forgotLink);
            forgotLink.Click += ForgotLink_Click;
        }


        protected void LoginButton_Click(object sender, EventArgs e)
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

        protected void UnauthorizedLink_Click(object sender, EventArgs e)
        {
            Petlance.Account = null;
            ShowActivity<MainActivity>();
            Finish();
        }

        protected void RegisterLink_Click(object sender, EventArgs e) => ShowActivity<RegisterActivity>();
        protected void ForgotLink_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            emailDialog.Show();
        }

        protected void ConfirmEmail_Click(object sender, EventArgs e)
        {
            string email = emailDialog.FindViewById<TextView>(Resource.Id.email).Text;
            TextView error = emailDialog.FindViewById<TextView>(Resource.Id.error);
            bool found = false;
            if (EmailService.CheckEmail(email))
            {
                using Database database = new Database();
                Command command = database.Command("SELECT `id` FROM `user` WHERE `email`=@email");
                command.Parameters.Add("@email", SqlType.Int32).Value = email;
                found = command.ExecuteNonQuery() > 0;
            }
            else
                error.Text = "Incorrect email";
            if (found)
                codeDialog = GetDialog(Resource.Layout.code_prompt, ConfirmCode_ClickAsync);
            else
                error.Text = "There is no account with this email";
            codeDialog.Show();
        }
        protected async void ConfirmCode_ClickAsync(object sender, EventArgs e)
        {

            EditText Code = emailDialog.FindViewById<EditText>(Resource.Id.code);
            if (Code.Text == EmailService.Code)
            {
                await EmailService.SendVerification(Petlance.User, "");
                emailDialog = GetDialog(Resource.Layout.new_password_prompt, Password_Click);
            }
            else
            {
                emailDialog.FindViewById<TextView>(Resource.Id.error).Text = "Incorrect code. Try again";
                Code.Text = "";
            }
            emailDialog.Show();
        }
        protected void Password_Click(object sender, EventArgs e)
        {
            TextView error = emailDialog.FindViewById<TextView>(Resource.Id.error);
            EditText new_password = emailDialog.FindViewById<EditText>(Resource.Id.new_password);
            EditText confirm_password = emailDialog.FindViewById<EditText>(Resource.Id.confirm_password);
            if (new_password.Text == confirm_password.Text)
            {
                if (IAccount.CheckPassword(new_password.Text))
                {
                    Petlance.User.Password = SecurePasswordHasher.Hash(new_password.Text);
                    Preferences.Remove("password");
                    Preferences.Set("password", Petlance.User.Password);
                    Petlance.User.Update();
                    emailDialog.Hide();
                    new_password.Text = confirm_password.Text = "";
                    return;
                }
                else error.Text = "Invalid password. It must be at least 8 characters long and contain 1 uppercase letter, 1 lowercase letter and 1 digit";
            }
            else error.Text = "Passwords are not equal";
            emailDialog.Show();
        }

    }
}