using CommunityToolkit.Maui.Views;

namespace MauiApp1.Areas.Exercise.Views;

public partial class ViewVideoContentPage : ContentPage
{
    #region [Fields]

    private string _videoUrl = string.Empty;
    private string _exerciseDescription = string.Empty;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewVideoContentPage(string videoUrl, string exercisedescription)
    {
        InitializeComponent();
        _videoUrl = videoUrl;
        _exerciseDescription = exercisedescription;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_videoUrl) != true)
        {
            this.MediaElement.Source = _videoUrl;
        }
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
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

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Close();
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
                if (this.MediaElement.Handler != null)
                {
                    this.MediaElement.Handler.DisconnectHandler();
                }
            }
            catch (Exception ex)
            {
                //Log Error
                //DisplayAlert("Starting Timer", "An error occurred while starting the timer.", "OK");
            }
        }
    }

    private async void Close()
    {
        this.MediaElement.Stop();
        await Navigation.PopModalAsync();
    }

    #endregion
}