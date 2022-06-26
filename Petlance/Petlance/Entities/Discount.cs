using Android.App;
using Android.Content;
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
    class Discount
    {
        public int Price { get; set; }
        public int MaxDiscount => Math.Min(Price / 4, 300);
        public int Paws { get; set; }
        public int AvailablePaws => Math.Min(Paws/10*10, MaxDiscount * 10);
        public int DiscountedPrice => Price - AvailablePaws/10;
        public int DiscountCashback => DiscountedPrice / 10;

    }
}