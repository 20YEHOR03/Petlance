using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class MainActivity : OfferListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_list_offer;
            WHEREclause = "`offer`.`id`=`animal`.`offer` AND `is_active`=1 ";
            FROMclause = "`offer`, `animal`,`favorites` ";
            base.OnCreate(savedInstanceState);
            SetReadOnly();
            FloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.fab_add);
            FloatingActionButton.Click += FabOnClick;
            if (Petlance.User != null && Petlance.User is Executor)
                FloatingActionButton.Visibility = ViewStates.Visible;
        }
    }
}

