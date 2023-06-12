
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView.Maui;
using ParentMiddleWare.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MessageApi.Net7;
using ParentMiddleWare;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MauiApp1.Areas.Chat.Models;

namespace MauiApp1.Areas.Chat.Views
{
    public partial class ChatContentPage: ContentPage
    {
        #region[Fields]

        public ObservableCollection<IMessage> MessageList = new ObservableCollection<IMessage>();
        public Command SendChatMessageReturnCommand;
        
        IDispatcherTimer _dispatcherTimer;

        private bool _isUserScroll = false;
        #endregion

        #region [Methods :: EventHandlers :: Class]

        public ChatContentPage()
        {
            InitializeComponent();
        }

        protected async void ContentPage_Loaded(object sender, EventArgs e)
        {
            SendChatMessageReturnCommand = new Command(execute: () =>
            {
                ChatEntryReturn();
            });
            await InitializeData();
            InitializeControl();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            DisposeControls();
        }

        public async Task InitializeData()
        {
            //   ChatMessages.ItemsSource = MessageList;
            BindableLayout.SetItemsSource(ChatMessages, MessageList);
            BindableLayout.SetItemTemplateSelector(ChatMessages, MessageDataTemplateSelectorname);



            foreach (var msg in await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.UserID, DateTime.UtcNow.AddDays(-7)))
            {
                MessageList.Add(new IMessage(msg.MessageContent, msg.TimeStamp.ToLocalTime(), msg.UserName, msg.IsUserMessage));
            }


            //_dispatcherTimer = Dispatcher.CreateTimer();
            //_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
            //_dispatcherTimer.Tick += Timer_Tick;
            //_dispatcherTimer.Start();
            ////await Body.ScrollToAsync(0, MessagesGrid.Height, true);
            //await ChatScrollView.ScrollToAsync(ChatMessages, ScrollToPosition.End, true);
        }

        private void InitializeControl()
        {

            _dispatcherTimer = Dispatcher.CreateTimer();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(3000);
            _dispatcherTimer.Tick += Timer_Tick;
            _dispatcherTimer.Start();
            //await Body.ScrollToAsync(0, MessagesGrid.Height, true);
            //await ChatScrollView.ScrollToAsync(ChatMessages, ScrollToPosition.End, true);
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimerTick();
        }

        private void sendButton_Clicked(object sender, EventArgs e)
        {
            SendMessage(this.myChatMessage.Text);
            //myChatMessage.Text = null;
            //await ChatScrollView.ScrollToAsync(ChatMessages, ScrollToPosition.End, true);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }

        private void ChatScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            ScrollView scrollView = sender as ScrollView;
            double scrollingSpace = scrollView.ContentSize.Height - scrollView.Height;

            CheckScrollToEnd(scrollView, scrollingSpace, e.ScrollY);
        }

        #endregion

        #region [Methods :: Tasks]

        private void ChatEntryReturn()
        {
            SendMessage(this.myChatMessage.Text);
        }
        public async void SendMessage(String Text)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                var sentMessage = await MessageApi.Net7.MessageApi.SendMessage(new FrontendMessage() { Fk_Sender_Id = MiddleWare.UserID, MessageContent = Text });
                if (sentMessage != null)
                {
                    MessageList.Add(new IMessage(sentMessage.MessageContent, sentMessage.TimeStamp.ToLocalTime(), sentMessage.UserName, sentMessage.IsUserMessage));
                    _isUserScroll = false;
                }
                myChatMessage.Text = null;
            }
            //await ChatScrollView.ScrollToAsync(ChatMessages, ScrollToPosition.End, true);
        }

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

        }

        private async void TimerTick()
        {
            //MessageList = new ObservableCollection<IMessage>();
            foreach (var msg in await MessageApi.Net7.MessageApi.GetMessages(MiddleWare.UserID, DateTime.UtcNow.AddSeconds(-30)))
            {
                if (MessageList.Where(t => t.TimeStamp.Equals(msg.TimeStamp.ToLocalTime())).FirstOrDefault() != null)
                {
                    continue;
                }
                else
                {
                    if (!msg.IsUserMessage)
                    {
                        MessageList.Add(new IMessage(msg.MessageContent, msg.TimeStamp.ToLocalTime(), msg.UserName, msg.IsUserMessage));
                        //await ChatScrollView.ScrollToAsync(ChatMessages, ScrollToPosition.End, true);
                        
                    }
                }
            }

            if (_isUserScroll == false)
            {
                await ChatScrollView.ScrollToAsync(ChatMessages, ScrollToPosition.End, true);
                _isUserScroll = false;
            }

            


            //ChatMessages.ItemsSource = MessageList;
            //await ChatScrollView.ScrollToAsync(ChatMessages, ScrollToPosition.End, true);
        }

        private void CheckScrollToEnd(ScrollView scrollView, double scrollingSpace, double scrollY)
        {
            if (scrollingSpace <= scrollY)
            {
                //reached end
                _isUserScroll = false;
            }
            else
            {
                _isUserScroll = true;
            }
        }

        #endregion

    }

}