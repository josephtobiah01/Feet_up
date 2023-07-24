package com.shiny.locations;


public class GeofenceBroadcastReceiver
	extends crc643387a08acbe69b14.ShinyBroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Shiny.Locations.GeofenceBroadcastReceiver, Shiny.Locations", GeofenceBroadcastReceiver.class, __md_methods);
	}


	public GeofenceBroadcastReceiver ()
	{
		super ();
		if (getClass () == GeofenceBroadcastReceiver.class) {
			mono.android.TypeManager.Activate ("Shiny.Locations.GeofenceBroadcastReceiver, Shiny.Locations", "", this, new java.lang.Object[] {  });
		}
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
