package crc64fc3325efa7b239bc;


public abstract class DrawerActivity
	extends crc64fc3325efa7b239bc.PetlanceActivity
	implements
		mono.android.IGCUserPeer,
		com.google.android.material.navigation.NavigationView.OnNavigationItemSelectedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"n_onCreateOptionsMenu:(Landroid/view/Menu;)Z:GetOnCreateOptionsMenu_Landroid_view_Menu_Handler\n" +
			"n_onOptionsItemSelected:(Landroid/view/MenuItem;)Z:GetOnOptionsItemSelected_Landroid_view_MenuItem_Handler\n" +
			"n_onNavigationItemSelected:(Landroid/view/MenuItem;)Z:GetOnNavigationItemSelected_Landroid_view_MenuItem_Handler:Google.Android.Material.Navigation.NavigationView/IOnNavigationItemSelectedListenerInvoker, Xamarin.Google.Android.Material\n" +
			"";
		mono.android.Runtime.register ("Petlance.DrawerActivity, Petlance", DrawerActivity.class, __md_methods);
	}


	public DrawerActivity ()
	{
		super ();
		if (getClass () == DrawerActivity.class)
			mono.android.TypeManager.Activate ("Petlance.DrawerActivity, Petlance", "", this, new java.lang.Object[] {  });
	}


	public DrawerActivity (int p0)
	{
		super (p0);
		if (getClass () == DrawerActivity.class)
			mono.android.TypeManager.Activate ("Petlance.DrawerActivity, Petlance", "System.Int32, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();


	public boolean onCreateOptionsMenu (android.view.Menu p0)
	{
		return n_onCreateOptionsMenu (p0);
	}

	private native boolean n_onCreateOptionsMenu (android.view.Menu p0);


	public boolean onOptionsItemSelected (android.view.MenuItem p0)
	{
		return n_onOptionsItemSelected (p0);
	}

	private native boolean n_onOptionsItemSelected (android.view.MenuItem p0);


	public boolean onNavigationItemSelected (android.view.MenuItem p0)
	{
		return n_onNavigationItemSelected (p0);
	}

	private native boolean n_onNavigationItemSelected (android.view.MenuItem p0);

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
