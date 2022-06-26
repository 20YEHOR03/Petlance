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
            WHEREclause = "`offer`.`id`=`animal`.`offer` AND `is_active`=1 ";
            FROMclause = "`offer`, `animal`,`favorites` ";
            base.OnCreate(savedInstanceState);
            SetReadOnly();
        }
    }
}

