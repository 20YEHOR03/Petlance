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
            WHEREclause = $" `favorites`.`user`={Petlance.User.Id} ";//AND `is_active`=1 ";
            FROMclause = " `offer` LEFT JOIN `animal` ON `offer`.`id`=`animal`.`offer` LEFT JOIN `favorites` ON `offer`.`id`=`favorites`.`offer` ";
            base.OnCreate(savedInstanceState);
            SetReadOnly();
        }
    }
}