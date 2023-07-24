package com.shiny.notifications;


public class ShinyNotificationBroadcastReceiver
	extends crc643387a08acbe69b14.ShinyBroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Shiny.Notifications.ShinyNotificationBroadcastReceiver, Shiny.Notifications", ShinyNotificationBroadcastReceiver.class, __md_methods);
	}


	public ShinyNotificationBroadcastReceiver ()
	{
		super ();
		if (getClass () == ShinyNotificationBroadcastReceiver.class) {
			mono.android.TypeManager.Activate ("Shiny.Notifications.ShinyNotificationBroadcastReceiver, Shiny.Notifications", "", this, new java.lang.Object[] {  });
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
