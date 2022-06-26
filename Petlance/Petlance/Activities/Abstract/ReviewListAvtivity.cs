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
    [Activity(Label = "Favorites", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public abstract class ReviewListActivity : DrawerActivity
    {
        protected int current = 0;
        protected int ReviewCount = 3;
        protected TextView moreButton;
        protected LinearLayout ReviewLayout;
        public static Offer Offer { get; set; }

        public string FROMclause { get; set; }
        public string WHEREclause { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ReviewLayout = FindViewById<LinearLayout>(Resource.Id.reviews);
            moreButton = FindViewById<TextView>(Resource.Id.more_button);
            moreButton.Click += MoreButton_Click;
            MoreButton_Click(new object(), new EventArgs());
        }
        private void FiltersButton_Click(object sender, EventArgs e)
        {
            ReviewLayout.RemoveAllViews();
            current = 0;
            MoreButton_Click(sender, e);
        }
        protected void MoreButton_Click(object sender, EventArgs e)
        {
            int prev = current;
            Review[] reviews = Review.GetReviewsByExecutor(Offer.Executor, current, ReviewCount);
            foreach (Review review in reviews)
                ReviewLayout.AddView(new ReviewLayout(this, review));
            if (reviews.Length == 0 && current == 0)
            {
                TextView view = new TextView(this)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                    Text = "No reviews yet"
                };
                view.SetForegroundGravity(Android.Views.GravityFlags.Start);
                ReviewLayout.AddView(view);
            }
            moreButton.Visibility = (current % ReviewCount == 0 && prev != current) ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
