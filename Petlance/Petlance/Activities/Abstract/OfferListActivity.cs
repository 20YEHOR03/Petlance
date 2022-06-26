using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using static Android.App.ActionBar;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public abstract class OfferListActivity : FilterActivity
    {
        protected int current = 0;
        protected int Count = 10;
        protected TextView MoreButton { get; set; }
        protected LinearLayout ListLayout { get; set; }
        protected FloatingActionButton FloatingActionButton { get; set; }
        public string FROMclause { get; set; }
        public string WHEREclause { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ListLayout = FindViewById<LinearLayout>(Resource.Id.list_layout);
            filtersButton.Click += FiltersButton_Click;
            MoreButton = FindViewById<TextView>(Resource.Id.more_button);
            MoreButton.Click += MoreButton_Click;
            MoreButton_Click(new object(), new EventArgs());
        }
        public void FiltersButton_Click(object sender, EventArgs e)
        {
            ListLayout.RemoveAllViews();
            current = 0;
            MoreButton_Click(sender, e);
        }
        protected string HavingClause()
        {
            string pts = "HAVING pts LIKE '%";
            if (FindViewById<CheckBox>(Resource.Id.filter0).Checked)
                pts += "0%";
            if (FindViewById<CheckBox>(Resource.Id.filter1).Checked)
                pts += "1%";
            if (FindViewById<CheckBox>(Resource.Id.filter2).Checked)
                pts += "2%";
            if (FindViewById<CheckBox>(Resource.Id.filter3).Checked)
                pts += "3%";
            if (FindViewById<CheckBox>(Resource.Id.filter4).Checked)
                pts += "4%";
            if (FindViewById<CheckBox>(Resource.Id.filter5).Checked)
                pts += "5%";
            pts += "' ";
            string minPrice = FindViewById<EditText>(Resource.Id.min_filter).Text;
            string maxPrice = FindViewById<EditText>(Resource.Id.max_filter).Text;

            if (minPrice != "")
                pts += $" AND `price` > {minPrice} ";
            if (maxPrice != "")
                pts += $" AND `price` < {maxPrice} ";
            return pts;
        }
        protected void MoreButton_Click(object sender, EventArgs e)
        {
            bool hasRows = false;
            int prev = current;
            List<Offer> offers = new List<Offer>();
            using Database database = new Database();
            Command command = database.Command(
                "SELECT `offer`.`id`, `offer`.`title`, `offer`.`short_desc`, `offer`.`executor`, `offer`.`is_active`, "
                + "GROUP_CONCAT(`animal`.`type`, '') as pts, "
                + "(SUM(`animal`.`price`) + `offer`.`initial_price`) AS price "
                + $"FROM {FROMclause} "
                + $"WHERE {WHEREclause} "
                + $"GROUP BY `offer`.`id` "
                + $"{HavingClause()} "
                + "ORDER BY `entopped` DESC "
                + $"LIMIT {current}, {Count} ");
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
                    });
            }
            foreach (Offer offer in offers)
            {
                List<Animal> animals = new List<Animal>();
                command = database.Command("SELECT * FROM `animal` WHERE `offer`=@offer");
                command.Parameters.Add("@offer", SqlType.Int32).Value = offer.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        animals.Add(new Animal(reader.GetInt32(reader.GetOrdinal("type")), 0));
                offer.Animals = animals.ToArray();
                current++;
                ListLayout.AddView(new OfferLayout(this, offer));
            }
            if (!hasRows && current == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No offers"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ListLayout.AddView(view);
            }
            MoreButton.Visibility = (current % Count == 0 && prev != current) ? ViewStates.Visible : ViewStates.Gone;
        }

        protected void SetReadOnly()
        {
            for (int i = 0; i < ListLayout.ChildCount; i++)
            {
                View view = ListLayout.GetChildAt(i);
                if (view is OfferLayout)
                {
                    (view as OfferLayout).editButton.Visibility = ViewStates.Gone;
                }
            }
        }
        protected void FabOnClick(object sender, EventArgs eventArgs)
        {
            EditOfferActivity.Prev = this;
            ShowActivity<EditOfferActivity>();
        }
    }
}
