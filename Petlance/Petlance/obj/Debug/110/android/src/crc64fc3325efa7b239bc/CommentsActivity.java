package crc64fc3325efa7b239bc;


public class CommentsActivity
	extends crc64fc3325efa7b239bc.ReviewListActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Petlance.CommentsActivity, Petlance", CommentsActivity.class, __md_methods);
	}


	public CommentsActivity ()
	{
		super ();
		if (getClass () == CommentsActivity.class)
			mono.android.TypeManager.Activate ("Petlance.CommentsActivity, Petlance", "", this, new java.lang.Object[] {  });
	}


	public CommentsActivity (int p0)
	{
		super (p0);
		if (getClass () == CommentsActivity.class)
			mono.android.TypeManager.Activate ("Petlance.CommentsActivity, Petlance", "System.Int32, mscorlib", this, new java.lang.Object[] { p0 });
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
