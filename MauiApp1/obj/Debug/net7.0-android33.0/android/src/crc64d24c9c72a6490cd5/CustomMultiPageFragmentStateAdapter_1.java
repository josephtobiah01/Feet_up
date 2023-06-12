package crc64d24c9c72a6490cd5;


public class CustomMultiPageFragmentStateAdapter_1
	extends androidx.viewpager2.adapter.FragmentStateAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getItemCount:()I:GetGetItemCountHandler\n" +
			"n_createFragment:(I)Landroidx/fragment/app/Fragment;:GetCreateFragment_IHandler\n" +
			"n_getItemId:(I)J:GetGetItemId_IHandler\n" +
			"n_containsItem:(J)Z:GetContainsItem_JHandler\n" +
			"";
		mono.android.Runtime.register ("Microsoft.Maui.Controls.Platform.CustomMultiPageFragmentStateAdapter`1, PureWeen.Maui.FixesAndWorkarounds", CustomMultiPageFragmentStateAdapter_1.class, __md_methods);
	}


	public CustomMultiPageFragmentStateAdapter_1 (androidx.fragment.app.Fragment p0)
	{
		super (p0);
		if (getClass () == CustomMultiPageFragmentStateAdapter_1.class) {
			mono.android.TypeManager.Activate ("Microsoft.Maui.Controls.Platform.CustomMultiPageFragmentStateAdapter`1, PureWeen.Maui.FixesAndWorkarounds", "AndroidX.Fragment.App.Fragment, Xamarin.AndroidX.Fragment", this, new java.lang.Object[] { p0 });
		}
	}


	public CustomMultiPageFragmentStateAdapter_1 (androidx.fragment.app.FragmentActivity p0)
	{
		super (p0);
		if (getClass () == CustomMultiPageFragmentStateAdapter_1.class) {
			mono.android.TypeManager.Activate ("Microsoft.Maui.Controls.Platform.CustomMultiPageFragmentStateAdapter`1, PureWeen.Maui.FixesAndWorkarounds", "AndroidX.Fragment.App.FragmentActivity, Xamarin.AndroidX.Fragment", this, new java.lang.Object[] { p0 });
		}
	}


	public CustomMultiPageFragmentStateAdapter_1 (androidx.fragment.app.FragmentManager p0, androidx.lifecycle.Lifecycle p1)
	{
		super (p0, p1);
		if (getClass () == CustomMultiPageFragmentStateAdapter_1.class) {
			mono.android.TypeManager.Activate ("Microsoft.Maui.Controls.Platform.CustomMultiPageFragmentStateAdapter`1, PureWeen.Maui.FixesAndWorkarounds", "AndroidX.Fragment.App.FragmentManager, Xamarin.AndroidX.Fragment:AndroidX.Lifecycle.Lifecycle, Xamarin.AndroidX.Lifecycle.Common", this, new java.lang.Object[] { p0, p1 });
		}
	}


	public int getItemCount ()
	{
		return n_getItemCount ();
	}

	private native int n_getItemCount ();


	public androidx.fragment.app.Fragment createFragment (int p0)
	{
		return n_createFragment (p0);
	}

	private native androidx.fragment.app.Fragment n_createFragment (int p0);


	public long getItemId (int p0)
	{
		return n_getItemId (p0);
	}

	private native long n_getItemId (int p0);


	public boolean containsItem (long p0)
	{
		return n_containsItem (p0);
	}

	private native boolean n_containsItem (long p0);

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
