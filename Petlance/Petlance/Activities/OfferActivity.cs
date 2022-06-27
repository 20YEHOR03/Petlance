using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;
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
        protected TextView ReportButton { get; set; }
        protected CheckBox OtherCheckBox { get; set; }
        protected EditText OtherEditText { get; set; }
        protected LinearLayout Photos { get; set; }
        List<LinearLayout> Areas { get; set; }
        protected Dictionary<ImageView, KeyValuePair<byte[], CoordinatorLayout>> ImagesDict { get; set; } = new Dictionary<ImageView, KeyValuePair<byte[], CoordinatorLayout>>();
        List<KeyValuePair<CheckBox, EditText>> Animals { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_offer;
            menu_layout = Resource.Menu.favorite;
            base.OnCreate(savedInstanceState);
            TakeButton = FindViewById<TextView>(Resource.Id.take_button);
            TakeButton.Click += TakeButton_Click;
            TakeButton.Visibility = ((Petlance.Admin == null) && Offer.IsActive) ? ViewStates.Visible : ViewStates.Gone;
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
            FindViewById<TextView>(Resource.Id.date).Text = "using service since\n" + Offer.Executor.Date.ToString("MMMM yyyy");
            FindViewById<RatingBar>(Resource.Id.rating).Rating = Offer.Executor.GetRating();
            Initialize(this, Offer);
            isFavorite = Petlance.User != null && Offer.IsFavorite(Petlance.User);
            ReportButton = FindViewById<TextView>(Resource.Id.report);
            ReportButton.Click += ReportButton_Click;

        }
        Dialog dialog;
        private void ReportButton_Click(object sender, EventArgs e)
        {
            dialog = GetDialog(Resource.Layout.report_prompt, delegate
            {
                List<byte[]> bitmaps = new List<byte[]>();
                foreach (var pair in ImagesDict)
                    bitmaps.Add(pair.Value.Key);
                new Report(-1, Offer, dialog.FindViewById<EditText>(Resource.Id.text).Text, bitmaps.ToArray()).Send();
            });
            dialog.Show();
            dialog.FindViewById<TextView>(Resource.Id.add_photo).Click += AddPhotoButton_Click;
            Photos = dialog.FindViewById<LinearLayout>(Resource.Id.photos_create);
        }
        protected async void AddPhotoButton_Click(object sender, EventArgs e)
        {
            try
            {
                var results = await FilePicker.PickMultipleAsync(PickOptions.Images);
                foreach (var result in results)
                    if (result != null)
                    {
                        using var stream = await result.OpenReadAsync();
                        using MemoryStream memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        AddImage(memoryStream.ToArray());
                    }

            }
            catch { }
        }

        protected void AddImage(byte[] bitmap)
        {
            CoordinatorLayout layout = new CoordinatorLayout(this) { LayoutParameters = new LayoutParams(90 * vmin, 90 * vmin) };
            ImageView image = new ImageView(this) { LayoutParameters = new LayoutParams(90 * vmin, 90 * vmin) };
            image.SetImageBitmap(Images.GetBitmapFromBytes(bitmap));
            ImageView button = new ImageView(this) { LayoutParameters = new LayoutParams(10 * vmin, 10 * vmin) };
            button.SetImageResource(Resource.Drawable.ic_mtrl_chip_close_circle);
            button.SetBackgroundResource(Resource.Drawable.external_login_button_backgroud);
            button.Click += DeleteImageButton_Click;
            ImagesDict.Add(button, new KeyValuePair<byte[], CoordinatorLayout>(bitmap, layout));
            layout.AddView(image);
            layout.AddView(button);
            Photos.AddView(layout);
        }

        private void DeleteImageButton_Click(object sender, EventArgs e)
        {
            var pair = ImagesDict[sender as ImageView];
            Photos.RemoveView(pair.Value);
            ImagesDict.Remove(sender as ImageView);
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
            if (offer.Animals.Length == 0)
                pets.AddView(new TextView(activity)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No pets chosen"
                });
            else
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
            if (offer.Photos.Length == 0)
                photos.AddView(new TextView(activity)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No photos"
                });
            else
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
            activity.FindViewById<TextView>(Resource.Id.desc).Text = offer.Description;
        }
        private void OtherCheckBox_Click(object sender, EventArgs e) => OtherEditText.Visibility = OtherCheckBox.Checked ? ViewStates.Visible : ViewStates.Gone;

        private void Send_Click(object sender, EventArgs e)
        {

            EditText daysEditText = SendDialog.FindViewById<EditText>(Resource.Id.days);
            if(daysEditText.Text != "" && daysEditText.Text != "0")
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
                                              isAccepted: false,
                                              days: Convert.ToInt32(daysEditText.Text));
                List<Animal> animals = new List<Animal>();
                for (int i = 0; i < Animals.Count; i++)
                    if (Animals[i].Key.Checked)
                        order.Animals.Add(new Animal(i), Convert.ToInt32(Animals[i].Value.Text));
                order.Send();
                TakeButton.Visibility = ViewStates.Gone;
            }
            else
                GetDialog("", "Please write days count", "OK");
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