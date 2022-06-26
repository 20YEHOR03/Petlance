using Android.Content;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    class OfferLayout : EntityLayout
    {
        private int TitleFontSize = 3 * vmin;
        private int FontSize = 2 * vmin;
        private int ImageSize = 25 * vmin;
        private int AnimalSize = 7 * vmin;
        private int ThisHeight = 20 * vh;
        private int TopPadding = 2 * vh;
        private int BottomPadding = 2 * vh;
        private int RightPadding = 2 * vw;
        private int LeftPadding = 2 * vw;
        private Offer offer;
        public FloatingActionButton editButton;
        public OfferLayout(Context context, Offer offer) : base(context)
        {
            this.offer = offer;
            Initialize();
        }
        private void Initialize()
        {
            SetBackgroundResource(offer.IsActive ? Resource.Drawable.external_login_button_backgroud : Resource.Drawable.external_login_button_backgroud_inactive);
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, ThisHeight);
            SetPadding(left: LeftPadding,
                       top: TopPadding,
                       right: RightPadding,
                       bottom: BottomPadding);
            LinearLayout layout = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent),
                Orientation = Orientation.Horizontal
            };
            layout.SetGravity(Android.Views.GravityFlags.CenterVertical);
            ImageView imageView = new ImageView(Context) { LayoutParameters = new LayoutParams(ImageSize, ImageSize) };
            try { imageView.SetImageBitmap(Images.GetBitmapFromBytes(offer.Executor.Picture)); }
            catch { imageView.SetImageResource(Resource.Drawable.no_image); }
            layout.AddView(imageView);
            LinearLayout Right = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent),
                Orientation = Orientation.Vertical
            };
            Right.SetGravity(Android.Views.GravityFlags.Start);
            LinearLayout Upper = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal
            };
            Upper.SetGravity(Android.Views.GravityFlags.End);
            for (int i = 0; i < offer.Animals.Length && i < 3; i++)
            {
                ImageView animalIco = new ImageView(Context) { LayoutParameters = new LayoutParams(AnimalSize, AnimalSize) };
                animalIco.SetImageResource(offer.Animals[i].GetResourceType());
                Upper.AddView(animalIco);
            }
            if (offer.Animals.Length > 3)
            {
                TextView animalsMore = new TextView(Context)
                {
                    LayoutParameters = new LayoutParams(AnimalSize, AnimalSize),
                    Text = $"+{offer.Animals.Length - 3}",
                    TextSize = TitleFontSize
                };
                Upper.AddView(animalsMore);
            }

            TextView title = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = offer.Title,
                TextSize = TitleFontSize
            };

            TextView desc = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = offer.ShortDescription,
                TextSize = FontSize
            };
            Right.AddView(Upper);
            Right.AddView(title);
            Right.AddView(desc);
            layout.AddView(Right);
            editButton = new FloatingActionButton(Context)
            {
                LayoutParameters = new LayoutParams(ButtonSize, ButtonSize),
                Visibility = (Petlance.User != null && Petlance.User.Id == offer.Executor.Id) ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone,
            };
            AddView(layout);
            AddView(editButton);
            editButton.Click += EditButton_Click;
            Click += OfferLayout_Click;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if ((Context is PetlanceActivity) && ((sender as FloatingActionButton) == editButton))
            {
                EditOfferActivity.Offer = Offer.GetOfferById(offer.Id);
                EditOfferActivity.Prev = Context as PetlanceActivity;
                (Context as PetlanceActivity).ShowActivity<EditOfferActivity>();
            }
        }

        private void OfferLayout_Click(object sender, EventArgs e)
        {
            if (Context is PetlanceActivity)
            {
                OfferActivity.Offer = Offer.GetOfferById(offer.Id);
                (Context as PetlanceActivity).ShowActivity<OfferActivity>();
            }
        }
        protected override void YesButton_Click(object sender, EventArgs e)
        {
            if (Context is OfferListActivity)
            {
                offer.Delete();
                (Context as OfferListActivity).FiltersButton_Click(sender,e);
            }
            base.YesButton_Click(sender, e);
        }
    }
}