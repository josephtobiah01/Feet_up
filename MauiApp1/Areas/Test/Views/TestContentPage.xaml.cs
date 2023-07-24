using DevExpress.Maui.Controls;
using CommunityToolkit.Maui.Core.Platform;
using Color = Microsoft.Maui.Graphics.Color;

namespace MauiApp1.Areas.Test.Views;

public partial class TestContentPage
{
    public TestContentPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        //CommunityToolkit.Maui.Core.Platform.StatusBar.SetColor(Color.FromRgb(255,255,255));
        //CommunityToolkit.Maui.Core.Platform.StatusBar.SetStyle(CommunityToolkit.Maui.Core.StatusBarStyle.DarkContent);
        bottomSheet.State = BottomSheetState.HalfExpanded;
    }
}