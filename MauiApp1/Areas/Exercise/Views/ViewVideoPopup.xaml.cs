using CommunityToolkit.Maui.Views;

namespace MauiApp1.Areas.Exercise.Views;

public partial class ViewVideoPopup : CommunityToolkit.Maui.Views.Popup
{

    #region [Fields]

    private string _videoUrl = string.Empty;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewVideoPopup(string videoUrl)
    {
        InitializeComponent();
        _videoUrl = videoUrl;
    }

    private void Popup_Opened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(_videoUrl)!= true)
        {
            this.MediaElement.Source= _videoUrl;
        }
    }

    private void Popup_Closed(object sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e)
    {
        UnloadMediaElement();
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void MediaElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        MediaElement mediaElement = (MediaElement)sender;
        PauseMediaElement(mediaElement);
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        ClosePopup();
    }

    #endregion

    #region [Methods :: Tasks]

    private void PauseMediaElement(MediaElement mediaElement)
    {
        if (mediaElement.IsVisible == false)
        {
            switch (mediaElement.CurrentState)
            {
                case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing:
                    mediaElement.Pause();
                    break;
                default:
                    break;
            }

        }
        else
        {
           
        }
    }

    private void UnloadMediaElement()
    {
        if (this.MediaElement != null)
        {
            switch (this.MediaElement.CurrentState)
            {
                case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing:
                case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Paused:
                    this.MediaElement.Stop();
                    break;
                default:
                    break;
            }

            try
            {
                //Crashing issue
                //MediaElement.Handler?.DisconnectHandler();
                //if (this.MediaElement.Handler != null)
                //{
                //    this.MediaElement.Handler.DisconnectHandler();
                //}
            }
            catch(Exception ex)
            {
                //Log Error
                //DisplayAlert("Starting Timer", "An error occurred while starting the timer.", "OK");
            }
        }
    }

   

    private void ClosePopup()
    {
        this.MediaElement.Stop();
        Close();
    }

    #endregion
}