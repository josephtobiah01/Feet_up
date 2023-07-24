package crc6461f3faed59cf0423;


public class ShinyGpsService
	extends crc643387a08acbe69b14.ShinyAndroidForegroundService_2
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBind:(Landroid/content/Intent;)Landroid/os/IBinder;:GetOnBind_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("Shiny.Locations.ShinyGpsService, Shiny.Locations", ShinyGpsService.class, __md_methods);
	}


	public ShinyGpsService ()
	{
		super ();
		if (getClass () == ShinyGpsService.class) {
			mono.android.TypeManager.Activate ("Shiny.Locations.ShinyGpsService, Shiny.Locations", "", this, new java.lang.Object[] {  });
		}
	}


	public android.os.IBinder onBind (android.content.Intent p0)
	{
		return n_onBind (p0);
	}

	private native android.os.IBinder n_onBind (android.content.Intent p0);

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
