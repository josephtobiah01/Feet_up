﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Firebase;
using Plugin.FirebasePushNotification;

namespace MauiApp1
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {


        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();



        public override void OnCreate()
        {
            //try
            //{
               base.OnCreate();

            //    //   Task<bool> task = Task.Run<bool>(async () => await MauiApp1.Pages.Index.SetupUser());
            //    // var serviceResult = task.Result;

            //    // await MauiApp1.Pages.Index.SetupUser();

            //    //Set the default notification channel for your app when running Android Oreo  
            //    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            //    {
            //        //Change for your default notification channel id here  
            //        FirebasePushNotificationManager.DefaultNotificationChannelId = $"General";

            //        ////Change for your default notification channel name here  
            //        FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            //        FirebasePushNotificationManager.DefaultNotificationChannelImportance = NotificationImportance.High;

            //        //FirebasePushNotificationManager.IconResource = Resource.Drawable.pushIcon;

            //        //Sets the sound  uri will be used for the notification
            //        //FirebasePushNotificationManager.Android.Net.Uri SoundUri { get; set; }

            //        //Sets the color will be used for the notification
            //        //  FirebasePushNotificationManager.Color = new Android.Graphics.Color(240, 8, 7);

            //        //Sets the default notification channel importance for Android O
            //        // FirebasePushNotificationManager.DefaultNotificationChannelImportance = NotificationImportance.Max;

            //        // FirebasePushNotificationManager.ShouldShowWhen = true;
            //        // FirebasePushNotificationManager.UseBigTextStyle = true;
            //        //   FirebasePushNotificationManager.LargeIconResource = Resource.Drawable.air_icon_90x32;
            //        //  FirebasePushNotificationManager.NotificationActivityFlags = ActivityFlags.

            //        FirebasePushNotificationManager.NotificationActivityFlags = ActivityFlags.SingleTop;

            //    }

            //    //If debug you should reset the token each time.  

            //    string AppId = "1:368008538066:android:b25aacfaa968fe44130a0b";
            //    string SenderId = "368008538066";
            //    string ProjectId = "age-in-reverse-longevity";
            //    string ApiKey = "AIzaSyAkP7jzgygDHerCG7PVU5Lo3l-nhbLYrLk";


            //    var options = new FirebaseOptions.Builder()
            //         .SetApplicationId(AppId)
            //         .SetProjectId(ProjectId)
            //         .SetApiKey(ApiKey)
            //         .SetGcmSenderId(SenderId)
            //    .Build();


            //    MyPushNotificationHandler myHandler = new MyPushNotificationHandler();

            //    CrossFirebasePushNotification.Android.NotificationHandler = myHandler;
            //    //var channelId = $"General";
            //    //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            //    //var channel = new NotificationChannel(channelId, "General", NotificationImportance.High);
            //    //channel.EnableVibration(true);
            //    //notificationManager.CreateNotificationChannel(channel);


            //    FirebaseApp.InitializeApp(Android.App.Application.Context, options);
            //    //FirebaseMessaging.Instance.AutoInitEnabled = true; //.SetAutoInitEnabled(true);
            //    //  FirebaseMessaging.Instance.SubscribeToTopic("subTopic");


            //    FirebasePushNotificationManager.Initialize(this, false);
            //    // FirebasePushNotificationManager.Initialize(this, false, true, false);
            //}
            //catch { }
            //}

        }
    }
}