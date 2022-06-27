package crc64fc3325efa7b239bc;


public class LinkTextView
	extends android.widget.TextView
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Petlance.LinkTextView, Petlance", LinkTextView.class, __md_methods);
	}


	public LinkTextView (android.content.Context p0)
	{
		super (p0);
		if (getClass () == LinkTextView.class)
			mono.android.TypeManager.Activate ("Petlance.LinkTextView, Petlance", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public LinkTextView (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == LinkTextView.class)
			mono.android.TypeManager.Activate ("Petlance.LinkTextView, Petlance", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public LinkTextView (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == LinkTextView.class)
			mono.android.TypeManager.Activate ("Petlance.LinkTextView, Petlance", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
