using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System;
using static Android.App.ActionBar;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    public class ReasonButton : TextView
    {
        Report Report { get; set; }
        Dialog dialog;
        public ReasonButton(Context context, Report report) : base(context)
        {
            Report = report;
            Click += ReasonButton_Click;
        }

        private void ReasonButton_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetView(Resource.Layout.report);
            dialog = builder.Create();
                dialog.Show();
            dialog.FindViewById<EditText>(Resource.Id.text).Text = Report.Reason;
            var Photos = dialog.FindViewById<LinearLayout>(Resource.Id.photos_create);
            foreach(var photo in Report.Photos)
            {
                ImageView image = new ImageView(Context) { LayoutParameters = new LayoutParams(90 * vmin, 90 * vmin) };
                image.SetImageBitmap(Images.GetBitmapFromBytes(photo));
                Photos.AddView(image);  
            }
        }
    }
}