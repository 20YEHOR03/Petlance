using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using static Android.App.ActionBar;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;
using static Petlance.AdaptiveLayout;
namespace Petlance
{
    [Activity(Label = "Favorites", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class DocumentsActivity : DrawerActivity
    {
        protected int current = 0;
        protected int Count = 10;
        protected TextView MoreButton { get; set; }
        protected LinearLayout ListLayout { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_docs;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            ListLayout = FindViewById<LinearLayout>(Resource.Id.list_layout);
            MoreButton = FindViewById<TextView>(Resource.Id.more_button);
            MoreButton.Click += MoreButton_Click;
            MoreButton_Click(new object(), new EventArgs());
        }
        protected void MoreButton_Click(object sender, EventArgs e)
        {
            bool hasRows = false;
            int prev = current;
            Dictionary<User, byte[]> keyValuePairs = new Dictionary<User, byte[]>();
            using Database database = new Database();
            Command command = database.Command(
                "SELECT  `user`.`id`, `user`.`name`, `user`.`email`, `user`.`picture`, `verification`.`document`, `verification`.`id` "
                + $"FROM `user`, `verification`"
                + $"WHERE `user`.`id` = `verification`.`user` "
                + $"LIMIT {current}, {Count} ");
            using (Reader reader = command.ExecuteReader())
            {
                hasRows = reader.HasRows;
                while (reader.Read())
                    ListLayout.AddView(new DocumentLayout(this, new User()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Picture = (byte[])reader[3]
                    }, (byte[])reader[4], reader.GetInt32(5)));
            }
            if (!hasRows && current == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No documnets to verify"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ListLayout.AddView(view);
            }
            MoreButton.Visibility = (current % Count == 0 && prev != current) ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}