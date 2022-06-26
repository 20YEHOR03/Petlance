using Android.Content;
using Android.Util;
using Android.Widget;

namespace Petlance
{
    public class LinkTextView : TextView
    {
        public LinkTextView(Context context, IAttributeSet attrs) :
            base(context, attrs) => PaintFlags = PaintFlags | Android.Graphics.PaintFlags.UnderlineText;

        public LinkTextView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle) => PaintFlags = PaintFlags | Android.Graphics.PaintFlags.UnderlineText;
    }
}