using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using static Android.App.ActionBar;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    class OfferActivity : ReviewListActivity
    {
        bool isFavorite;
        IMenuItem favoriteButton;

        protected Dialog ContactsDialog { get; set; }
        protected Dialog SendDialog { get; set; }
        protected TextView ContactsButton { get; set; }
        protected TextView TakeButton { get; set; }
        protected CheckBox OtherCheckBox { get; set; }
        protected EditText OtherEditText { get; set; }
        List<LinearLayout> Areas { get; set; }

        List<KeyValuePair<CheckBox, EditText>> Animals { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_offer;
            menu_layout = Resource.Menu.favorite;
            base.OnCreate(savedInstanceState);

            TakeButton = FindViewById<TextView>(Resource.Id.take_button);
            TakeButton.Click += TakeButton_Click;
            TakeButton.Visibility = Offer.IsActive ? ViewStates.Visible : ViewStates.Gone;
            if (Petlance.User is Executor)
                TakeButton.Visibility = (Petlance.User as Executor).Id == Offer.Executor.Id ? ViewStates.Gone : ViewStates.Visible;

            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.order);
            alert.SetPositiveButton("Send", Send_Click);
            SendDialog = alert.Create();

            ContactsButton = FindViewById<TextView>(Resource.Id.contacts_button);
            alert = new AlertDialog.Builder(this);
            alert.SetTitle("Contacts");
            alert.SetMessage(Offer.Contacts);
            ContactsDialog = alert.Create();
            ContactsButton.Click += ContactsButton_Click;
            try { FindViewById<ImageView>(Resource.Id.picture).SetImageBitmap(Images.GetBitmapFromBytes(Offer.Executor.Picture)); }
            catch { FindViewById<ImageView>(Resource.Id.picture).SetImageResource(Resource.Drawable.no_image); }
            FindViewById<TextView>(Resource.Id.title).Text = Offer.Executor.Name;
            FindViewById<TextView>(Resource.Id.desc).Text = Offer.Description;
            FindViewById<TextView>(Resource.Id.date).Text = "using service since\n" + Offer.Executor.Date.ToString("MMMM yyyy");
            FindViewById<RatingBar>(Resource.Id.rating).Rating = Offer.Executor.GetRating();
            Initialize(this, Offer);
            isFavorite = Petlance.User != null && Offer.IsFavorite(Petlance.User);

        }

        private void CheckBox_Click(object sender, EventArgs e)
        {
            foreach (var animal in Animals)
                animal.Value.Visibility = animal.Key.Checked ? ViewStates.Visible : ViewStates.Invisible;
        }

        public static void Initialize(PetlanceActivity activity, Offer offer)
        {
            int AnimalSize = 7 * vmin;
            activity.FindViewById<TextView>(Resource.Id.initial_price).Text += offer.InitialPrice;
            LinearLayout pets = activity.FindViewById<LinearLayout>(Resource.Id.pets);
            foreach (Animal animal in offer.Animals)
            {
                LinearLayout layout = new LinearLayout(activity)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent),
                    Orientation = Orientation.Vertical
                };
                layout.SetPadding(0, 0, 2 * vh, 0);
                layout.SetGravity(GravityFlags.Center);
                TextView price = new TextView(activity)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "$" + animal.Price
                };
                ImageView icon = new ImageView(activity) { LayoutParameters = new LayoutParams(AnimalSize, AnimalSize) };
                icon.SetImageResource(animal.GetResourceType());
                layout.AddView(icon);
                layout.AddView(price);
                layout.SetMinimumWidth(price.Width);
                pets.AddView(layout);
            }
            LinearLayout photos = activity.FindViewById<LinearLayout>(Resource.Id.photos);
            foreach (byte[] photo in offer.Photos)
            {
                ImageView imageView = new ImageView(activity) { LayoutParameters = new LayoutParams(60 * vh, 60 * vh) };
                try { imageView.SetImageBitmap(Images.GetBitmapFromBytes(photo)); }
                catch { imageView.SetImageResource(Resource.Drawable.no_image); }
                int TopPadding = 2 * vh;
                int BottomPadding = 2 * vh;
                int RightPadding = 2 * vw;
                int LeftPadding = 2 * vw;
                imageView.SetPadding(left: LeftPadding,
                       top: TopPadding,
                       right: RightPadding,
                       bottom: BottomPadding);
                photos.AddView(imageView);
            }
        }
        private void OtherCheckBox_Click(object sender, EventArgs e) => OtherEditText.Visibility = OtherCheckBox.Checked ? ViewStates.Visible : ViewStates.Gone;

        private void Send_Click(object sender, EventArgs e)
        {
            Order order = new Order(id: -1,
                                          user: Petlance.User,
                                          offer: Offer,
                                          time: DateTime.Now,
                                          price: 0,
                                          isPaid: false,
                                          animals: new Dictionary<Animal, int>(),
                                          other: SendDialog.FindViewById<EditText>(Resource.Id.other).Text,
                                          desc: SendDialog.FindViewById<EditText>(Resource.Id.desc).Text,
                                          isAccepted: false);
            List<Animal> animals = new List<Animal>();
            for (int i = 0; i < Animals.Count; i++)
                if (Animals[i].Key.Checked)
                    order.Animals.Add(new Animal(i), Convert.ToInt32(Animals[i].Value.Text));
            order.Send();
            TakeButton.Visibility = ViewStates.Gone;
        }
        private void TakeButton_Click(object sender, EventArgs e)
        {
            SendDialog.Show();
            Areas = new List<LinearLayout>
            {
                SendDialog.FindViewById<LinearLayout>(Resource.Id.pet0),
                SendDialog.FindViewById<LinearLayout>(Resource.Id.pet1),
                SendDialog.FindViewById<LinearLayout>(Resource.Id.pet2),
                SendDialog.FindViewById<LinearLayout>(Resource.Id.pet3),
                SendDialog.FindViewById<LinearLayout>(Resource.Id.pet4),
                SendDialog.FindViewById<LinearLayout>(Resource.Id.pet5)
            };
            Animals = new List<KeyValuePair<CheckBox, EditText>>()
            {
                new KeyValuePair<CheckBox, EditText>(
                    Areas[0].FindViewById<CheckBox>(Resource.Id.checkbox),
                    Areas[0].FindViewById<EditText>(Resource.Id.editText)),
                new KeyValuePair<CheckBox, EditText>(
                    Areas[1].FindViewById<CheckBox>(Resource.Id.checkbox),
                    Areas[1].FindViewById<EditText>(Resource.Id.editText)),
                new KeyValuePair<CheckBox, EditText>(
                    Areas[2].FindViewById<CheckBox>(Resource.Id.checkbox),
                    Areas[2].FindViewById<EditText>(Resource.Id.editText)),
                new KeyValuePair<CheckBox, EditText>(
                    Areas[3].FindViewById<CheckBox>(Resource.Id.checkbox),
                    Areas[3].FindViewById<EditText>(Resource.Id.editText)),
                new KeyValuePair<CheckBox, EditText>(
                    Areas[4].FindViewById<CheckBox>(Resource.Id.checkbox),
                    Areas[4].FindViewById<EditText>(Resource.Id.editText)),
                new KeyValuePair<CheckBox, EditText>(
                    Areas[5].FindViewById<CheckBox>(Resource.Id.checkbox),
                    Areas[5].FindViewById<EditText>(Resource.Id.editText))
            };
            foreach (var animal in Animals)
                animal.Value.Visibility = ViewStates.Invisible;
            for (int i = 0; i < Areas.Count; i++)
            {
                Areas[i].Visibility = ViewStates.Gone;
                Animals[i].Key.Click += CheckBox_Click;
            }
            foreach (Animal animal in Offer.Animals)
                Areas[animal.Type].Visibility = ViewStates.Visible;
            OtherCheckBox = SendDialog.FindViewById<CheckBox>(Resource.Id.checkBox6);
            OtherCheckBox.Click += OtherCheckBox_Click;
            OtherEditText = SendDialog.FindViewById<EditText>(Resource.Id.other);
        }

        private void ContactsButton_Click(object sender, System.EventArgs e) => ContactsDialog.Show();

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(menu_layout, menu);
            favoriteButton = menu.GetItem(0);
            SetIcon();
            return true;
        }

        private void SetIcon()
        {
            if (Petlance.User != null)
                favoriteButton.SetIcon(isFavorite ? Resource.Drawable.favorite_active : Resource.Drawable.favorite);
            else
                favoriteButton.SetIcon(Resource.Drawable.empty);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    return base.OnOptionsItemSelected(item);
                case Resource.Id.favorite_button:
                    if (Petlance.User != null)
                    {
                        if (isFavorite)
                            isFavorite = !Petlance.User.RemoveFavorite(Offer);
                        else
                            isFavorite = Petlance.User.AddFavorite(Offer);
                        SetIcon();
                    }
                    else
                        favoriteButton.SetIcon(Resource.Drawable.empty);
                    return true;
            }
            return false;
        }
    }
}