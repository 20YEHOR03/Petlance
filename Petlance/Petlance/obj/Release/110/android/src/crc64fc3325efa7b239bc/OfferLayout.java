package crc64fc3325efa7b239bc;


public class OfferLayout
	extends crc64fc3325efa7b239bc.EntityLayout
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Petlance.OfferLayout, Petlance", OfferLayout.class, __md_methods);
	}


	public OfferLayout (android.content.Context p0)
	{
		super (p0);
		if (getClass () == OfferLayout.class)
			mono.android.TypeManager.Activate ("Petlance.OfferLayout, Petlance", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public OfferLayout (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == OfferLayout.class)
			mono.android.TypeManager.Activate ("Petlance.OfferLayout, Petlance", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public OfferLayout (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == OfferLayout.class)
			mono.android.TypeManager.Activate ("Petlance.OfferLayout, Petlance", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
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
