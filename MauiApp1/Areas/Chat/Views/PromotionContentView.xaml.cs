using ChartApi.Net7;
using MauiApp1.Areas.Chat.ViewModels;
using Microsoft.Maui.Dispatching;
using System.Collections.ObjectModel;

namespace MauiApp1.Areas.Chat.Views;

public partial class PromotionContentView : ContentView
{

    #region [Fields]

    private bool _isRefreshing = false;
    private ObservableCollection<PromotionViewModel> _promotionViewModel;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public PromotionContentView()
    {
        InitializeComponent();
    }

    private async void ContentView_Loaded(object sender, EventArgs e)
    {
        InitializeControl();
        await InitialzeData();
        
    }

    private void ContentView_Unloaded(object sender, EventArgs e)
    {
        ClearPromotionMessage();
    }

    private async Task InitialzeData()
    {
        _promotionViewModel = new ObservableCollection<PromotionViewModel>();

        this.PromotionalCollectionView.ItemsSource = _promotionViewModel;

        if(_isRefreshing == false)
        {
            await RefreshPromotionMessages();
        }
    }

    private void InitializeControl()
    {
        this.PromotionalRefreshView.Command = new Command(() =>
        {
            RefreshPage();

            this.PromotionalRefreshView.IsRefreshing = false;
        });
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void PromotionalCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is PromotionViewModel == true)
        {
            PromotionViewModel promotionViewModel = (e.CurrentSelection.FirstOrDefault() as PromotionViewModel);
            ResetPromotionalCollectionViewSelectedItem();
            OpenPromotionalLinkUrl(promotionViewModel);
        }

    }

    #endregion

    #region [Methods :: Tasks]  

    /// <summary>
    /// Method <c>RefreshPromotionMessages</c> 
    /// - Clears the private field _promotionViewModel.
    /// - Call API of GetPromotionChat.
    /// - Populate private field of _promotionViewModel using foreach.
    /// </summary>
    private async Task RefreshPromotionMessages()
    {
        try
        {
            _isRefreshing = true;

            //Size size = new Size();
            ClearPromotionMessage();

            List<PromotionChatmessage> promotionChatmessages = await MessageApi.Net7.MessageApi.GetPromotionChat();

            foreach (PromotionChatmessage promotionChatmessage in promotionChatmessages)
            {
                LoadPromotionViewModel(promotionChatmessage);
 
                await Task.Delay(10);
            }
            return;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Retrieve Promotional Message", "An error occured while retrieving promotion messages." +
                           " Please check the internet connection and try again.", "OK");
            return;
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private void LoadPromotionViewModel(PromotionChatmessage promotionChatmessage)
    {
        PromotionViewModel promotionViewModel = new PromotionViewModel();
        if (promotionChatmessage.Icon == "Question")
        {
            promotionViewModel.Icon = "question_44x44.png";
        }
        else if (promotionChatmessage.Icon == "Alert")
        {
            promotionViewModel.Icon = "alert_44x44.png";
        }
        else if (promotionChatmessage.Icon == "Information")
        {
            promotionViewModel.Icon = "information_44x44.png";
        }
        else
        {
            // promotionViewModel.Icon = "information_44x44.png";
        }

        promotionViewModel.Title = promotionChatmessage.Title;
        promotionViewModel.Message = promotionChatmessage.Message;
        promotionViewModel.LinkUrl = promotionChatmessage.LinkUrl;
        promotionViewModel.HasImageUrl = !string.IsNullOrEmpty(promotionChatmessage.ImageUrl);
        promotionViewModel.HasIconUrl = !string.IsNullOrEmpty(promotionChatmessage.Icon);
        promotionViewModel.HasLinkUrl = !string.IsNullOrEmpty(promotionChatmessage.LinkUrl);
        promotionViewModel.ImageUrl = promotionChatmessage.ImageUrl;

        promotionViewModel.ImageHeight = 140;
        promotionViewModel.ImageWidth = 343;
        //if (promotionViewModel.HasImageUrl)
        //{
        //    size = await GetImageDimension(promotionViewModel.ImageUrl,343,343);
        //    promotionViewModel.ImageHeight = Convert.ToInt32(size.Height);
        //    promotionViewModel.ImageWidth = Convert.ToInt32(size.Width);
        //}

        _promotionViewModel.Add(promotionViewModel);
    }

    //private void LoadPromotionViewModel(List<PromotionViewModel> promotionViewModels)
    //{
    //    PromotionViewModel promotionViewModel = null;
    //    foreach (PromotionViewModel promotionMessage in promotionViewModels)
    //    {
    //        promotionViewModel = new PromotionViewModel();
    //        promotionViewModel.Icon = promotionMessage.Icon;
    //        promotionViewModel.Title = promotionMessage.Title;
    //        promotionViewModel.Message = promotionMessage.Message;
    //        promotionViewModel.ImageUrl = promotionMessage.ImageUrl;
    //        promotionViewModel.LinkUrl = promotionMessage.LinkUrl;

    //        if (string.IsNullOrWhiteSpace(promotionMessage.Icon) == false)
    //        {
    //            promotionViewModel.HasIconUrl = true;
    //        }
    //        else
    //        {
    //            promotionViewModel.HasIconUrl = false;
    //        }

    //        if (string.IsNullOrWhiteSpace(promotionMessage.ImageUrl) == false)
    //        {
    //            promotionViewModel.HasImageUrl = true;
    //        }
    //        else
    //        {
    //            promotionViewModel.HasImageUrl = false;
    //        }            

    //        _promotionViewModel.Add(promotionViewModel);
    //    }
    //}

    /// <summary>
    /// Method <c>GetImageDimension</c> 
    /// - Returns the image size from image url.
    /// </summary>
    private async Task<Size> GetImageDimension(string imageUrl,double maxHeight, double maxWidth)
    {
        double height = 0;
        double width = 0;
        Size size = new Size();

        try
        {           
            HttpClient httpClient = new HttpClient();
            using (Stream stream = await httpClient.GetStreamAsync(imageUrl))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);


#if ANDROID || IOS
                   Microsoft.Maui.Graphics.IImage image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(memStream);
                    height = image.Height;
                    width = image.Width;
#else
                    System.Drawing.Image image = System.Drawing.Image.FromStream(memStream);
                    height = image.Height;
                    width = image.Width;
#endif

                }

                if(height > maxHeight)
                {
                    size.Height = maxHeight;
                }
                else
                {
                    size.Height = height;
                }

                if(width > maxWidth)
                {
                    size.Width = maxWidth;
                }
                else
                {
                    size.Width = width;
                }
                //size.Height = height; 
                //size.Width = width;
                return size;
            }

        }
        catch
        {
            await App.Current.MainPage.DisplayAlert("Retrieve Image Size", "An error occured while retrieving image size.", "OK");
            return size;
        }
        finally
        {

        }

    }

    private async void OpenPromotionalLinkUrl(PromotionViewModel promotionViewModel)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(promotionViewModel.LinkUrl) == false)
            {
                Uri uri = new Uri(promotionViewModel.LinkUrl);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }

        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Open Browser", "An error occured while opening the browser." +
                           " Please check the internet connection and try again.", "OK");
        }
        finally
        {

        }
    }

    private void ResetPromotionalCollectionViewSelectedItem()
    {
        if (this.PromotionalCollectionView != null)
        {
            this.PromotionalCollectionView.SelectedItem = null;
            this.PromotionalCollectionView.SelectedItems = null;
        }
    }

    private void ClearPromotionMessage()
    {
        if (_promotionViewModel != null)
        {
            _promotionViewModel.Clear();
        }
    }

    #endregion

    #region [Public Methods :: Tasks]  

    public async void RefreshPage()
    {
        //ClearPromotionMessage();
        if (_isRefreshing == false)
        {
            await RefreshPromotionMessages();
        }
        //await RefreshPromotionMessages();
    }

    #endregion


}