using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content.Resources;
using System;
using System.Net;
using Xamarin.Essentials;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    class ReviewLayout : EntityLayout
    {
        private int TitleFontSize = 3 * vmin;
        private int FontSize = 2 * vmin;
        private int AnimalSize = 7 * vmin;
        private int TopPadding = 2 * vh;
        private int BottomPadding = 2 * vh;
        private int RightPadding = 2 * vw;
        private int LeftPadding = 2 * vw;
        private Review review;
        public ReviewLayout(Context context, Review review) : base(context)
        {
            SetPadding(left: LeftPadding,
                       top: TopPadding,
                       right: RightPadding,
                       bottom: BottomPadding);
            this.review = review;
            Initialize();
        }
        private void Initialize()
        {
            SetBackgroundResource(Resource.Drawable.external_login_button_backgroud);
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            LayoutParameters = new MarginLayoutParams(LayoutParameters);
            (LayoutParameters as MarginLayoutParams).SetMargins(0, 2 * vh, 0, 2 * vh);
            LinearLayout layout = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Orientation = Orientation.Vertical
            };
            LinearLayout upper = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal
            };
            upper.SetGravity(Android.Views.GravityFlags.End);
            TextView name = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                Text = review.User.Name,
                TextSize = TitleFontSize
            };
            name.SetForegroundGravity(Android.Views.GravityFlags.End);
            RatingBar ratingBar = new RatingBar(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                NumStars = 5,
                ScaleX = 0.5F,
                ScaleY = 0.5F,
                IsIndicator = true,
                Rating = review.Rate
            };
            upper.AddView(name);
            upper.AddView(ratingBar);
            layout.AddView(upper);
            layout.AddView(new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = review.Description,
                TextSize = FontSize
            });
            layout.AddView(new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = "Left pets:",
                TextSize = TitleFontSize
            });
            LinearLayout bottom = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal
            };
            bottom.SetGravity(Android.Views.GravityFlags.Start);
            for (int i = 0; i < review.Animals.Length; i++)
            {
                ImageView animalIco = new ImageView(Context)
                { LayoutParameters = new LayoutParams(AnimalSize, AnimalSize) };
                animalIco.SetImageResource(new Animal(review.Animals[i].Type, 0).GetResourceType());
                bottom.AddView(animalIco);
            }
            layout.AddView(bottom);
            AddView(layout);
        }
        protected override void YesButton_Click(object sender, EventArgs e) => base.YesButton_Click(sender, e);
    }
}