﻿using Android.App;
using Android.Content;
using Android.Util;
using AndroidX.CoordinatorLayout.Widget;
using System;

namespace Petlance
{
    public static class AdaptiveLayout
    {
        private static DisplayMetrics displayMetrics = Application.Context.Resources.DisplayMetrics;
        public static int vw = displayMetrics.WidthPixels / 100;
        public static int vh = displayMetrics.HeightPixels / 100;
        public static int vmin = Math.Min(vw, vh);
        public static int vmax = Math.Max(vw, vh);
    }
}