package crc64f054f7e1a4febbd2;


public class CustomFrameRenderer
	extends crc64e1fb321c08285b90.FrameRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMeasure:(II)V:GetOnMeasure_IIHandler\n" +
			"";
		mono.android.Runtime.register ("Maui.FixesAndWorkarounds.CustomFrameRenderer, PureWeen.Maui.FixesAndWorkarounds", CustomFrameRenderer.class, __md_methods);
	}


	public CustomFrameRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == CustomFrameRenderer.class) {
			mono.android.TypeManager.Activate ("Maui.FixesAndWorkarounds.CustomFrameRenderer, PureWeen.Maui.FixesAndWorkarounds", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
		}
	}


	public CustomFrameRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == CustomFrameRenderer.class) {
			mono.android.TypeManager.Activate ("Maui.FixesAndWorkarounds.CustomFrameRenderer, PureWeen.Maui.FixesAndWorkarounds", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
		}
	}


	public CustomFrameRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == CustomFrameRenderer.class) {
			mono.android.TypeManager.Activate ("Maui.FixesAndWorkarounds.CustomFrameRenderer, PureWeen.Maui.FixesAndWorkarounds", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, System.Private.CoreLib", this, new java.lang.Object[] { p0, p1, p2 });
		}
	}


	public void onMeasure (int p0, int p1)
	{
		n_onMeasure (p0, p1);
	}

	private native void n_onMeasure (int p0, int p1);

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
