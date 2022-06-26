using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    [Activity(Label = "Favorites", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class FavoritesActivity : OfferListActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_list_favorites;
            WHEREclause = $"`offer`.`id`=`animal`.`offer` AND `offer`.`id`=`favorites`.`offer` AND `favorites`.`user`={Petlance.User.Id} ";//AND `is_active`=1 ";
            FROMclause = "`offer`, `animal`, `favorites` ";
            base.OnCreate(savedInstanceState);
            SetReadOnly();
        }
    }
}