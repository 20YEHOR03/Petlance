package crc64fc3325efa7b239bc;


public abstract class FilterActivity
	extends crc64fc3325efa7b239bc.DrawerActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Petlance.FilterActivity, Petlance", FilterActivity.class, __md_methods);
	}


	public FilterActivity ()
	{
		super ();
		if (getClass () == FilterActivity.class)
			mono.android.TypeManager.Activate ("Petlance.FilterActivity, Petlance", "", this, new java.lang.Object[] {  });
	}


	public FilterActivity (int p0)
	{
		super (p0);
		if (getClass () == FilterActivity.class)
			mono.android.TypeManager.Activate ("Petlance.FilterActivity, Petlance", "System.Int32, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
