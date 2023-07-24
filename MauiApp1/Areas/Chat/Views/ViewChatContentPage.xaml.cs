using MauiApp1.Areas.Chat.Models;
using ParentMiddleWare;
using System.Collections.ObjectModel;
using MessageApi.Net7;
//using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Application = Microsoft.Maui.Controls.Application;
using CommunityToolkit.Maui.Core.Platform;

namespace MauiApp1.Areas.Chat.Views;

public partial class ViewChatContentPage : ContentPage
{

    #region[Fields]

    private ObservableCollection<IMessage> _messageList = new ObservableCollection<IMessage>();
    private List<RecievedMessage> _messageListReserve;

    IDispatcherTimer _dispatcherTimer;

    private bool _isUserScroll = false;
    private bool _isFirstLoad = true;

    private int _pageSize = 1;
    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewChatContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeData();
        InitializeControl();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        UnloadControl();
        DisposeControls();
    }

    public async void InitializeData()
    {

        try
        {
            this.MessageCollectionView.ItemsSource = _messageList;
            //BindableLayout.SetItemsSource(this.MessageVerticalStackLayout, _messageList);
            IMessage message = null;
            _messageListReserve = await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.UserID, DateTime.UtcNow.AddDays(-7));

            int count = _messageListReserve.Count;
            if (_messageListReserve.Count > 0)
            {

                for (int index = 0; index < _messageListReserve.Count; index++)
                {
                    RecievedMessage recievedMessage = _messageListReserve.ElementAt(index);
                    message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, recievedMessage.MessageContent);
                    _messageList.Add(message);
                }

                _isUserScroll = false;
                message = _messageList.ElementAt(_messageList.Count - 1);
                this.MessageCollectionView.ScrollTo(message, position: ScrollToPosition.End);
                //await this.MessageScrollView.ScrollToAsync(this.MessageVerticalStackLayout, ScrollToPosition.End,true);
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Retrieve Message", ex.Message, "OK");
            //await DisplayAlert("Retrieve Message", "An error occured while retrieving the messages." +
            //                " Please check the internet connection and try again.", "OK");
        }
        finally
        {
        }

    }

    private void InitializeControl()
    {
        ComputeMessageChatContentRowDefinition();

        this.QueriesRadioButton.IsChecked = true;
        this.FromUsRadioButton.IsChecked = false;

        _dispatcherTimer = Dispatcher.CreateTimer();
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(4000);
        _dispatcherTimer.Tick += Timer_Tick;
        _dispatcherTimer.Start();

        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            if (this.ChatBoxScrollView != null)
            {
                this.ChatBoxScrollView.IsEnabled = false;
            }
        }
         

        //App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

        this.MessageRefreshView.Command = new Command(() =>
        {
            //this.MessageRefreshView.IsRefreshing = true;
            LoadPreviousMessage();
            // IsRefreshing is true
            // Refresh data here
            this.MessageRefreshView.IsRefreshing = false;
        });
    }

    private void UnloadControl()
    {
        // Set the keyboard input mode back to Resize
        //App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void SendButton_Clicked(object sender, EventArgs e)
    {
        SendMessage(this.ChatMessageEntry.Text);
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
        TimerTick();
    }

    private void ChatMessageEntry_Focused(object sender, FocusEventArgs e)
    {
        TranslateUpChatBoxForIOS();
    }

    private void ChatMessageEntry_Unfocused(object sender, FocusEventArgs e)
    {
        TranslateBackChatBoxForIOS();
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

    public async void SendMessage(String Text)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Text) == false)
            {
                IMessage message = null;
                RecievedMessage sentMessage = await MessageApi.Net7.MessageApi.SendMessage(new FrontendMessage() { Fk_Sender_Id = MiddleWare.UserID, MessageContent = Text });

                if (sentMessage != null)
                {
                    message = new IMessage(sentMessage.TimeStamp.ToLocalTime(), sentMessage.UserName, sentMessage.IsUserMessage, sentMessage.MessageContent);
                    _messageList.Add(message);
                    _isUserScroll = false;

                    this.MessageCollectionView.ScrollTo(message, true);
                    //await this.MessageScrollView.ScrollToAsync(this.MessageVerticalStackLayout, ScrollToPosition.End, true);
                }
            }
            this.ChatMessageEntry.Text = null;
        }
        catch (Exception ex)
        {
            //await DisplayAlert("Send Message", ex.Message, "OK");

            await DisplayAlert("Send Message", "An error occured while sending the messages." +
                            " Please check the internet connection and try again.", "OK");
        }
        finally
        {

        }
    }

    private async void TimerTick()
    {
        try
        {
            IMessage message = null;
            List<RecievedMessage> recievedMessages = await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.UserID, DateTime.UtcNow.AddSeconds(-30));

            foreach (RecievedMessage recievedMessage in recievedMessages)
            {
                if (_messageList.Where(t => t.TimeStamp.Equals(recievedMessage.TimeStamp.ToLocalTime())).FirstOrDefault() != null)
                {
                    continue;
                }
                else
                {
                    if (recievedMessage.IsUserMessage == false)
                    {
                        message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, recievedMessage.MessageContent);

                        _messageList.Add(message);
                    }
                }
            }

            if (message != null)
            {
                this.MessageCollectionView.ScrollTo(message, true);
                //await this.MessageScrollView.ScrollToAsync(this.MessageVerticalStackLayout, ScrollToPosition.End, true);
            }
            else
            {



            }
        }
        catch (Exception ex)
        {

            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
            }
            //await DisplayAlert("Retrieve Message", ex.Message, "OK");
            await DisplayAlert("Retrieve Message", "An error occured while retrieving the messages." +
                           " Please check the internet connection and try again.", "OK");
        }
        finally
        {
        }
    }

    private void DisposeControls()
    {
        if (_dispatcherTimer != null)
        {
            _dispatcherTimer.Stop();
            _dispatcherTimer = null;
        }

    }

    private void LoadPreviousMessage()
    {
        IMessage message = null;

        decimal computedMaxPageSize = _messageListReserve.Count / 12;
        int maxPageSize = (int)Math.Ceiling(computedMaxPageSize);

        int requestedPageNumber = _pageSize + 1;
        _pageSize = requestedPageNumber;
        int maxCounttoQuery = requestedPageNumber * 12;

        int startingIndex = 0;
        //_messageList.Clear();

        if (_pageSize == maxPageSize)
        {
            _messageList.Clear();
            foreach (RecievedMessage recievedMessage in _messageListReserve)
            {
                message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, recievedMessage.MessageContent);
                _messageList.Add(message);
            }
        }
        else if (_pageSize < maxPageSize)
        {
            _messageList.Clear();
            startingIndex = _messageListReserve.Count - maxCounttoQuery;
            for (int index = startingIndex; index < _messageListReserve.Count; index++)
            {
                RecievedMessage recievedMessage = _messageListReserve.ElementAt(index);
                message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, recievedMessage.MessageContent);
                _messageList.Add(message);
            }
        }
        else if (_pageSize > maxPageSize)
        {
            return;
        }
        else
        {
            return;
        }

    }

    private void ComputeMessageChatContentRowDefinition()
    {
        //this.RootScrollView.HeightRequest = Application.Current.MainPage.Height - 8;
#if IOS

        if (this.ContentRowDefinition != null)
        {
            this.ContentRowDefinition.Height = new GridLength(9, GridUnitType.Star);
        }            

        if (this.ChatEntryContentRowDefinition != null)
        {
            this.ChatEntryContentRowDefinition.Height = new GridLength(1, GridUnitType.Star);
        }

        //double screenHeight = Application.Current.MainPage.Height;
        //int computedHeight = (int)Math.Round(screenHeight * 0.71);
        ////if (this.MessageCollectionView != null)
        ////{
        ////    this.MessageCollectionView.HeightRequest = computedHeight;
        ////}

        //if (this.ChatContentRowDefinition != null)
        //{
        //    this.ChatContentRowDefinition.Height = computedHeight;
        //}

#else
        if (this.ChatContentRowDefinition != null)
        {
            this.ChatContentRowDefinition.Height = new GridLength(1, GridUnitType.Star);
        }
#endif

        if (this.ChatEntryContentRowDefinition != null)
        {
            this.ChatEntryContentRowDefinition.Height = new GridLength(1, GridUnitType.Star);
        }

    }

    private void ComputePromotionChatContentRowDefinition()
    {

#if IOS
        double screenHeight = Application.Current.MainPage.Height;
        int computedHeight = (int)Math.Round(screenHeight * 0.8);

        if (this.ChatContentRowDefinition != null)
        {
            this.ChatContentRowDefinition.Height = computedHeight;
        }


#else
        if (this.ChatContentRowDefinition != null)
        {
            this.ChatContentRowDefinition.Height = new GridLength(1, GridUnitType.Star);
        }

#endif

        if (this.ChatEntryContentRowDefinition != null)
        {
            this.ChatEntryContentRowDefinition.Height = 0;
        }
    }

    private async void TranslateUpChatBoxForIOS()
    {
#if IOS
        if (KeyboardExtensions.IsSoftKeyboardShowing(this.ChatMessageEntry) == true)
        {
            await KeyboardExtensions.ShowKeyboardAsync(this.ChatMessageEntry, new CancellationToken());
           
        }
         //this.RootScrollView.IsEnabled = true;

        if (this.ChatEntryContentRowDefinition != null)
        {
            this.ChatEntryContentRowDefinition.Height = new GridLength(7.8, GridUnitType.Star);
        }
#endif

        //if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        //{
        //    this.ChatBoxFrame.TranslateTo(0, -310, 50);
        //}
    }

    private async void TranslateBackChatBoxForIOS()
    {
#if IOS

        if (KeyboardExtensions.IsSoftKeyboardShowing(this.ChatMessageEntry) == false)
        {
            await KeyboardExtensions.HideKeyboardAsync(this.ChatMessageEntry, new CancellationToken());
            //await this.RootScrollView.ScrollToAsync(this.RootGrid,ScrollToPosition.Start,true);
            //this.RootScrollView.IsEnabled = false;
           
        }
        
        if (this.ChatEntryContentRowDefinition != null)
        {
            this.ChatEntryContentRowDefinition.Height = new GridLength(1, GridUnitType.Star);
        }

#endif

        //if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        //{
        //    ChatBoxFrame.TranslateTo(0, 0, 50);
        //}
    }

    private void HandleQueriesRibbonButtonCheckChange(RadioButton radioButton)
    {
        switch (radioButton.IsChecked)
        {
            case true:

                EnableRefreshMessageView();
                ShowMessageCollectionView();
                ShowChatBoxFrame();
                HidePromotionAreaStackLayout();
                //UnloadPromotionContentView();
                ComputeMessageChatContentRowDefinition();
                break;

            case false:

                break;

        }
    }

    private async void HandleFromUsRibbonButtonCheckChange(RadioButton radioButton)
    {
        switch (radioButton.IsChecked)
        {
            case true:

                DisableRefreshMessageView();
                LoadPromotionContentView();
                ComputePromotionChatContentRowDefinition();
                HideMessageCollectionView();
                HideChatBoxFrame();
                ShowPromotionAreaStackLayout();
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

    private void HideMessageCollectionView()
    {
        if (this.ChatAreaGrid != null)
        {
            this.ChatAreaGrid.IsVisible = false;
        }

        if (this.MessageRefreshView != null)
        {
            this.MessageRefreshView.IsVisible = false;
        }
    }

    private void ShowMessageCollectionView()
    {
        if (this.ChatAreaGrid != null)
        {
            this.ChatAreaGrid.IsVisible = true;
        }

        if (this.MessageRefreshView != null)
        {
            this.MessageRefreshView.IsVisible = true;
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

    private void HideChatBoxFrame()
    {
        if (this.ChatBoxFrame != null)
        {
            this.ChatBoxFrame.IsVisible = false;
        }
    }

    private void ShowChatBoxFrame()
    {
        if (this.ChatBoxFrame != null)
        {
            this.ChatBoxFrame.IsVisible = true;
        }
    }

    private void DisableRefreshMessageView()
    {
        if (this.MessageRefreshView != null)
        {
            this.MessageRefreshView.IsEnabled = false;
        }
    }

    private void EnableRefreshMessageView()
    {
        if (this.MessageRefreshView != null)
        {
            this.MessageRefreshView.IsEnabled = true;
        }
    }



    #endregion
}