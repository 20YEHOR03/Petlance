using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    class OrderLayout : LinearLayout
    {
        private int AnimalSize = 7 * vmin;
        Order Order { get; set; }
        OrderType OrderType { get; set; }

        public OrderLayout(Context context, Order order, OrderType orderType) : base(context)
        {
            Order = order;
            OrderType = orderType;
            Initialize();
        }
        private void Initialize()
        {
            SetGravity(GravityFlags.Right);
            SetBackgroundResource(Resource.Drawable.external_login_button_backgroud);
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            LayoutParameters = new MarginLayoutParams(LayoutParameters);
            (LayoutParameters as MarginLayoutParams).SetMargins(0, 2 * vh, 0, 2 * vh);
            Orientation = Orientation.Vertical;
            TextView title = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = Order.Offer.Title
            };
            AddView(title);
            LinearLayout layout = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal
            };
            LinearLayout table = new TableLayout(Context) { LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent), Orientation=Orientation.Vertical };
            LinearLayout tableRow = null;
            int i = 0;
            foreach (var animal in Order.Animals)
            {
                if (i % 2 == 0)
                    tableRow = new LinearLayout(Context) { LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent), Orientation = Orientation.Horizontal };
                ImageView image = new ImageView(Context) { LayoutParameters = new LayoutParams(AnimalSize, AnimalSize) };
                image.SetImageResource(animal.Key.GetResourceType());
                tableRow.AddView(image);
                TextView count = new TextView(Context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, AnimalSize),
                    Text = $"x{animal.Value}"
                };
                tableRow.AddView(count);
                if (i % 2 == 0)
                    table.AddView(tableRow);
                i++;
            }
            layout.AddView(table);
            TextView description = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                Text = Order.Desc
            };
            layout.AddView(description);
            AddView(layout);
            if(Order.Other != null)
            {
                LinearLayout other = new LinearLayout(Context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                    Orientation = Orientation.Horizontal
                };
                ImageView image = new ImageView(Context) { LayoutParameters = new LayoutParams(AnimalSize, AnimalSize) };
                image.SetImageResource(Resource.Drawable.FAQ);
                other.AddView(image);
                TextView text = new TextView(Context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                    Text = $"Other: {Order.Other}"
                };
                other.AddView(text);
                AddView(other);
            }
            TextView state = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = Order.IsPaid ? "Paid" : Order.IsAccepted ? "Pending for client paid" : "Pending for your answer"
            };
            AddView(state);
            Click += RequestLayout_Click;
        }

        private void RequestLayout_Click(object sender, EventArgs e)
        {
            if (Context is PetlanceActivity)
            {
                OrderActivity.Order = Order;
                OrderActivity.Type = OrderType;
                OrderActivity.Prev = Context as UserOrdersActivity;
                (Context as PetlanceActivity).ShowActivity<OrderActivity>();
            }
        }
    }
}