using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content.Resources;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Net;
using Xamarin.Essentials;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    class EntityLayout : CoordinatorLayout
    {
        protected ImageView deleteButton;
        protected Dialog deleteDialog;
        protected int ButtonSize = 7 * vmin;
        public EntityLayout(Context context) : base(context)
        {
            deleteButton = new ImageView(context)
            {
                LayoutParameters = new LayoutParams(ButtonSize, ButtonSize),
                Visibility = Petlance.Admin != null ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone,
                TranslationZ = 1
            };
            deleteButton.SetImageResource(Resource.Drawable.ic_mtrl_chip_close_circle);
            deleteButton.SetBackgroundResource(Resource.Drawable.external_login_button_backgroud);
            deleteButton.Click += DisableButton_Click;
            AddView(deleteButton);
            AlertDialog.Builder alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Confirm disable");
            alert.SetMessage("Are you sure you want to disable this?");
            alert.SetPositiveButton("Yes", YesButton_Click);
            alert.SetNegativeButton("No", NoButton_Click);
            deleteDialog = alert.Create();
        }

        protected void DisableButton_Click(object sender, EventArgs e) => deleteDialog.Show();
        protected virtual void YesButton_Click(object sender, EventArgs e)
        {
            Toast.MakeText(Context, "Deleted!", ToastLength.Short).Show();
        }
        protected virtual void NoButton_Click(object sender, EventArgs e)
        {
            Toast.MakeText(Context, "Cancelled!", ToastLength.Short).Show();
        }
    }
}