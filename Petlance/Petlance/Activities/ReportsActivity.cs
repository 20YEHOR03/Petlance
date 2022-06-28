using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using static Android.Views.ViewGroup;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;
using static Petlance.AdaptiveLayout;

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
            List<Report> reports = new List<Report>();
            using Database database = new Database();
            Command command = database.Command(
                "SELECT `report`.`id`, `report`.`offer`, `report`.`reason`, "
                + "GROUP_CONCAT(`animal`.`type`, '') as pts, "
                + "(SUM(`animal`.`price`) + `offer`.`initial_price`) AS price "
                + $"FROM `offer`, `animal`,`report` "
                + $"WHERE `offer`.`id`=`animal`.`offer` AND `offer`.`id`=`report`.`offer` "
                + $"GROUP BY `report`.`id`, `offer`.`id`, `report`.`reason` "
                + "ORDER BY `entopped` DESC "
                + $"LIMIT {current}, {ReportCount} ");
            using (Reader reader = command.ExecuteReader())
            {
                hasRows = reader.HasRows;
                while (reader.Read())
                    reports.Add(new Report(reader.GetInt32(0), Offer.GetOfferById(reader.GetInt32(1)), reader.GetString(2), null));
            }
            foreach (var report in reports)
            {
                List<byte[]> photos = new List<byte[]>();
                command = database.Command("SELECT `photo` FROM `report_photo` WHERE `report`=@id");
                command.Parameters.Add("@id", SqlType.Int32).Value = report.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        photos.Add((byte[])reader[0]);
                report.Photos = photos.ToArray();
                List<Animal> animals = new List<Animal>();
                command = database.Command("SELECT `type` FROM `animal` WHERE `offer`=@offer");
                command.Parameters.Add("@offer", SqlType.Int32).Value = report.Offer.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        animals.Add(new Animal(reader.GetInt32(0), 0));
                report.Offer.Animals = animals.ToArray();
                current++;
                OfferLayout layout = new OfferLayout(this, report.Offer);
                OfferLayout.AddView(layout);
                ReasonButton reason = new ReasonButton(this, report) { 
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent), 
                    Text = "Reason"
                };
                reason.SetPadding(1 * vh, 0, 0, 0);
                reason.LayoutParameters = new MarginLayoutParams(reason.LayoutParameters);
                (reason.LayoutParameters as MarginLayoutParams).SetMargins(0, -2 * vmin, 0, 2 * vh);
                reason.SetBackgroundResource(Resource.Drawable.enter_button);
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