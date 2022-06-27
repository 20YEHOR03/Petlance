package crc64fc3325efa7b239bc;


public class OrderLayout
	extends android.widget.LinearLayout
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Petlance.OrderLayout, Petlance", OrderLayout.class, __md_methods);
	}


	public OrderLayout (android.content.Context p0)
	{
		super (p0);
		if (getClass () == OrderLayout.class)
			mono.android.TypeManager.Activate ("Petlance.OrderLayout, Petlance", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public OrderLayout (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == OrderLayout.class)
			mono.android.TypeManager.Activate ("Petlance.OrderLayout, Petlance", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public OrderLayout (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == OrderLayout.class)
			mono.android.TypeManager.Activate ("Petlance.OrderLayout, Petlance", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public OrderLayout (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == OrderLayout.class)
			mono.android.TypeManager.Activate ("Petlance.OrderLayout, Petlance", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
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
