using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    class UserActivity : DrawerActivity
    {
        bool isEdit = true;
        List<EditText> Fields = new List<EditText>();
        TextView Error { get; set; }
        ImageView AvatarView { get; set; }
        ImageView AvatarButton { get; set; }
        TextView ChangePasswordButton { get; set; }
        LinearLayout DocumentArea { get; set; }
        TextView OpenDocumentButton { get; set; }
        TextView ReplaceDocumentButton { get; set; }
        ImageView EditButton { get; set; }
        byte[] Avatar { get; set; }
        Dialog CodeDialog { get; set; }
        Dialog ChangePasswordDialog { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_user;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            Error = FindViewById<TextView>(Resource.Id.error_message);
            AvatarView = FindViewById<ImageView>(Resource.Id.avatar);
            AvatarView.SetImageBitmap(Images.GetBitmapFromBytes(Petlance.User.Picture));
            AvatarView.Click += Avatar_Click;
            AvatarButton = FindViewById<ImageView>(Resource.Id.edit_avatar);
            AvatarButton.Click += Avatar_Click;
            FindViewById<LinearLayout>(Resource.Id.rating_area).Visibility = (Petlance.User is Executor) ? ViewStates.Visible : ViewStates.Gone;
            if (Petlance.User is Executor)
                FindViewById<RatingBar>(Resource.Id.rating).Rating = (Petlance.User as Executor).GetRating();

            Fields = new List<EditText>()
            {
                FindViewById<EditText>(Resource.Id.username),
                FindViewById<EditText>(Resource.Id.email),
                FindViewById<EditText>(Resource.Id.phone)
            };
            Fields[0].Text = Petlance.User.Name;
            Fields[1].Text = Petlance.User.Email;
            Fields[2].Text = Petlance.User.Phone;
            Avatar = Petlance.User.Picture;

            foreach (EditText field in Fields)
                ChangeState(field);

            DocumentArea = FindViewById<LinearLayout>(Resource.Id.document_area);
            DocumentArea.Visibility = ((Petlance.User as Executor) != null) ? ViewStates.Visible : ViewStates.Gone;

            ChangePasswordButton = FindViewById<TextView>(Resource.Id.change_password_button);
            ChangePasswordButton.Click += ChangePasswordButton_Click;

            ChangePasswordDialog = GetDialog(Resource.Layout.change_password_prompt, ChangePasswordPromptButton_Click);

            OpenDocumentButton = FindViewById<TextView>(Resource.Id.open_button);
            OpenDocumentButton.Click += OpenDocumentButton_Click;

            ReplaceDocumentButton = FindViewById<TextView>(Resource.Id.replace_button);
            ReplaceDocumentButton.Click += ReplaceDocumentButton_Click;

            EditButton = FindViewById<ImageView>(Resource.Id.edit_button);
            EditButton.Click += EditButton_Click;

            CodeDialog = GetDialog(Resource.Layout.code_prompt, PopUp_Click);
        }

        private void ChangePasswordPromptButton_Click(object sender, System.EventArgs e)
        {
            EditText old_password = ChangePasswordDialog.FindViewById<EditText>(Resource.Id.old_password);
            EditText new_password = ChangePasswordDialog.FindViewById<EditText>(Resource.Id.new_password);
            EditText confirm_password = ChangePasswordDialog.FindViewById<EditText>(Resource.Id.confirm_password);
            TextView error = ChangePasswordDialog.FindViewById<TextView>(Resource.Id.error);
            if (SecurePasswordHasher.Verify(old_password.Text, Petlance.User.Password))
            {
                if (new_password.Text == confirm_password.Text)
                {
                    if (IAccount.CheckPassword(new_password.Text))
                    {
                        Petlance.User.Password = SecurePasswordHasher.Hash(new_password.Text);
                        Preferences.Remove("password");
                        Preferences.Set("password", Petlance.User.Password);
                        Petlance.User.Update();
                        ChangePasswordDialog.Hide();
                        old_password.Text = new_password.Text = confirm_password.Text = "";
                    }
                    else error.Text = "Invalid password. It must be at least 8 characters long and contain 1 uppercase letter, 1 lowercase letter and 1 digit";
                }
                else error.Text = "Passwords are not equal";
            }
            else error.Text = "Invalid old password";
        }

        private void PopUp_Click(object sender, System.EventArgs e)
        {
            EditText Code = CodeDialog.FindViewById<EditText>(Resource.Id.code);
            if (Code.Text == EmailService.Code)
            {
                CodeDialog.Hide();
                ApplyChanges();
            }
            else
            {
                CodeDialog.FindViewById<TextView>(Resource.Id.error).Text = "Incorrect code. Try again";
                Code.Text = "";
            }
        }

        private async void EditButton_Click(object sender, System.EventArgs e)
        {
            if (isEdit = !isEdit)
            {
                if (EmailService.CheckEmail(Fields[1].Text))
                {
                    if (Fields[1].Text.Trim() != Petlance.User.Email)
                    {
                        await EmailService.SendVerification(Petlance.User, "");
                        CodeDialog.Show();
                    }
                    else
                        ApplyChanges();
                }
                else
                    Error.Text = "Invalid email";
            }
            else
                ApplyChanges();
        }

        private void ApplyChanges()
        {
            foreach (EditText field in Fields)
                ChangeState(field);
            Petlance.User.Name = Fields[0].Text;
            Petlance.User.Email = Fields[1].Text;
            Petlance.User.Phone = Fields[2].Text;
            Petlance.User.Picture = Avatar;
            Preferences.Remove("login");
            Preferences.Set("login", Petlance.User.Email);
            Petlance.User.Update();
        }

        private void ChangeState(EditText view)
        {
            if (view.Enabled)
                view.Background = null;
            else
                view.SetBackgroundResource(Resource.Drawable.external_login_button_backgroud);
            view.Enabled = !view.Enabled;
        }

        private async void ReplaceDocumentButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions()
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> { { DevicePlatform.Android, new[] { "application/pdf" } } }),
                    PickerTitle = "Select a document(Image or PDF)"
                });
                if (result != null)
                {
                    using Stream stream = await result.OpenReadAsync();
                    using MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    (Petlance.User as Executor).SendVerification(memoryStream.ToArray());
                }
            }
            catch { }
        }

        private async void OpenDocumentButton_Click(object sender, System.EventArgs e)
        {
            var filePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "document.pdf");
            if (File.Exists(filePath))
                File.Delete(filePath);
            byte[] bytes = (Petlance.User as Executor).Document;
            try { File.WriteAllBytes(filePath, bytes); }
            catch { Error.Text = "Error opening file"; }
            await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
        }

        private void ChangePasswordButton_Click(object sender, System.EventArgs e) => ChangePasswordDialog.Show();

        private async void Avatar_Click(object sender, System.EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(PickOptions.Images);
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    Avatar = memoryStream.ToArray();
                    Petlance.User.SetAvatar(Avatar);
                    AvatarView.SetImageBitmap(Images.GetBitmapFromBytes(Avatar));
                    Toggle.UpdateHeader();
                }
            }
            catch
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage("error loading your picture");
                builder.Create().Show();
            }
        }
    }
}