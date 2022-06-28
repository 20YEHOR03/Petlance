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
        Dictionary<int, User> users = new Dictionary<int, User>();
        Dictionary<int, Offer> offers = new Dictionary<int, Offer>();
        Dictionary<int, Executor> executors = new Dictionary<int, Executor>();
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
            int prev = current[1];
            Dictionary<Order, KeyValuePair<int, int>> orders = new Dictionary<Order, KeyValuePair<int, int>>();
            using (Database database = new Database())
            {
                Command command = database.Command(
                    "SELECT `order`.`id`, "
                    + "`offer`.`executor`, "
                    + "`order`.`offer`, "
                    + "`order`.`time`, "
                    + "`order`.`price`, "
                    + "`order`.`is_paid`, "
                    + "`order`.`other`, "
                    + "`order`.`desc`, "
                    + "`order`.`is_accepted`, "
                    + "`order`.`days` "
                    + " FROM `order`, `offer` "
                    + " WHERE `user`=@id AND `order`.`offer`=`offer`.`id` "
                    + $" ORDER BY `time` DESC LIMIT {current[1]}, {Count} ");
                command.Parameters.Add("@id", SqlType.Int32).Value = Petlance.User.Id;
                using Reader reader = command.ExecuteReader();
                while (reader.Read())
                    orders.Add(new Order(
                        id: reader.GetInt32(0),
                        user: Petlance.User,//User.GetUserById(reader.GetInt32(1)),
                        offer: null,//Offer.GetOfferById(reader.GetInt32(2), Petlance.User as Executor),
                        time: reader.GetDateTime(3),
                        price: reader.GetInt32(4),
                        isPaid: reader.GetBoolean(5),
                        animals: new Dictionary<Animal, int>(),
                        other: reader.GetString(6),
                        desc: reader.GetString(7),
                        isAccepted: reader.GetBoolean(8),
                        days: reader.GetInt32(9)),
                        new KeyValuePair<int, int>(reader.GetInt32(1), reader.GetInt32(2)));
            }
            foreach (var order in orders)
            {
                using (Database database = new Database())
                {
                    Command command = database.Command("SELECT `animal`, `count` FROM `order_animal` WHERE `order`=@order ");
                    command.Parameters.Add("@order", SqlType.Int32).Value = order.Key.Id;
                    using Reader reader = command.ExecuteReader();
                    while (reader.Read())
                        order.Key.Animals.Add(new Animal(reader.GetInt32(0)), reader.GetInt32(1));
                }
                if (!executors.ContainsKey(order.Value.Key))
                    executors.Add(order.Value.Key, Executor.GetExecutorById(order.Value.Key));
                if (!offers.ContainsKey(order.Value.Value))
                    offers.Add(order.Value.Value, Offer.GetOfferById(order.Value.Value, executors[order.Value.Key]));
                order.Key.Offer = offers[order.Value.Value];
                current[1]++;
                ListLayout[1].AddView((View)new OrderLayout(this, order.Key, OrderType.Outgoing));
            }
            if (orders.Count == 0 && current[1] == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                    Text = "No orders"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ListLayout[1].AddView(view);
            }
            MoreButton[1].Visibility = (current[1] % Count == 0 && prev != current[1]) ? ViewStates.Visible : ViewStates.Gone;
        }
        protected void MoreButton_Click1(object sender, EventArgs e)
        {
            int prev = current[0];
            Dictionary<Order, KeyValuePair<int, int>> orders = new Dictionary<Order, KeyValuePair<int, int>>();
            using (Database database = new Database())
            {
                Command command = database.Command(
                    "SELECT "
                    + "`order`.`id`, "
                    + "`order`.`user`, "
                    + "`order`.`offer`, "
                    + "`order`.`time`, "
                    + "`order`.`other`, "
                    + "`order`.`desc`, "
                    + "`order`.`days` "
                    + " FROM `order`, `offer` "
                    + "WHERE `order`.`offer`=`offer`.`id` AND `offer`.`executor`=@id "
                    + $" ORDER BY `order`.`time` DESC LIMIT {current[0]}, {Count} ");
                command.Parameters.Add("@id", SqlType.Int32).Value = Petlance.User.Id;
                using Reader reader = command.ExecuteReader();
                while (reader.Read())
                    orders.Add(new Order(id: reader.GetInt32(0),
                                         user: null,
                                         offer: null,//Offer.GetOfferById(reader.GetInt32(2)),
                                         time: reader.GetDateTime(3),
                                         price: 0,
                                         isPaid: false,
                                         animals: new Dictionary<Animal, int>(),
                                         other: reader.GetString(4),
                                         desc: reader.GetString(5),
                                         isAccepted: false,
                                         days: reader.GetInt32(6)),
                                         new KeyValuePair<int, int>(reader.GetInt32(1), reader.GetInt32(2)));
            }
            foreach (var order in orders)
            {
                using (Database database = new Database())
                {
                    Command command = database.Command("SELECT `animal`, `count` FROM `order_animal` WHERE `order`=@order ");
                    command.Parameters.Add("@order", SqlType.Int32).Value = order.Key.Id;
                    using Reader reader = command.ExecuteReader();
                    while (reader.Read())
                        order.Key.Animals.Add(new Animal(reader.GetInt32(0)), reader.GetInt32(1));
                }
                if (!users.ContainsKey(order.Value.Key))
                    users.Add(order.Value.Key, User.GetUserById(order.Value.Key));
                order.Key.User = users[order.Value.Key];
                if (!offers.ContainsKey(order.Value.Value))
                    offers.Add(order.Value.Value, Offer.GetOfferById(order.Value.Value, Petlance.User as Executor));
                order.Key.Offer = offers[order.Value.Value];
                current[0]++;
                ListLayout[0].AddView(new OrderLayout(this, order.Key, OrderType.Incomming));
            }
            if (orders.Count==0 && current[0] == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                    Text = "No orders"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ListLayout[0].AddView(view);
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
                MoreButton_Click1(new object(), new EventArgs());
                x -= Count;
            }
        }
    }
}