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
    public class UserOrdersActivity : DrawerActivity
    {
        protected int[] current = { 0, 0};
        protected int Count = 10;
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
            FindViewById<TextView>(Resource.Id.incoming).Click += delegate { SetTab(0); };
            FindViewById<TextView>(Resource.Id.outgoing).Click += delegate { SetTab(1); };
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
            MoreButton[0].Click += MoreButton_Click1;
            MoreButton[1].Click += MoreButton_Click2;
            MoreButton_Click1(new object(), new EventArgs());
            MoreButton_Click2(new object(), new EventArgs());
            SetTab((Petlance.User is Executor) ? 0 : 1);
            TabLayout.Visibility = (Petlance.User is Executor) ? ViewStates.Visible : ViewStates.Gone;
        }

        private void SetTab(int n)
        {
            TabContent[n].Visibility = ViewStates.Visible;
            TabContent[1 - n].Visibility = ViewStates.Gone;
        }
        
        protected void MoreButton_Click2(object sender, EventArgs e)
        {
            bool hasRows = false;
            int prev = current[1];
            List<Order> orders = new List<Order>();
            using Database database = new Database();
            Command command = database.Command(
                "SELECT `id`, `user`, `offer`, `time`, `price`, `is_paid`, `other`, `desc`, `is_accepted` "
                + " FROM `order` "
                + " WHERE `user`=@id "
                + $" ORDER BY `time` DESC LIMIT {current[1]}, {Count} ");
            command.Parameters.Add("@id", SqlType.Int32).Value = Petlance.User.Id;
            using (Reader reader = command.ExecuteReader())
            {
                hasRows = reader.HasRows;
                while (reader.Read())
                    orders.Add(new Order(
                        id: reader.GetInt32(0),
                        user: User.GetUserById(reader.GetInt32(1)),
                        offer: Offer.GetOfferById(reader.GetInt32(2)),
                        time: reader.GetDateTime(3),
                        price: reader.GetInt32(4),
                        isPaid: reader.GetBoolean(5),
                        animals: new Dictionary<Animal, int>(),
                        other: reader.GetString(6),
                        desc: reader.GetString(7),
                        isAccepted: reader.GetBoolean(8)));
            }
            foreach (Order order in orders)
            {
                command = database.Command("SELECT `animal`, `count` FROM `order_animal` WHERE `order`=@order ");
                command.Parameters.Add("@order", SqlType.Int32).Value = order.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        order.Animals.Add(new Animal(reader.GetInt32(0)), reader.GetInt32(1));
                current[1]++;
                ListLayout[1].AddView((View)new OrderLayout(this, order, OrderType.Outgoing));
            }
            if (!hasRows && current[1] == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No orders"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ListLayout[1].AddView(view);
            }
            MoreButton[1].Visibility = (current[1] % Count == 0 && prev != current[1]) ? ViewStates.Visible : ViewStates.Gone;
        }
        protected void MoreButton_Click1(object sender, EventArgs e)
        {
            bool hasRows = false;
            int prev = current[0];
            List<Order> orders = new List<Order>();
            using Database database = new Database();
            Command command = database.Command(
                "SELECT "
                + "`order`.`id`, "
                + "`order`.`user`, "
                + "`order`.`offer`, "
                + "`order`.`time`, "
                + "`order`.`other`, "
                + "`order`.`desc` "
                + " FROM `order`, `offer` "
                + "WHERE `order`.`offer`=`offer`.`id` AND `offer`.`executor`=@id AND `order`.`is_accepted`=0"
                + $" ORDER BY `order`.`time` DESC LIMIT {current[0]}, {Count} ");
            command.Parameters.Add("@id", SqlType.Int32).Value = Petlance.User.Id;
            using (Reader reader = command.ExecuteReader())
            {
                hasRows = reader.HasRows;
                while (reader.Read())
                    orders.Add(new Order(
                        reader.GetInt32(0),
                        User.GetUserById(reader.GetInt32(1)),
                        Offer.GetOfferById(reader.GetInt32(2)),
                        reader.GetDateTime(3),
                        0,
                        false,
                        new Dictionary<Animal, int>(),
                        reader.GetString(4),
                        reader.GetString(5),
                        false));
            }
            foreach (Order order in orders)
            {
                command = database.Command("SELECT `animal`, `count` FROM `order_animal` WHERE `order`=@order ");
                command.Parameters.Add("@order", SqlType.Int32).Value = order.Id;
                using (Reader reader = command.ExecuteReader())
                    while (reader.Read())
                        order.Animals.Add(new Animal(reader.GetInt32(0)), reader.GetInt32(1));
                current[0]++;
                ListLayout[0].AddView((View)new OrderLayout(this, order, OrderType.Incomming));
            }
            if (!hasRows && current[0] == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No orders"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ListLayout[1].AddView(view);
            }
            MoreButton[0].Visibility = (current[0] % Count == 0 && prev != current[0]) ? ViewStates.Visible : ViewStates.Gone;
        }
        public void UpdateIncomming()
        {
            ListLayout[1].RemoveAllViews();
            int x = (int)Math.Ceiling((double)current[1] / Count);
            current[1] = 0;
            while (x > 0)
            {
                MoreButton_Click2(new object(), new EventArgs());
                x -= Count;
            }
        }
    }
}