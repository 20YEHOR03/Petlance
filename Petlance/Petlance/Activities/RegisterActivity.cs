using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using System;
using Xamarin.Essentials;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    [Activity(NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class RegisterActivity : PetlanceActivity
    {
        private User tempUser;
        //CodePrompt popUp;
        TextView error;
        TextView button;
        Dialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_register;
            base.OnCreate(savedInstanceState);
            error = FindViewById<TextView>(Resource.Id.registerErrorTextView);
            button = FindViewById<TextView>(Resource.Id.register_button);
            button.Click += RegisterButton_Click;
            dialog = GetDialog(Resource.Layout.code_prompt,PopUp_Click);
        }

        protected async void RegisterButton_Click(object sender, EventArgs e)
        {
            string name = FindViewById<EditText>(Resource.Id.nameEditText).Text;
            string password = FindViewById<EditText>(Resource.Id.passwordEditText).Text.Trim();
            string tel = FindViewById<EditText>(Resource.Id.telEditText).Text;
            string email = FindViewById<EditText>(Resource.Id.emailEditText).Text;
            if (name == "")
                error.Text = "Name cannot be empty";
            else if (!EmailService.CheckEmail(email))
                error.Text = "Invalid email";
            else if (!IAccount.CheckPassword(password))
                error.Text = "Invalid password. It must be at least 8 characters long and contain 1 uppercase letter, 1 lowercase letter and 1 digit";
            else if (password != FindViewById<EditText>(Resource.Id.confirmEditText).Text)
                error.Text = "Passwords are not equal!";
            else
            {
                tempUser = new User(name, SecurePasswordHasher.Hash(password), tel, email, 0, User.GetDefaultAvatar(this), 0);
                bool registered = false;
                using(Database database = new Database())
                {
                    Command command = database.Command($"SELECT `id` FROM `user` WHERE (`tel` = @tel AND `tel` <> '') OR (`email` = @email AND `email` <> '')");
                    command.Parameters.Add("@tel", SqlType.String).Value = tempUser.Phone;
                    command.Parameters.Add("@email", SqlType.String).Value = tempUser.Email;
                    using Reader reader = command.ExecuteReader();
                    registered = reader.HasRows;
                }
                if (registered)
                    error.Text = "Tel or email are already taken!";
                else
                {
                    await EmailService.SendVerification(tempUser, "You registered an account on Petlance, before being able to use your account you need to verify that this is your email address by entering this code:");
                    dialog.Show();
                }
            }
        }

        private void PopUp_Click(object sender, EventArgs e)
        {
            EditText Code = dialog.FindViewById<EditText>(Resource.Id.code);
            if (Code.Text == EmailService.Code)
            {
                dialog.Hide();
                if (tempUser.Register())
                ShowActivity<LoginActivity>();
                else
                    error.Text = "Something went wrong";
            }
            else
            {
                dialog.FindViewById<TextView>(Resource.Id.error).Text = "Incorrect code. Try again";
                Code.Text = "";
            }
        }
    }
}