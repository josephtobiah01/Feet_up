package crc64f054f7e1a4febbd2;


public class ShellWorkarounds_ShellToolbarTrackerWorkaround
	extends crc640ec207abc449b2ca.ShellToolbarTracker
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Maui.FixesAndWorkarounds.ShellWorkarounds+ShellToolbarTrackerWorkaround, PureWeen.Maui.FixesAndWorkarounds", ShellWorkarounds_ShellToolbarTrackerWorkaround.class, __md_methods);
	}


	public ShellWorkarounds_ShellToolbarTrackerWorkaround ()
	{
		super ();
		if (getClass () == ShellWorkarounds_ShellToolbarTrackerWorkaround.class) {
			mono.android.TypeManager.Activate ("Maui.FixesAndWorkarounds.ShellWorkarounds+ShellToolbarTrackerWorkaround, PureWeen.Maui.FixesAndWorkarounds", "", this, new java.lang.Object[] {  });
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
