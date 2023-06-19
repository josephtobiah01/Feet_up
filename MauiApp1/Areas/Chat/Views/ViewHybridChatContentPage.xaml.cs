using MauiApp1.Areas.Chat.Models;
using ParentMiddleWare;
using System.Collections.ObjectModel;
using MessageApi.Net7;
using Microsoft.Maui.Dispatching;
using MauiApp1.Areas.Chat.ViewModels.DeviceServices;

namespace MauiApp1.Areas.Chat.Views;

public partial class ViewHybridChatContentPage : ContentPage
{
    #region[Fields]

    IDispatcherTimer _dispatcherTimer;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewHybridChatContentPage()
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
    }

    public async void InitializeData()
    {
        await MauiApp1.Pages.Index.SetupUser();
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

    private void UnloadControl()
    {
        DisposeControls();
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