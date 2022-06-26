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

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class ReportsActivity : DrawerActivity
    {
        protected int current = 0;
        protected int ReportCount = 10;
        protected TextView moreButton;
        protected LinearLayout OfferLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_list_offer;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            OfferLayout = FindViewById<LinearLayout>(Resource.Id.list_layout);
            moreButton = FindViewById<TextView>(Resource.Id.more_button);
            moreButton.Click += MoreButton_Click;
            MoreButton_Click(new object(), new EventArgs());
        }
        protected void MoreButton_Click(object sender, EventArgs e)
        {
            bool hasRows = false;
            int prev = current;
            Dictionary<Offer, string> offers = new Dictionary<Offer, string>();
            using Database database = new Database();
            Command command = database.Command(
                "SELECT `offer`.`id`, `offer`.`title`, `offer`.`short_desc`, `offer`.`executor`, `offer`.`is_active`, `report`.`reason`, "
                + "GROUP_CONCAT(`animal`.`type`, '') as pts, "
                + "(SUM(`animal`.`price`) + `offer`.`initial_price`) AS price "
                + $"FROM `offer`, `animal`,`report` "
                + $"WHERE `offer`.`id`=`animal`.`offer` AND `offer`.`id`=`report`.`offer` "
                + $"GROUP BY `offer`.`id` "
                + "ORDER BY `entopped` DESC "
                + $"LIMIT {current}, {ReportCount} ");
            using (Reader reader = command.ExecuteReader())
            {
                hasRows = reader.HasRows;
                while (reader.Read())
                    offers.Add(new Offer()
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        ShortDescription = reader.GetString(2),
                        Executor = Executor.GetExecutorById(reader.GetInt32(3)),
                        IsActive = reader.GetBoolean(4)
                    }, reader.GetString(5));
            }
            foreach (var offer in offers)
            {
                List<Animal> animals = new List<Animal>();
                command = database.Command("SELECT `type` FROM `animal` WHERE `offer`=@offer");
                command.Parameters.Add("@offer", SqlType.Int32).Value = offer.Key.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        animals.Add(new Animal(reader.GetInt32(0), 0));
                offer.Key.Animals = animals.ToArray();
                current++;
                OfferLayout layout = new OfferLayout(this, offer.Key);
                OfferLayout.AddView(layout);
                TextView reason = new TextView(this) { 
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent), 
                    Text = $"Reason: {offer.Value}",
                };
                reason.SetBackgroundResource(Resource.Drawable.external_login_button_backgroud);
                OfferLayout.AddView(reason);
                
            }
            if (!hasRows && current == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No reports"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                OfferLayout.AddView(view);
            }
            moreButton.Visibility = (current % ReportCount == 0 && prev != current) ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}