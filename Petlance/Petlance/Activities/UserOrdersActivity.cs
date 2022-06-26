using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Tabs;
using System;
using System.Collections.Generic;
using static Android.App.ActionBar;
using Command = MySqlConnector.MySqlCommand;
using Reader = MySqlConnector.MySqlDataReader;
using SqlType = MySqlConnector.MySqlDbType;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class UserOrdersActivity : DrawerActivity
    {
        protected int SelectedTab;
        protected int[] current = { 0, 0 };
        protected int Count = 10;
        private string[] query =
        {
            " FROM `request`, `offer` " +
                    "WHERE `request`.`offer`=`offer`.`id` AND `offer`.`executor`=@id ",
            " FROM `request`" +
                    "WHERE `request`.`user`=@id "
        };
        protected View[] TabContent { get; set; }
        protected TextView[] MoreButton { get; set; }
        protected LinearLayout[] ListLayout { get; set; }
        protected LinearLayout TabLayout { get; set; }
        protected TextView Incomming { get; set; }
        protected TextView Outgoing { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_list_tabbed;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            TabLayout = FindViewById<LinearLayout>(Resource.Id.tabs);
            FindViewById<TextView>(Resource.Id.outgoing).Click += delegate { SetTab(0); };
            FindViewById<TextView>(Resource.Id.incoming).Click += delegate { SetTab(1); };

            TabContent = new View[] {
                FindViewById<View>(Resource.Id.list1),
                FindViewById<View>(Resource.Id.list2)
            };
            ListLayout = new LinearLayout[]
            {
                TabContent[0].FindViewById<LinearLayout>(Resource.Id.list_layout),
                TabContent[1].FindViewById<LinearLayout>(Resource.Id.list_layout)
            };
            MoreButton = new TextView[]
            {
                TabContent[0].FindViewById<TextView>(Resource.Id.more_button),
                TabContent[1].FindViewById<TextView>(Resource.Id.more_button)
            };
            MoreButton[0].Click += MoreButton_Click;
            MoreButton[1].Click += MoreButton_Click;
            SelectedTab = 1;
            MoreButton_Click(new object(), new EventArgs());
            SelectedTab = 0;
            MoreButton_Click(new object(), new EventArgs());
            SetTab(0);
        }

        private void SetTab(int n)
        {
            SelectedTab = n;
            TabContent[SelectedTab].Visibility = ViewStates.Visible;
            TabContent[1-SelectedTab].Visibility = ViewStates.Gone;
        }
        protected void MoreButton_Click(object sender, EventArgs e)
        {
            bool hasRows = false;
            int prev = current[SelectedTab];
            List<Request> requests = new List<Request>();
            using Database database = new Database();
            Command command = database.Command(
                "SELECT "
                + "`request`.`id`, "
                + "`request`.`user`, "
                + "`request`.`offer`, "
                + "`request`.`time`, "
                + "`request`.`other`, "
                + "`request`.`desc` "
                + query[SelectedTab]
                + $" ORDER BY `request`.`time` DESC LIMIT {current[SelectedTab]}, {Count} ");
            command.Parameters.Add("@id", SqlType.Int32).Value = Petlance.User.Id;
            using (Reader reader = command.ExecuteReader())
            {
                hasRows = reader.HasRows;
                while (reader.Read())
                    requests.Add(new Request(
                        reader.GetInt32(0),
                        User.GetUserById(reader.GetInt32(1)),
                        Offer.GetOfferById(reader.GetInt32(2)),
                        reader.GetDateTime(3),
                        new Dictionary<Animal, int>(),
                        reader.GetString(4),
                        reader.GetString(5)));
            }
            foreach (Request request in requests)
            {
                command = database.Command("SELECT `animal`, `count` FROM `request_animal` WHERE `request`=@request ");
                command.Parameters.Add("@request", SqlType.Int32).Value = request.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        request.Animals.Add(new Animal(reader.GetInt32(0)), reader.GetInt32(1));
                current[SelectedTab]++;
                ListLayout[SelectedTab].AddView(new RequestLayout(this, request, (SelectedTab == 0) ? OrderType.Incomming : OrderType.Outgoing));
            }
            if (!hasRows && current[SelectedTab] == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No offers"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ListLayout[SelectedTab].AddView(view);
            }
            MoreButton[SelectedTab].Visibility = (current[SelectedTab] % Count == 0 && prev != current[SelectedTab]) ? ViewStates.Visible : ViewStates.Gone;
        }
        public void Update()
        {
            ListLayout[SelectedTab].RemoveAllViews();
            int x = (int)Math.Ceiling((double)current[SelectedTab] /Count);
            current[SelectedTab] = 0;
            while(x > 0)
            {
                MoreButton_Click(new object(), new EventArgs());
                x -= Count;
            }
        }
    }
}