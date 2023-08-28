using DevExpress.Maui.Controls;
using MauiApp1.Areas.Chat.ViewModels.DeviceServices;
using MauiApp1.Business;
using MauiApp1.Business.UserServices;
using MauiApp1.Interfaces;
using MauiApp1.Pages.Chat;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using static MauiApp1.Pages.Chat.ViewChatDetailPage;

namespace MauiApp1.Areas.Chat.Views;

public partial class ViewHybridChatContentPage : ContentPage, INotifyPropertyChanged
{
    #region Fields


    IDispatcherTimer _dispatcherTimer;
    private readonly ISelectedImageService _selectedImageService;
   

    private string _selectedImage;



    #endregion Fields



    #region Properties





    public event EventHandler SendMessageTrigger;

    public event PropertyChangedEventHandler PropertyChanged;

    public string SelectedImage
    {
        get => _selectedImage;
        set
        {
            if (_selectedImage != value)
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }
    }

    #endregion Properties

    #region [Methods :: EventHandlers :: Class]

    #region Constructor

    public ViewHybridChatContentPage(ISelectedImageService selectedImageService)
    {
        InitializeComponent();

        _selectedImageService = selectedImageService ?? throw new ArgumentNullException(nameof(selectedImageService));
        

        this.rootComponent.Parameters = new Dictionary<string, object>
        {
            {"OpenGalleryAndBottomSheet", new EventCallback(null, OpenGalleryBottomSheet) }
        };

        
      //  _selectedImageService.SelectedBottomSheetImageChanged += SelectedImage_SelectedBottomSheetChanged;

 
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeData();
        InitializeControl();
    }


    #endregion Constructor


    private void SelectedImage_SelectedBottomSheetChanged(object sender, string image)
    {
        this.UserSelectedImage.Source = ImageSource.FromUri(new Uri(image));
        SelectedImage = image.ToString();
        _selectedImageService.SelectedChatContentImage = SelectedImage;
        
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task OpenGalleryBottomSheet()
    {
        try
        {
            var result = await _selectedImageService.UploadPhoto();

            if (result != null)
            {
                SelectedImage = result;
#if ANDROID
                this.UserSelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(SelectedImage)));
                this.UserSelectedImage.Aspect = Aspect.AspectFit;

                OpenSelectedImageBottomSheet();
#endif
            }
            else
            {
                CloseSelectedImageBottomSheet();
            }
#if IOS
            this.UserSelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(SelectedImage)));
            this.UserSelectedImage.Aspect = Aspect.AspectFit;

            await Task.Delay(500);
            OpenSelectedImageBottomSheet();

#endif
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "There was a problem selecting an image", "Ok");
        }
        finally
        {

        }
        //try
        //{
        //    var result = await _selectedImageService.UploadPhoto();

        //    if (result != null)
        //    {
        //        SelectedImage = result;
        //        this.UserSelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(SelectedImage)));
        //        this.UserSelectedImage.Aspect = Aspect.AspectFit;
        //        OpenSelectedImageBottomSheet();
        //    }
        //    else
        //    {
        //        await Application.Current.MainPage.DisplayAlert("Error", "There was a problem selecting an image", "Ok");
        //    }

            
        //}
        //catch(Exception ex)
        //{

        //}
        //finally
        //{

        //}        
    }

    private void SendButton_Clicked(object sender, EventArgs e)
    {
        _selectedImageService.SelectedChatContentImage = SelectedImage;
        MessagingCenter.Send(this, "SendMessageTrigger");
        SendPhotoBottomSheet.State = BottomSheetState.Hidden;
        this.UserSelectedImage.Source = "";
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        this.UserSelectedImage.Source = "";
        CloseSelectedImageBottomSheet();
    }
    

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        DisposeControls();
    }

    public async void InitializeData()
    {
        await UserHandler.SetupUser();
    }

    private void InitializeControl()
    {
        this.QueriesRadioButton.IsChecked = true;
        this.FromUsRadioButton.IsChecked = false;

        _dispatcherTimer = Dispatcher.CreateTimer();
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(4000);
        _dispatcherTimer.Tick += Timer_Tick;
        _dispatcherTimer.Start();

        if (ChatHTMLBridge.StopTimerTick != null)
        {
            ChatHTMLBridge.StopTimerTick -= Stop_Timer;
        }
        ChatHTMLBridge.StopTimerTick += Stop_Timer;
    }

    private void OpenSelectedImageBottomSheet()
    {
        if(this.SendPhotoBottomSheet != null)
        {
            this.SendPhotoBottomSheet.State = BottomSheetState.HalfExpanded;
        }        
    }

    private void CloseSelectedImageBottomSheet()
    {
        if (this.SendPhotoBottomSheet != null)
        {
            this.SendPhotoBottomSheet.State = BottomSheetState.Hidden;
        }
    }

#endregion

    #region [Methods :: EventHandlers :: Controls]

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    protected virtual void Stop_Timer(object sender, EventArgs e)
    {
        StopTimer();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        TimerTick();
    }

    private void FromUsRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        HandleFromUsRibbonButtonCheckChange(radioButton);
    }

    private void QueriesRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        HandleQueriesRibbonButtonCheckChange(radioButton);
    }

    #endregion

    #region [Methods :: Tasks]

    private async void Close()
    {
        await Navigation.PopAsync();     
    }

    private void DisposeControls()
    {
        if (_dispatcherTimer != null)
        {
            _dispatcherTimer.Stop();
            _dispatcherTimer = null;
        }

        if (ChatHTMLBridge.StopTimerTick != null)
        {
            ChatHTMLBridge.StopTimerTick -= Stop_Timer;
        }

    }

    private void HandleQueriesRibbonButtonCheckChange(RadioButton radioButton)
    {
        switch (radioButton.IsChecked)
        {
            case true:

                ShowChatBlazorWebView();
                //ShowChatBlazorStackLayout();
                HidePromotionAreaStackLayout();
                StartTimer();
                break;

            case false:

                break;

        }
    }

    private void HandleFromUsRibbonButtonCheckChange(RadioButton radioButton)
    {
        switch (radioButton.IsChecked)
        {
            case true:

                StopTimer();
                HideChatBlazorWebView();
                //HideChatBlazorStackLayout();
                ShowPromotionAreaStackLayout();
                LoadPromotionContentView();
                break;

            case false:

                break;

        }
    }

    private void LoadPromotionContentView()
    {
        if (this.PromotionAreaStackLayout != null)
        {
            if (this.PromotionAreaStackLayout.Children.Count <= 0)
            {
                this.PromotionAreaStackLayout.Add(new PromotionContentView());
            }
            else
            {
                foreach (IView view in this.PromotionAreaStackLayout.Children)
                {
                    if (view is PromotionContentView)
                    {
                        (view as PromotionContentView).RefreshPage();
                    }
                }
            }
        }
    }

    private void HidePromotionAreaStackLayout()
    {
        if (this.PromotionAreaStackLayout != null)
        {
            this.PromotionAreaStackLayout.IsVisible = false;
        }
    }

    private void ShowPromotionAreaStackLayout()
    {
        if (this.PromotionAreaStackLayout != null)
        {
            this.PromotionAreaStackLayout.IsVisible = true;
        }
    }

    private void ShowChatBlazorWebView()
    {
        if (this.ChatBlazorWebView != null)
        {
            this.ChatBlazorWebView.IsVisible = true;
        }
    }
    
    private void HideChatBlazorWebView()
    {
        if (this.ChatBlazorWebView != null)
        {
            this.ChatBlazorWebView.IsVisible = false;
        }
    }

    //private void ShowChatBlazorStackLayout()
    //{
    //    if (this.ChatBlazorStackLayout != null)
    //    {
    //        this.ChatBlazorStackLayout.IsVisible = true;
    //    }
    //}

    //private void HideChatBlazorStackLayout()
    //{
    //    if (this.ChatBlazorStackLayout != null)
    //    {
    //        this.ChatBlazorStackLayout.IsVisible = false;
    //    }
    //}

    private void StartTimer()
    {
        if (_dispatcherTimer != null)
        {
            if(_dispatcherTimer.IsRunning == false)
            {
                _dispatcherTimer.Start();
            }           
        }
    }

    private void StopTimer()
    {
        if (_dispatcherTimer != null)
        {
            _dispatcherTimer.Stop();
        }
    }

    private void TimerTick()
    {
        try
        {
            if (ChatHTMLBridge.RefreshChatData != null)
            {
                ChatHTMLBridge.RefreshChatData.Invoke(this, null);
            }            
        }
        catch (Exception ex)
        {
            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
            }
            
        }
        finally
        {
        }
    }


    #endregion

    
}