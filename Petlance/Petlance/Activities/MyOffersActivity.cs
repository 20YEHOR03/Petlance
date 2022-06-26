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
    public class MyOffersActivity : OfferListActivity
    {
        protected FloatingActionButton addButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            WHEREclause = $"`offer`.`id`=`animal`.`offer` AND `is_active`=1 AND `offer`.`executor`={Petlance.User.Id} ";
            FROMclause = "`offer`, `animal` ";
            base.OnCreate(savedInstanceState);
        }
    }
}

