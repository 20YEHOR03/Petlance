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
    class EditOfferActivity : DrawerActivity
    {
        protected int checkedIndex = 0;
        protected TextView NextButton { get; set; }
        protected TextView PrevButton { get; set; }
        protected TextView AddPhotoButton { get; set; }
        protected LinearLayout Photos { get; set; }
        protected EditText ShortDesc { get; set; }
        protected EditText OfferTitle { get; set; }
        protected EditText LongDesc { get; set; }
        protected EditText InitialPrice { get; set; }
        protected EditText Contacts { get; set; }
        protected List<RadioButton> RadioButtons { get; set; }
        protected List<LinearLayout> Steps { get; set; }
        protected Dictionary<ImageView, KeyValuePair<Bitmap, CoordinatorLayout>> ImagesDict { get; set; } = new Dictionary<ImageView, KeyValuePair<Bitmap, CoordinatorLayout>>();
        protected List<KeyValuePair<CheckBox, EditText>> Animals { get; set; }
        public static Offer Offer { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_create;
            menu_layout = Resource.Menu.no_menu;
            base.OnCreate(savedInstanceState);
            OfferTitle = FindViewById<EditText>(Resource.Id.title);
            ShortDesc = FindViewById<EditText>(Resource.Id.short_description);
            LongDesc = FindViewById<EditText>(Resource.Id.long_description);
            Photos = FindViewById<LinearLayout>(Resource.Id.photos_create);
            RadioButtons = new List<RadioButton>()
            {
                FindViewById<RadioButton>(Resource.Id.page_indicator1),
                FindViewById<RadioButton>(Resource.Id.page_indicator2),
                FindViewById<RadioButton>(Resource.Id.page_indicator3),
                FindViewById<RadioButton>(Resource.Id.page_indicator4)
            };
            Steps = new List<LinearLayout>()
            {
                FindViewById<LinearLayout>(Resource.Id.step1),
                FindViewById<LinearLayout>(Resource.Id.step2),
                FindViewById<LinearLayout>(Resource.Id.step3),
                FindViewById<LinearLayout>(Resource.Id.step4)
            };
            List<LinearLayout> Areas = new List<LinearLayout>
            {
                FindViewById<LinearLayout>(Resource.Id.pet0),
                FindViewById<LinearLayout>(Resource.Id.pet1),
                FindViewById<LinearLayout>(Resource.Id.pet2),
                FindViewById<LinearLayout>(Resource.Id.pet3),
                FindViewById<LinearLayout>(Resource.Id.pet4),
                FindViewById<LinearLayout>(Resource.Id.pet5)
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
            foreach (var button in RadioButtons)
                button.Click += RadioGroup_Click;
            PrevButton = FindViewById<TextView>(Resource.Id.fab_prev);
            NextButton = FindViewById<TextView>(Resource.Id.fab_next);
            AddPhotoButton = FindViewById<TextView>(Resource.Id.add_photo);
            PrevButton.Click += PrevButton_Click;
            NextButton.Click += NextButton_Click;
            AddPhotoButton.Click += AddPhotoButton_Click;
            RadioGroup_Click(new object(), new EventArgs());
            InitialPrice = FindViewById<EditText>(Resource.Id.initial_price);
            Contacts = FindViewById<EditText>(Resource.Id.contacts);
            if (Offer != null)
            {
                OfferTitle.Text = Offer.Title;
                ShortDesc.Text = Offer.ShortDescription;
                LongDesc.Text = Offer.Description;
                InitialPrice.Text = Offer.InitialPrice.ToString();
                Contacts.Text = Offer.Contacts;
                foreach (var animal in Offer.Animals)
                {
                    Animals[animal.Type].Key.Checked = true;
                    Animals[animal.Type].Value.Text = animal.Price.ToString();
                }
                foreach (var photo in Offer.Photos)
                    AddImage(photo);
            }
            else
            {
                Offer = new Offer();
                Contacts.Text = $"Email: {Petlance.User.Email}\nPhone: {Petlance.User.Phone}";
            }
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
            ImagesDict.Add(button, new KeyValuePair<Bitmap, CoordinatorLayout>(Images.GetBitmapFromBytes(bitmap), layout));
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
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (++checkedIndex == 4)
            {
                if (OfferTitle.Text != "")
                {
                    List<byte[]> bitmaps = new List<byte[]>();
                    List<Animal> animals = new List<Animal>();
                    foreach (var pair in ImagesDict)
                        bitmaps.Add(Images.GetBytesFromBitmap(pair.Value.Key));
                    for (int i = 0; i < Animals.Count; i++)
                        if (Animals[i].Key.Checked)
                            animals.Add(new Animal(i, Convert.ToInt32(Animals[i].Value.Text)));
                    string initPrice = FindViewById<EditText>(Resource.Id.initial_price).Text;
                    if (initPrice == "") initPrice = null;
                    Offer = new Offer(
                        OfferTitle.Text,
                        ShortDesc.Text,
                        LongDesc.Text,
                        Convert.ToInt32(initPrice),
                        FindViewById<EditText>(Resource.Id.contacts).Text,
                        Offer.IsActive,
                        Petlance.User as Executor,
                        Offer.Entopped,
                        animals.ToArray(),
                        bitmaps.ToArray())
                    { Id = Offer.Id};
                    Offer.Update();
                    Prev.Recreate();
                    Finish();
                    return;
                }
                else checkedIndex--;
            }
            RadioButtons[checkedIndex].Checked = true;
            RadioGroup_Click(sender, e);

        }
        private void PrevButton_Click(object sender, EventArgs e)
        {
            RadioButtons[--checkedIndex].Checked = true;
            RadioGroup_Click(sender, e);
        }

        private void RadioGroup_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Steps.Count; i++)
                if (RadioButtons[i].Checked)
                    Steps[checkedIndex = i].Visibility = ViewStates.Visible;
                else
                    Steps[i].Visibility = ViewStates.Gone;
            PrevButton.Visibility = checkedIndex == 0 ? ViewStates.Gone : ViewStates.Visible;
            NextButton.Text = (checkedIndex == 3) ? "Done" : "Next";
        }
    }
}