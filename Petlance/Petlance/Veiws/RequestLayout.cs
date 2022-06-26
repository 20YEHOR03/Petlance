using Android.Content;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    class RequestLayout : LinearLayout
    {
        private OrderType orderType;
        Request Request { get; set; }
        public RequestLayout(Context context, Request request, OrderType orderType) : base(context)
        {
            Request = request;
            this.orderType = orderType;
            Initialize();
        }
        private void Initialize()
        {
            SetBackgroundResource(Resource.Drawable.external_login_button_backgroud);
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            LayoutParameters = new MarginLayoutParams(LayoutParameters);
            (LayoutParameters as MarginLayoutParams).SetMargins(0, 2 * vh, 0, 2 * vh);
            Orientation = Orientation.Vertical;

            AddView(new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = "I don't know what should be there"
            });
            Click += RequestLayout_Click;
        }

        private void RequestLayout_Click(object sender, EventArgs e)
        {
            if (Context is PetlanceActivity)
            {
                OrderActivity.Request = Request;
                OrderActivity.Type = orderType;
                OrderActivity.Parent = Context as UserOrdersActivity;
                (Context as PetlanceActivity).ShowActivity<OrderActivity>();
            }
        }
    }
}