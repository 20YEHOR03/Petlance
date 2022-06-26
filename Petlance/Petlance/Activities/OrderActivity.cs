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
        public static UserOrdersActivity Parent { get; set; }
        protected Dialog Review { get; set; }
        public static OrderType Type { get; set; }
        public static Request Request { get; set; }
        protected TextView AcceptButton { get; set; }
        protected TextView DeclineButton { get; set; }
        protected Dialog AcceptDialog { get; set; }
        List<LinearLayout> Animals { get; set; }
        protected int TotalCalculatedPrice { get; set; }
        public AlertDialog DeclineDialog { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_order;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            OfferActivity.Initialize(this, Request.Offer);
            AcceptButton = FindViewById<TextView>(Resource.Id.accept_button);
            DeclineButton = FindViewById<TextView>(Resource.Id.decline_button);
            TotalCalculatedPrice = Request.Offer.InitialPrice;
            Dictionary<int, Animal> animals = Request.Offer.GetAnimalDictionary();
            foreach (var animal in Request.Animals)
                TotalCalculatedPrice += animal.Value * animals[animal.Key.Type].Price;
            if (Type == OrderType.Outgoing)
            {
                AcceptButton.Text = "Add review";
                FindViewById<TextView>(Resource.Id.total_calculated_price).Visibility = 
                    DeclineButton.Visibility = ViewStates.Gone;
                AcceptButton.Click += AddReviewButton_Click;
            }
            else
            {
                AcceptButton.Click += AcceptButton_Click;
                FindViewById<TextView>(Resource.Id.total_calculated_price).Text += TotalCalculatedPrice;
            }
            DeclineButton.Click += DeclineButton_Click;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Accept");
            //builder.SetView();
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
            foreach(var animal in Request.Animals)
            {
                Animals[animal.Key.Type].Visibility = ViewStates.Visible;
                Animals[animal.Key.Type].FindViewById<CheckBox>(Resource.Id.checkbox).Visibility = ViewStates.Gone;
                Animals[animal.Key.Type].FindViewById<EditText>(Resource.Id.editText).Text = animal.Value.ToString();
            }
            FindViewById<EditText>(Resource.Id.other).Text = Request.Other;
            FindViewById<EditText>(Resource.Id.addins).Text = Request.Desc;


            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Review");
            alert.SetView(Resource.Layout.review);
            alert.SetPositiveButton("Confirm", Confirm_Click);
            alert.SetNegativeButton("Cancel", (sender, e) => { });
            Review = alert.Create();
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
        int totalPrice;
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
                totalPrice = (TotalCalculatedPrice + (editText.Text == "" ? 0 : Convert.ToInt32(editText.Text)));
                textView.Text = "Total price: " + totalPrice;
            };

        }

        private void Accept(object sender, EventArgs e)
        {
            new Order(-1, Request.User, Request.Offer, totalPrice, false).Send();
            AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(this);
            ad.SetMessage("Cheque sent");
            ad.SetPositiveButton("OK", (sender, e) => { });
            ad.Create().Show();
        }
        private void AddReviewButton_Click(object sender, EventArgs e) => Review.Show();

        private void Confirm_Click(object sender, EventArgs e)
        {
            List<Animal> animals = new List<Animal>();
            foreach(var animal in Request.Animals)
                animals.Add(animal.Key);
            new Review(0, Petlance.User,
                Request.Offer.Executor,
                Review.FindViewById<EditText>(Resource.Id.text).Text,
                Review.FindViewById<RatingBar>(Resource.Id.rating).Rating,
                animals.ToArray(),
                DateTime.Now).Update();
        }
        private async void Decline(object sender, EventArgs e)
        {
            await EmailService.SendEmail(Request.User, "Order", "Your order was declined. The reasons are:" +
                                             $"<br><b>{DeclineDialog.FindViewById<EditText>(Resource.Id.text).Text}</b>");
            Request.Delete();
            Finish();
            Parent.Update();
        }
    }
}