package crc64fc3325efa7b239bc;


public class UserActivity
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
		mono.android.Runtime.register ("Petlance.UserActivity, Petlance", UserActivity.class, __md_methods);
	}


	public UserActivity ()
	{
		super ();
		if (getClass () == UserActivity.class)
			mono.android.TypeManager.Activate ("Petlance.UserActivity, Petlance", "", this, new java.lang.Object[] {  });
	}


	public UserActivity (int p0)
	{
		super (p0);
		if (getClass () == UserActivity.class)
			mono.android.TypeManager.Activate ("Petlance.UserActivity, Petlance", "System.Int32, mscorlib", this, new java.lang.Object[] { p0 });
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
