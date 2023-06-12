using FeedApi.Net7.Models;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MauiApp1.Areas.Nutrient.Views;

public partial class SearchRecipesPage : ContentPage
{
    #region[Fields]

#endregion
#region[Initialization]
public SearchRecipesPage(FeedItem IncomingFeedItem,long status)
	{
        //if status==1, display search;
        //if status ==2, display favorites;
        //if status ==2, display history;
        InitializeComponent();
        rootComponent.Parameters = new Dictionary<string, object>
             {
                {"feedItem", IncomingFeedItem},
                {"status", status }
             };
    }


    #endregion
    #region[Events]
    #endregion
    #region[Methods]
    #endregion

}