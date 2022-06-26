using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    class OrderActivity : DrawerActivity
    {
        int totalPrice;
        private CheckBox checkBox;
        private Discount discount;
        public static UserOrdersActivity Parent { get; set; }
        protected Dialog Review { get; set; }
        public static OrderType Type { get; set; }
        public static Order Order { get; set; }
        protected TextView RightButton { get; set; }
        protected TextView LeftBUtton { get; set; }
        protected Dialog AcceptDialog { get; set; }
        List<LinearLayout> Animals { get; set; }
        protected int TotalCalculatedPrice { get; set; }
        public AlertDialog DeclineDialog { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_order;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            OfferActivity.Initialize(this, Order.Offer);
            RightButton = FindViewById<TextView>(Resource.Id.accept_button);
            LeftBUtton = FindViewById<TextView>(Resource.Id.decline_button);
            TotalCalculatedPrice = Order.Offer.InitialPrice;
            Dictionary<int, Animal> animals = Order.Offer.GetAnimalDictionary();
            foreach (var animal in Order.Animals)
                TotalCalculatedPrice += animal.Value * animals[animal.Key.Type].Price;
            if (Type == OrderType.Outgoing)
            {
                LeftBUtton.Text = "Add review";
                LeftBUtton.Click += AddReviewButton_Click;
                if (Order.IsPaid)
                    RightButton.Visibility = ViewStates.Gone;
                else
                {
                    RightButton.Text = "Pay";
                    RightButton.Click += PayButton_Click;
                }
                FindViewById<TextView>(Resource.Id.total_calculated_price).Text = $"Total price: {Order.Price}";
            }
            else
            {
                RightButton.Click += AcceptButton_Click;
                LeftBUtton.Click += DeclineButton_Click;
                FindViewById<TextView>(Resource.Id.total_calculated_price).Text += TotalCalculatedPrice;
            }
            Animals = new List<LinearLayout>
            {
                FindViewById<LinearLayout>(Resource.Id.pet0),
                FindViewById<LinearLayout>(Resource.Id.pet1),
                FindViewById<LinearLayout>(Resource.Id.pet2),
                FindViewById<LinearLayout>(Resource.Id.pet3),
                FindViewById<LinearLayout>(Resource.Id.pet4),
                FindViewById<LinearLayout>(Resource.Id.pet5)
            };
            foreach (var item in Animals)
                item.Visibility = ViewStates.Gone;
            foreach(var animal in Order.Animals)
            {
                Animals[animal.Key.Type].Visibility = ViewStates.Visible;
                Animals[animal.Key.Type].FindViewById<CheckBox>(Resource.Id.checkbox).Visibility = ViewStates.Gone;
                Animals[animal.Key.Type].FindViewById<EditText>(Resource.Id.editText).Text = animal.Value.ToString();
            }
            FindViewById<EditText>(Resource.Id.other).Text = Order.Other;
            FindViewById<EditText>(Resource.Id.addins).Text = Order.Desc;


            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Review");
            alert.SetView(Resource.Layout.review);
            alert.SetPositiveButton("Confirm", Confirm_Click);
            alert.SetNegativeButton("Cancel", (sender, e) => { });
            Review = alert.Create();
        }

        private void PayButton_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetTitle("Pseudopayments");
            ad.SetView(Resource.Layout.payment);
            ad.SetPositiveButton("Confirm", Pay);
            ad.SetNegativeButton("Cancel", (sender, e) => { });
            DeclineDialog = ad.Create();
            DeclineDialog.Show();
            discount = new Discount
            {
                Price = Order.Price,
                Paws = Petlance.User.Paws
            };
            DeclineDialog.FindViewById<TextView>(Resource.Id.start_price).Text += Order.Price;
            DeclineDialog.FindViewById<TextView>(Resource.Id.available_paws).Text += $"{discount.AvailablePaws}/{Petlance.User.Paws}";
            TextView totalPrice = DeclineDialog.FindViewById<TextView>(Resource.Id.total_price);
            totalPrice.Text = $"Total price: {Order.Price}";
            checkBox = DeclineDialog.FindViewById<CheckBox>(Resource.Id.use_paws);
            checkBox.CheckedChange += delegate
            {
                discount.Paws = checkBox.Checked ? Petlance.User.Paws : 0;
                totalPrice.Text = $"Total price: {discount.DiscountedPrice}";
            };
        }
        private void Pay(object sender, EventArgs e)
        {
            RightButton.Visibility = ViewStates.Gone;
            Order.IsPaid = true;
            Order.Price = discount.DiscountedPrice;
            Order.Update();
            Petlance.User.Paws -= discount.AvailablePaws;
            Petlance.User.Paws += discount.DiscountCashback;
            Petlance.User.Update();
        }

        private void DeclineButton_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetView(Resource.Layout.decline);
            ad.SetPositiveButton("Confirm", Decline);
            ad.SetNegativeButton("Cancel", (sender, e) => { });
            DeclineDialog = ad.Create();
            DeclineDialog.Show();
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetView(Resource.Layout.accept);
            ad.SetPositiveButton("Confirm", Accept);
            ad.SetNegativeButton("Cancel", (sender, e) => { });
            DeclineDialog = ad.Create();
            DeclineDialog.Show();
            DeclineDialog.FindViewById<TextView>(Resource.Id.calc_price).Text += TotalCalculatedPrice.ToString();
            EditText editText = DeclineDialog.FindViewById<EditText>(Resource.Id.text);
            TextView textView = DeclineDialog.FindViewById<TextView>(Resource.Id.total_price);
            textView.Text += TotalCalculatedPrice;
            editText.TextChanged += delegate
            {
                totalPrice = TotalCalculatedPrice + (editText.Text == "" ? 0 : Convert.ToInt32(editText.Text));
                textView.Text = "Total price: " + totalPrice;
            };

        }

        private void Accept(object sender, EventArgs e)
        {
            Order.Price = totalPrice;
            Order.IsAccepted = true;
            Order.Send();
            AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetMessage("Cheque sent");
            ad.SetPositiveButton("OK", (sender, e) => {
                Finish();
                Parent.UpdateIncomming();
             });
            ad.Create().Show();
        }
        private void AddReviewButton_Click(object sender, EventArgs e) => Review.Show();

        private void Confirm_Click(object sender, EventArgs e)
        {
            List<Animal> animals = new List<Animal>();
            foreach(var animal in Order.Animals)
                animals.Add(animal.Key);
            new Review(0, Petlance.User,
                Order.Offer.Executor,
                Review.FindViewById<EditText>(Resource.Id.text).Text,
                Review.FindViewById<RatingBar>(Resource.Id.rating).Rating,
                animals.ToArray(),
                DateTime.Now).Update();
        }
        private async void Decline(object sender, EventArgs e)
        {
            await EmailService.SendEmail(Order.User, "Order", "Your order was declined. The reasons are:" +
                                             $"<br><b>{DeclineDialog.FindViewById<EditText>(Resource.Id.text).Text}</b>");
            Order.Delete();
            Finish();
            Parent.UpdateIncomming();
        }
    }
}