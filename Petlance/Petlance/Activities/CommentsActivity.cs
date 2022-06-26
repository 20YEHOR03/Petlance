using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.App.ActionBar;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    class CommentsActivity : ReviewListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_comments;
            menu_layout = Resource.Menu.no_menu;
            Offer = new Offer() { Executor = Petlance.User as Executor };
            base.OnCreate(savedInstanceState);
        }
    }
}