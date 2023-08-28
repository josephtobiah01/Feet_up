using CommunityToolkit.Mvvm.Input;
using MauiApp1.Areas.Chat.Models;
using MauiApp1.Areas.Chat.ViewModels.DeviceServices;
using MauiApp1.Helpers;
using MessageApi.Net7.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ParentMiddleWare;
using System.Text.RegularExpressions;

namespace MauiApp1.Areas.Chat.ViewModels
{
    public partial class ViewChatDetailPageVer2ViewModel : Bindablebase
    {
        #region[Fields]

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        private List<IMessage> _messageList;
        private List<ReceivedMessage> _messageListReserve;

        IDispatcherTimer _dispatcherTimer;

        private string _userMessageText = "";

        private bool _isUserScroll = false;
        private bool _isFirstLoad = true;

        private int _pageSize = 1;

        #endregion

        #region Properties
        
        public List<IMessage> MessageList
        {
            get => _messageList;
            set => _messageList = value;
           //set { SetPropertyValue(ref _messageList, value); }
        }

        #endregion Properties

        #region [Methods :: EventHandlers :: Class]

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    await base.OnAfterRenderAsync(firstRender);
        //    if (firstRender)
        //    {
        //        await ScrollDivToEnd();
        //    }
        //}

        //protected override async Task OnInitializedAsync()
        //{
        //    await IntializeData();
        //    InitializeControl();
        //}

        public ViewChatDetailPageVer2ViewModel()
        {
            //MessageList = new List<IMessage>();
            InitializeData();
            InitializeControl();
        }


        private async Task InitializeData()
        {
            
            try
            {
                IMessage message = null;
                _messageListReserve = await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.FkFederatedUser, DateTime.UtcNow.AddDays(-7));

                int count = _messageListReserve.Count;
                if (_messageListReserve.Count > 0)
                {
                    for (int index = 0; index < _messageListReserve.Count; index++)
                    {
                        ReceivedMessage recievedMessage = _messageListReserve.ElementAt(index);
                        message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, recievedMessage.MessageContent);
                        MessageList.Add(message);
                    }

                    _isUserScroll = false;
                    message = MessageList.ElementAt(MessageList.Count - 1);
                }

            }
            catch (Exception ex)
            {
                if (ChatHTMLBridge.StopTimerTick != null)
                {
                    ChatHTMLBridge.StopTimerTick.Invoke(this, null);
                }
                //await App.Current.MainPage.DisplayAlert("Retrieve Message", ex.Message, "OK");
                //await App.Current.MainPage.DisplayAlert("Retrieve Message", "An error occured while retrieving the messages." +
                //                " Please check the internet connection and try again.", "OK");
                ShowAlertBottomSheet("Retrieve Message", "An error occured while retrieving the messages." +
                                " Please check the internet connection and try again.", "OK");
            }
            finally
            {
                //StateHasChanged();

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

        public string Linkify(string input)
        {
            MatchCollection matches = Regex.Matches(input, "(?i)\\b((?:[a-z][\\w-]+:(?:/{1,3}|[a-z0-9%])|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))");

            foreach (Match match in matches)
            {
                input = input.Replace(match.Value, "<a href='" + match.Value + "'>" + match.Value + "</a>");
            }

            return input;
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        [RelayCommand]
        private void SendButton()
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
                List<ReceivedMessage> recievedMessages = await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.FkFederatedUser, DateTime.UtcNow.AddSeconds(-30));

                foreach (ReceivedMessage recievedMessage in recievedMessages)
                {
                    if (MessageList.Where(t => t.TimeStamp.Equals(recievedMessage.TimeStamp.ToLocalTime())).FirstOrDefault() != null)
                    {
                        continue;
                    }
                    else
                    {
                        if (recievedMessage.IsUserMessage == false)
                        {
                            message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, recievedMessage.MessageContent);

                            MessageList.Add(message);
                        }
                    }
                }

                if (message != null)
                {
                    //StateHasChanged();
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

                //await App.Current.MainPage.DisplayAlert("Retrieve Message", "An error occured while retrieving the messages." +
                //               " Please check the internet connection and try again.", "OK");
                ShowAlertBottomSheet("Retrieve Message", "An error occured while retrieving the messages." +
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
                    ReceivedMessage sentMessage = await MessageApi.Net7.MessageApi.SendMessage(new FrontendMessage() { FkFederatedUser = MiddleWare.FkFederatedUser, MessageContent = text });

                    if (sentMessage != null)
                    {
                        message = new IMessage(sentMessage.TimeStamp.ToLocalTime(), sentMessage.UserName, sentMessage.IsUserMessage, sentMessage.MessageContent);
                        MessageList.Add(message);
                        _isUserScroll = false;
                    }
                }
                this._userMessageText = null;

                //StateHasChanged();

                await ScrollDivToEnd();
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Send Message", ex.Message, "OK");

                //await App.Current.MainPage.DisplayAlert("Send Message", "An error occured while sending the messages." +
                //                " Please check the internet connection and try again.", "OK");
                ShowAlertBottomSheet("Send Message", "An error occured while sending the messages." +
                                " Please check the internet connection and try again.", "OK");
            }
            finally
            {

            }
        }

        [RelayCommand]

        private async void BackButton()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async Task ScrollDivToEnd()
        {
            await JSRuntime.InvokeVoidAsync("ScrollToEnd");
        }

        private async Task ScrollHTMLToEnd()
        {
            await JSRuntime.InvokeVoidAsync("ScrollHTMLToEnd");
        }

        private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
            }
        }

        #endregion

    }
}
