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

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public abstract class FilterActivity : DrawerActivity
    {
        protected LinearLayout filters;
        protected TextView filtersButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            menu_layout = Resource.Menu.filters;
            base.OnCreate(savedInstanceState);
            filters = FindViewById<LinearLayout>(Resource.Id.filters);
            filtersButton = FindViewById<TextView>(Resource.Id.apply_filters);
            menu = filters;
        }
    }
}