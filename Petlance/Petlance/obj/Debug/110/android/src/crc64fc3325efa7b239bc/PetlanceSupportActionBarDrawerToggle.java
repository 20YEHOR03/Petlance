package crc64fc3325efa7b239bc;


public class PetlanceSupportActionBarDrawerToggle
	extends androidx.appcompat.app.ActionBarDrawerToggle
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDrawerOpened:(Landroid/view/View;)V:GetOnDrawerOpened_Landroid_view_View_Handler\n" +
			"";
		mono.android.Runtime.register ("Petlance.PetlanceSupportActionBarDrawerToggle, Petlance", PetlanceSupportActionBarDrawerToggle.class, __md_methods);
	}


	public PetlanceSupportActionBarDrawerToggle (android.app.Activity p0, androidx.drawerlayout.widget.DrawerLayout p1, androidx.appcompat.widget.Toolbar p2, int p3, int p4)
	{
		super (p0, p1, p2, p3, p4);
		if (getClass () == PetlanceSupportActionBarDrawerToggle.class)
			mono.android.TypeManager.Activate ("Petlance.PetlanceSupportActionBarDrawerToggle, Petlance", "Android.App.Activity, Mono.Android:AndroidX.DrawerLayout.Widget.DrawerLayout, Xamarin.AndroidX.DrawerLayout:AndroidX.AppCompat.Widget.Toolbar, Xamarin.AndroidX.AppCompat:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3, p4 });
	}


	public PetlanceSupportActionBarDrawerToggle (android.app.Activity p0, androidx.drawerlayout.widget.DrawerLayout p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == PetlanceSupportActionBarDrawerToggle.class)
			mono.android.TypeManager.Activate ("Petlance.PetlanceSupportActionBarDrawerToggle, Petlance", "Android.App.Activity, Mono.Android:AndroidX.DrawerLayout.Widget.DrawerLayout, Xamarin.AndroidX.DrawerLayout:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public void onDrawerOpened (android.view.View p0)
	{
		n_onDrawerOpened (p0);
	}

	private native void n_onDrawerOpened (android.view.View p0);

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
