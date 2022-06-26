using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class VerifyActivity : DrawerActivity
    {
        TextView Upload;
        TextView Send;
        byte[] Document;
        TextView Filename;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_promote;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            Upload = FindViewById<TextView>(Resource.Id.upload);
            Send = FindViewById<TextView>(Resource.Id.send);
            Upload.Click += Upload_Click;
            Send.Click += Send_Click;
            Filename = FindViewById<TextView>(Resource.Id.filename);
        }

        private void Send_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetPositiveButton("Ok", (sender, e) => { });
            if (Petlance.User.SendVerification(Document))
                alert.SetMessage("Request sent");
            else
                alert.SetMessage("Something went wrong");
            alert.Create().Show();
        }

        private async void Upload_Click(object sender, EventArgs e)
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
                    Document = memoryStream.ToArray();
                    Filename.Text = result.FileName;
                }
            }
            catch { }
        }
    }
}