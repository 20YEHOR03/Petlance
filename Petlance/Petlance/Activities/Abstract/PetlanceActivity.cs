using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using System;
using SupportToolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public abstract class PetlanceActivity : AppCompatActivity
    {
        protected int Activity_layout { get; set; }
        public static PetlanceActivity Prev { get; set; }
        protected SupportToolbar Toolbar { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Activity_layout);
            Toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
                SetSupportActionBar(Toolbar);
        }
        public void ShowActivity<T>() => StartActivity(new Intent(this, typeof(T)));

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected Dialog GetDialog(int layoutResId, System.EventHandler<DialogClickEventArgs> PopUp_Click)
        {
            Android.App.AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetView(layoutResId);
            ad.SetPositiveButton("Confirm", PopUp_Click);
            return ad.Create();
        }
        protected Dialog GetDialog(System.EventHandler<DialogClickEventArgs> PopUp_Click)
        {
            Android.App.AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetPositiveButton("Confirm", PopUp_Click);
            return ad.Create();
        }
        protected Dialog GetDialog(string title, string message, string button, System.EventHandler<DialogClickEventArgs> PopUp_Click)
        {
            Android.App.AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetTitle(title);
            ad.SetMessage(message);
            ad.SetPositiveButton(button, PopUp_Click);
            return ad.Create();
        }
        protected Dialog GetDialog(string title, string message, string button)
        {
            Android.App.AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetTitle(title);
            ad.SetMessage(message);
            ad.SetPositiveButton(button, (sender, e) => { });
            return ad.Create();
        }

        protected void NotImplemented(object sender, EventArgs e)
        {
            AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            builder.SetTitle("Error");
            builder.SetMessage("Work in progress");
            builder.Create().Show();
        }
    }
}