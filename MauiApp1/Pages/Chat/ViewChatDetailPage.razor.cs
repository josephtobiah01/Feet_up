using MauiApp1.Areas.Chat.Models;
using MauiApp1.Areas.Chat.ViewModels.DeviceServices;
using MauiApp1.Helpers;
using MauiApp1.Models;
using MessageApi.Net7;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ParentMiddleWare;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Image = Microsoft.Maui.Controls.Image;

namespace MauiApp1.Pages.Chat
{
    public partial class ViewChatDetailPage
    {

        #region[Fields]

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        

        private ObservableCollection<IMessage> _messageList = new ObservableCollection<IMessage>();
        private List<RecievedMessage> _messageListReserve;
        private List<string> _galleryImages = new List<string>();
        

        IDispatcherTimer _dispatcherTimer;

        private string _userMessageText = "";
        private string _userMessageImage = "";
        private bool _isCancelButtonVisible = false;
        

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
                await JSRuntime.InvokeVoidAsync("StartUp");
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
                        
                        message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, Linkify(recievedMessage.MessageContent));
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
            SendMessage(this._userMessageText, this._userMessageImage);
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
                            message = new IMessage(recievedMessage.TimeStamp.ToLocalTime(), recievedMessage.UserName, recievedMessage.IsUserMessage, Linkify(recievedMessage.MessageContent));

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

        public async void SendMessage(string text, string image)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text) == false && _userMessageImage != null)
                {
                    IMessage message = null;
                    //RecievedMessage sentMessage = await MessageApi.Net7.MessageApi.SendMessage(
                    //    new FrontendMessage() { Fk_Sender_Id = MiddleWare.UserID, MessageContent = text});

                    MessageReceivedResponse sentMessage = await GetData(text, image);

                    if (sentMessage != null)
                    {
                        message = new IMessage(sentMessage.TimeStamp.ToLocalTime(), sentMessage.UserName, 
                            sentMessage.IsUserMessage, Linkify(sentMessage.MessageContent), sentMessage.MessageImageContent);
                        _messageList.Add(message);
                        _isUserScroll = false;
                    }
                }
              
                this._userMessageText = null;
                this._userMessageImage = null;
                this._isCancelButtonVisible = false;

                await JSRuntime.InvokeVoidAsync("ResetInputHeight");
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

        private async Task UploadPhoto()
        {
            string uploadImage = "";
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Media>();
            if (status != PermissionStatus.Denied || status != PermissionStatus.Unknown || status != PermissionStatus.Disabled)
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select image(s) to send",
                    FileTypes = FilePickerFileType.Images
                });

                if (result == null)
                    return;

                var stream = await result.OpenReadAsync();

                uploadImage = await ConvertStreamToBase64Async(stream);

                _userMessageImage += $"data:image/png;base64,{uploadImage}";
                _isCancelButtonVisible = true;

            }

            else
            {
                var result = await Permissions.RequestAsync<Permissions.Media>();

                if (result == PermissionStatus.Denied || result == PermissionStatus.Unknown || result != PermissionStatus.Disabled)
                {
                    await App.Current.MainPage.DisplayAlert("Required", "You must allow the permission to get your image", "Ok");
                }
            }
        }

        private async Task TakePhoto()
        {
            string takeImage = "";
            var photo = await MediaPicker.CapturePhotoAsync();

            var stream = await photo.OpenReadAsync();

            takeImage = await ConvertStreamToBase64Async(stream);

            _userMessageImage = $"data:image/png;base64,{takeImage}";

        }

        private async Task<string> ConvertStreamToBase64Async(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
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


        private async Task<MessageReceivedResponse> GetData(string text, string image)
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
            await JSRuntime.InvokeVoidAsync("ScrollToEnd");
        }

        private async Task ScrollHTMLToEnd()
        {
            await JSRuntime.InvokeVoidAsync("ScrollHTMLToEnd");
        }

        #endregion

    }
}
