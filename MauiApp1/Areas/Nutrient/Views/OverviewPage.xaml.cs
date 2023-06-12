using FeedApi.Net7.Models;

namespace MauiApp1.Areas.Nutrient.Views;

public partial class OverviewPage : ContentPage
{
    #region[Fields]
    public int thisangle = 0;
    #endregion
    #region[Initialization]
    public OverviewPage(FeedItem IncomingFeedItem, bool IsSubmitted = false)
	{
		InitializeComponent();
        rootComponent.Parameters = new Dictionary<string, object>
             {
                {"feedItem", IncomingFeedItem},
                {"IsSubmitted", IsSubmitted },
             };
    }



    #endregion
    #region[Events]
    public event EventHandler CLosing;
    #endregion
    #region[Methods]
    #endregion

}