using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Petlance
{
    static class Images
    {
        public static Bitmap GetBitmapFromBytes(byte[] bytes) => BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
        public static byte[] GetBytesFromBitmap(Bitmap bitmap)
        {
            if(bitmap == null)return new byte[0];
            using MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
            return stream.ToArray();
        }
    }
}