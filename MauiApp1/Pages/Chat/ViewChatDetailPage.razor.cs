using MauiApp1.Areas.Chat.Models;
using MauiApp1.Areas.Chat.ViewModels.DeviceServices;
using MessageApi.Net7;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ParentMiddleWare;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages.Chat
{
    public partial class ViewChatDetailPage
    {

        #region[Fields]

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        private ObservableCollection<IMessage> _messageList = new ObservableCollection<IMessage>();
        private List<RecievedMessage> _messageListReserve;

        IDispatcherTimer _dispatcherTimer;

        private string _userMessageText = "";

        private bool _isUserScroll = false;
        private bool _isFirstLoad = true;

        private int _pageSize = 1;

        #endregion

        #region [Methods :: EventHandlers :: Class]

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await ScrollDivToEnd();
            }           
        }

        protected override async Task OnInitializedAsync()
        {
            await IntializeData();
            InitializeControl();
        }
        

        private async Task IntializeData()
        {
            try
            {
                IMessage message = null;
                _messageListReserve = await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.UserID, DateTime.UtcNow.AddDays(-7));

                int count = _messageListReserve.Count;
                if (_messageListReserve.Count > 0)
                {
                    for (int index = 0; index < _messageListReserve.Count; index++)
                    {
                        RecievedMessage recievedMessage = _messageListReserve.ElementAt(index);
                        message = new IMessage(recievedMessage.MessageContent, recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage);
                        _messageList.Add(message);
                    }

                    _isUserScroll = false;
                    message = _messageList.ElementAt(_messageList.Count - 1);                   
                }

            }
            catch (Exception ex)
            {
                if (ChatHTMLBridge.StopTimerTick != null)
                {
                    ChatHTMLBridge.StopTimerTick.Invoke(this, null);
                }
                //await App.Current.MainPage.DisplayAlert("Retrieve Message", ex.Message, "OK");
                await App.Current.MainPage.DisplayAlert("Retrieve Message", "An error occured while retrieving the messages." +
                                " Please check the internet connection and try again.", "OK");
            }
            finally
            {
                StateHasChanged();

                await ScrollDivToEnd();
            }
        }

        private void InitializeControl()
        {
            if (ChatHTMLBridge.RefreshChatData != null)
            {
                ChatHTMLBridge.RefreshChatData -= RefreshData_OnRefresh;
            }

            ChatHTMLBridge.RefreshChatData += RefreshData_OnRefresh;
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private void SendButton_Clicked()
        {
            SendMessage(this._userMessageText);
        }

        private async void ChatInput_Focused()
        {
            await ScrollHTMLToEnd();
        }

        protected virtual async void RefreshData_OnRefresh(object sender, EventArgs e)
        {
            await RefreshPage();
        }

        #endregion

        #region [Methods :: Tasks]

        private async Task RefreshPage()
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
                            message = new IMessage(recievedMessage.MessageContent, recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage);

                            _messageList.Add(message);
                        }
                    }
                }

                if (message != null)
                {
                    StateHasChanged();
                    await ScrollDivToEnd();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {

                if (ChatHTMLBridge.StopTimerTick != null)
                {
                    ChatHTMLBridge.StopTimerTick.Invoke(this, null);
                }

                await App.Current.MainPage.DisplayAlert("Retrieve Message", "An error occured while retrieving the messages." +
                               " Please check the internet connection and try again.", "OK");
            }
            finally
            {
                
            }
        }

        public async void SendMessage(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    IMessage message = null;
                    RecievedMessage sentMessage = await MessageApi.Net7.MessageApi.SendMessage(new FrontendMessage() { Fk_Sender_Id = MiddleWare.UserID, MessageContent = text });

                    if (sentMessage != null)
                    {
                        message = new IMessage(sentMessage.MessageContent, sentMessage.TimeStamp.ToLocalTime(), sentMessage.UserName, sentMessage.IsUserMessage);
                        _messageList.Add(message);
                        _isUserScroll = false;
                    }
                }
                this._userMessageText = null;
                
                StateHasChanged();

                await ScrollDivToEnd();
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Send Message", ex.Message, "OK");

                await App.Current.MainPage.DisplayAlert("Send Message", "An error occured while sending the messages." +
                                " Please check the internet connection and try again.", "OK");
            }
            finally
            {

            }
        }

        private async Task ScrollDivToEnd()
        {
            await JSRuntime.InvokeVoidAsync("ScrollToEnd");
        }

        private async Task ScrollHTMLToEnd()
        {
            await JSRuntime.InvokeVoidAsync("ScrollHTMLToEnd");
        }

        #endregion

    }
}
