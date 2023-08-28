using DevExpress.Maui.Core.Internal;
using MauiApp1.Areas.Chat.Models;
using MauiApp1.Areas.Chat.ViewModels.DeviceServices;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Interfaces;
using MauiApp1.Models;
using MessageApi.Net7.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ParentMiddleWare;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace MauiApp1.Pages.Chat
{
    public partial class ViewChatDetailPage : INotifyPropertyChanged
    {

        #region[Fields]

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        ISelectedImageService selectedImageService { get; set; }
        IDispatcherTimer _dispatcherTimer;



        private ObservableCollection<IMessage> _messageList = new ObservableCollection<IMessage>();
        private List<ReceivedMessage> _messageListReserve;
        private List<string> _galleryImages = new List<string>();
       

        private string _selectedImage;
        private string _userMessageText = "";
        public static string _userMessageImage = "";
        private bool _isCancelButtonVisible = false;
        private double _previousScrollTop = 0;
        private double _scrollPosition; 



        private bool _isUserScroll = false;
        private bool _isFirstLoad = true;

        private int _pageSize = 1;

        [Parameter]
        public EventCallback OpenGalleryAndBottomSheet { get; set; }

        //public delegate void MyMethodDelegate();

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

        public double PreviousScrollTop
        {
            get { return _previousScrollTop; }
            set
            {
                _previousScrollTop = value;
                OnPropertyChanged(nameof(PreviousScrollTop));
            }
        }

        public bool IsFirstLoad
        {
            get { return _isFirstLoad; }
            set
            {
                _isFirstLoad = value;
                OnPropertyChanged(nameof(IsFirstLoad));
            }
        }

        public bool IsUserScroll
        {
            get { return _isUserScroll; }
            set
            {
                _isUserScroll = value;
                OnPropertyChanged(nameof(IsUserScroll));
            }
        }

        public double ScrollPosition
        {
            get { return _scrollPosition; }
            set
            {
                _scrollPosition = value;
                OnPropertyChanged(nameof(ScrollPosition));
            }
        }

        #endregion

        #region [Methods :: EventHandlers :: Class]

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                try
                {
                    await Task.Delay(70);
                    await ScrollDivToEnd();
                }
                catch(Exception ex)
                {
                    ShowAlertBottomSheet("Error", $"{ex.Message}", "Ok");
                }
                
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await IntializeData();
            InitializeControl();

            await ScrollDivToEnd();

            selectedImageService.SelectedChatContentImageChanged += SelectedImageService_OnSelectedChatContentImageChanged;

            MessagingCenter.Subscribe<ViewHybridChatContentPage>(this, "SendMessageTrigger", TriggerMethod);

        }

    
        private async Task HandleScrollEvent()
        {
            try
            {
                bool isScrolledTop = await JSRuntime.InvokeAsync<bool>("GetIsScrolledTop");
                if (isScrolledTop == true)
                {
                    await LoadPreviousMessages();
                }
            }
            catch(Exception ex)
            {
                ShowAlertBottomSheet("Error", $"{ex.Message}", "Ok");
            }
            
            
        }

        private async Task ScrollDivToPosition(double position)
        {
            await JSRuntime.InvokeVoidAsync("SetScrollPosition", position);
        }

        private async Task LoadPreviousMessages()
        {
            try
            {
                ScrollPosition = await JSRuntime.InvokeAsync<double>("GetScrollPosition");

                IMessage message = null;

                DateTime oldestMessageDate = _messageList.First().TimeStamp.Date.AddDays(-7);

                DateTime lastMessageDate = _messageList.First().TimeStamp.Date;

                _messageListReserve = await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.FkFederatedUser, oldestMessageDate);
                

                if (_messageListReserve.Count > 0)
                {
                    foreach (ReceivedMessage receivedMessage in _messageListReserve)
                    {
                        if (receivedMessage.TimeStamp >= oldestMessageDate && receivedMessage.TimeStamp <= lastMessageDate)
                        {
                            message = new IMessage(
                            receivedMessage.TimeStamp.ToLocalTime(),
                            receivedMessage.UserName,
                            receivedMessage.IsUserMessage,
                            Linkify(receivedMessage.MessageContent),
                            receivedMessage.ThumbnailImageUrl);

                            _messageList.Insert(0, message);
                        }
                        
                    }
                    await JSRuntime.InvokeVoidAsync("SetScrollPosition", ScrollPosition);

                }
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Retrieve Message", "An error occurred while retrieving the messages." +
                                    " Please check the internet connection and try again.", "OK");
            }
            finally
            {
                StateHasChanged();
            }
        }

        private async void TriggerMethod(ViewHybridChatContentPage sender)
        {
            await SendMessage(image: sender.SelectedImage);
        }
    
        private void SelectedImageService_OnSelectedChatContentImageChanged(object sender, string image)
        {
            _userMessageImage = image.ToString();
        }

        private async Task IntializeData()
        {
            try
            {
                IMessage message = null;
                _messageListReserve = await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.FkFederatedUser, DateTime.UtcNow.AddDays(-7));

                //int count = _messageListReserve.Count;
                if (_messageListReserve.Count > 0)
                {
                    for (int index = 0; index < _messageListReserve.Count; index++)
                    {
                        ReceivedMessage recievedMessage = _messageListReserve.ElementAt(index);
                        
                        message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, Linkify(recievedMessage.MessageContent), recievedMessage.ThumbnailImageUrl);
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
                ShowAlertBottomSheet("Retrieve Message", "An error occured while retrieving the messages." +
                                " Please check the internet connection and try again.", "OK");
            }
            finally
            {
                StateHasChanged();
            }
        }



        private async void InitializeControl()
        {
            if (ChatHTMLBridge.RefreshChatData != null)
            {
                ChatHTMLBridge.RefreshChatData -= RefreshData_OnRefresh;
            }

            ChatHTMLBridge.RefreshChatData += RefreshData_OnRefresh;
            await ScrollDivToEnd();
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private async Task SendButton_Clicked()
        {
            await SendMessage(this._userMessageText, _userMessageImage);
        }

        private void SendImage_Clicked()
        {

        }

        private async void ChatInput_Focused()
        {
            await ScrollHTMLToEnd();
        }

        protected virtual async void RefreshData_OnRefresh(object sender, EventArgs e)
        {
            await RefreshPage();
        }

        public string Linkify(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            MatchCollection matches = Regex.Matches(input, "(?i)\\b((?:[a-z][\\w-]+:(?:/{1,3}|[a-z0-9%])|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))");

            foreach (Match match in matches)
            {
                input = input.Replace(match.Value, "<a href='" + match.Value + "'>" + match.Value + "</a>");
            }

            return input;
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
                    if (_messageList.Where(t => t.TimeStamp.Equals(recievedMessage.TimeStamp.ToLocalTime())).FirstOrDefault() != null)
                    {
                        continue;
                    }
                    else
                    {
                        if (recievedMessage.IsUserMessage == false)
                        {
                            message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, Linkify(recievedMessage.MessageContent), recievedMessage.ThumbnailImageUrl);
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

                ShowAlertBottomSheet("Retrieve Message", "An error occured while retrieving the messages." +
                               " Please check the internet connection and try again.", "OK");
            }
            finally
            {
                
            }
        }


        public async Task SendMessage(string text = "", string image = "")
        {
            try
            {          
                if (string.IsNullOrWhiteSpace(text) == false || string.IsNullOrWhiteSpace(image) == false)
                {  
                    IMessage message = null;
                    _userMessageImage = image;
                    ReceivedMessage sentMessage = await MessageApi.Net7.MessageApi.SendMessage(new FrontendMessage() 
                        { FkFederatedUser = MiddleWare.FkFederatedUser, MessageContent = text, ImageFileContent = image, ImageFileContentType = "image/png" });

                    //MessageReceivedResponse sentMessage = await GetData(text, image);

                    if (sentMessage != null)
                    {
                        message = new IMessage(sentMessage.TimeStamp.ToLocalTime(), sentMessage.UserName, 
                            sentMessage.IsUserMessage, Linkify(sentMessage.MessageContent), sentMessage.ThumbnailImageUrl);
                        _messageList.Add(message);
                        _isUserScroll = false;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Sorry", "You cannot send an empty message", "Ok");
                }
              
                this._userMessageText = null;
                _userMessageImage = null;
                this._isCancelButtonVisible = false;

                await JSRuntime.InvokeVoidAsync("ResetInputHeight");

                if (JSRuntime == null)
                {
                    ShowAlertBottomSheet("JSRuntime Missing", "JSRuntime is not available.", "OK");
                    return;
                }

                StateHasChanged();

                await ScrollDivToEnd();
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Send Message", ex.Message, "OK");

                ShowAlertBottomSheet("Send Message", "An error occured while sending the messages." +
                                " Please check the internet connection and try again.", "OK");
            }
            finally
            {
            }
        }

        private async Task UploadRecording()
        {

        }
        
        private async Task UploadPhoto()
        {          
            await OpenGalleryAndBottomSheet.InvokeAsync();
        }

       

        private void CancelButtonClick()
        {
            _userMessageImage = "";
            _isCancelButtonVisible = false;
        }

        private string SetPlaceholder()
        {
            if (!string.IsNullOrEmpty(_userMessageImage))
            {
                return "";
            }
            else
            {
                return string.IsNullOrEmpty(_userMessageText) ? "Send a message" : "";
            }
        }


        private async Task<MessageReceivedResponse> GetData(string? text, string? image)
        {
            MessageReceivedResponse response = new MessageReceivedResponse();

            response.MessageContent = text;
            response.MessageImageContent = image;
            response.IsUserMessage = true;
            response.TimeStamp = DateTime.Now;
            response.UserName = "test";

            await Task.Delay(2);
            return response;
        }

        private async Task ScrollDivToEnd()
        {
            bool isScrollable = await JSRuntime.InvokeAsync<bool>("IsDivScrollable");

            if (isScrollable)
            {
                await JSRuntime.InvokeVoidAsync("ScrollToEnd");
            }
            else
            {
                await Task.Delay(100);
                await ScrollDivToEnd();
            }
            
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
