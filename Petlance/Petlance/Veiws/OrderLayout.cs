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
                Text = Order.Offer.Title,
                TextSize = 3 * vmin
            };
            title.LayoutParameters = new MarginLayoutParams(title.LayoutParameters);
            (title.LayoutParameters as MarginLayoutParams).SetMargins(4 * vmin, 2 * vmin, 2 * vmin, 0);
            AddView(title);
            LinearLayout layout = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal
            };
            layout.LayoutParameters = new MarginLayoutParams(layout.LayoutParameters);
            (layout.LayoutParameters as MarginLayoutParams).SetMargins(4 * vmin, 2 * vmin, 2 * vmin, 1 * vmin);
            LinearLayout table = new TableLayout(Context) { LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent), Orientation = Orientation.Vertical };
            LinearLayout tableRow = null;
            table.SetPadding(0, 0, 5 * vmin, 0);
            int i = 0;
            foreach (var animal in Order.Animals)
            {
                if (i % 2 == 0)
                    tableRow = new LinearLayout(Context) { LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent), Orientation = Orientation.Horizontal };
                ImageView image = new ImageView(Context) { LayoutParameters = new LayoutParams(AnimalSize, AnimalSize) };
                image.SetPadding(0, 0, 1 * vmin, 0);
                image.SetImageResource(animal.Key.GetResourceType());
                tableRow.AddView(image);
                TextView count = new TextView(Context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.WrapContent, AnimalSize),
                    Text = $"x{animal.Value}"
                };
                count.SetPadding(0, 0, 2 * vmin, 0);
                tableRow.AddView(count);
                if (i % 2 == 0)
                    table.AddView(tableRow);
                i++;
            }
            layout.AddView(table);
            View view = new View(Context) {
                LayoutParameters = new LayoutParams(2, LayoutParams.MatchParent),
            };
            view.SetBackgroundColor(Android.Graphics.Color.Black);
            view.LayoutParameters = new MarginLayoutParams(view.LayoutParameters);
            (view.LayoutParameters as MarginLayoutParams).SetMargins(0, 0, 1 * vmin, 0);
            layout.AddView(view);
            TextView description = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                Text = Order.Desc,
                Gravity = GravityFlags.Left
            };
            description.LayoutParameters = new MarginLayoutParams(description.LayoutParameters);
            (description.LayoutParameters as MarginLayoutParams).SetMargins(0, 0, 1 * vmin, 0);
            layout.AddView(description);
            AddView(layout);
            if(Order.Other != null && Order.Other != "")
            {
                LinearLayout other = new LinearLayout(Context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                    Orientation = Orientation.Horizontal
                };
                other.LayoutParameters = new MarginLayoutParams(other.LayoutParameters);
                (other.LayoutParameters as MarginLayoutParams).SetMargins(4 * vmin, 2 * vmin, 2 * vmin, 1 * vmin);
                ImageView image = new ImageView(Context) { LayoutParameters = new LayoutParams(AnimalSize, AnimalSize) };
                image.SetImageResource(Resource.Drawable.FAQ);
                image.SetPadding(0, 0, 1 * vmin, 0);
                other.AddView(image);
                TextView text = new TextView(Context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                    Text = $"Other: {Order.Other}",
                    Gravity = GravityFlags.CenterVertical
                };
                text.SetPadding(0, 0, 0, 1 * vmin);
                other.AddView(text);
                AddView(other);
            }
            TextView state = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Text = Order.IsPaid ? "Paid" : Order.IsAccepted ? "Pending for client paid" : "Pending for your answer",
                TextSize = 1.6f * vmin,
                Gravity = GravityFlags.Right
            };
            
            state.SetTextColor(Order.IsPaid ? Android.Graphics.Color.Rgb(136, 192, 121) : Order.IsAccepted ? Android.Graphics.Color.Rgb(186, 89, 89) : Android.Graphics.Color.Rgb(137, 90, 173));
            state.LayoutParameters = new MarginLayoutParams(state.LayoutParameters);
            (state.LayoutParameters as MarginLayoutParams).SetMargins(4 * vmin, 0, 5 * vmin, 2 * vmin);
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