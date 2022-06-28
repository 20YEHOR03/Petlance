using Android.App;
using Android.Util;
using System;

namespace Petlance
{
    public static class AdaptiveLayout
    {
        private static readonly DisplayMetrics displayMetrics = Application.Context.Resources.DisplayMetrics;
        public static int vw = displayMetrics.WidthPixels / 100;
        public static int vh = displayMetrics.HeightPixels / 100;
        public static int vmin = Math.Min(vw, vh);
        public static int vmax = Math.Max(vw, vh);
    }
}