using CommunityToolkit.Maui;
#if ANDROID
using Firebase;
#endif
using Maui.FixesAndWorkarounds;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ParentMiddleWare;
using Sample;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace MauiApp1
{
    public static class MauiProgram
    {
        private static string AppId = "1:368008538066:android:b25aacfaa968fe44130a0b";
        private static string SenderId = "368008538066";
        private static string ProjectId = "age-in-reverse-longevity";
        private static string ApiKey = "AIzaSyAkP7jzgygDHerCG7PVU5Lo3l-nhbLYrLk";

        public static MauiApp CreateMauiApp() => MauiApp
          .CreateBuilder()
              .UseMauiApp<App>()
              .UseShiny()
              .UseMauiCommunityToolkit()
              .UseMauiCommunityToolkitMediaElement()
              .UseBarcodeReader()
              .ConfigureMauiHandlers(h =>
              {
                  h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView), typeof(CameraBarcodeReaderViewHandler));
                  h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraView), typeof(CameraViewHandler));
                  h.AddHandler(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView), typeof(BarcodeGeneratorViewHandler));
              })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("montserrat-variablefont_wght.ttf", "Montserrat");
                fonts.AddFont("montserrat-variablefont_wght.ttf", "Montserrat-Italic");
                fonts.AddFont("montserrat-regular.ttf", "Montserrat-Regular");
                fonts.AddFont("montserrat-medium.ttf", "Montserrat-Medium");
                fonts.AddFont("montserrat-bold.ttf", "Montserrat-Bold");
                //fonts here are set for .net maui native only, not on.razor stuff.fonts there are set through wwwroot css
            })
            .RegisterInfrastructure()
            .RegisterAppServices()
            .RegisterViews()
            .Build();

        static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
        {
            builder.Services.AddMauiBlazorWebView();
#if IOS
            //for IOS Keyboard Issue
            builder.ConfigureKeyboardAutoScroll();
            builder.ConfigureShellWorkarounds();
            builder.ConfigureTabbedPageWorkarounds();
            builder.ConfigureEntryNextWorkaround();
            builder.ConfigureKeyboardAutoScroll();
            builder.ConfigureFlyoutPageWorkarounds();

#if ANDROID
builder.ConfigureEntryFocusOpensKeyboard();
#endif
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif


            return builder;
        }
        static  MauiAppBuilder RegisterInfrastructure(this MauiAppBuilder builder)
        {
            var s = builder.Services;
#if !WINDOWS
            builder.Configuration.AddJsonPlatformBundle();
#if DEBUG
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Logging.AddDebug();
#endif

            s.AddNotifications<Sample.Notifications.MyNotificationDelegate>();

#if DEBUG

#if ANDROID

            //  string AppId = "1:368008538066:android:b25aacfaa968fe44130a0b";
            //  string SenderId = "368008538066";
            //  string ProjectId = "age-in-reverse-longevity";
            // string ApiKey = "AIzaSyAkP7jzgygDHerCG7PVU5Lo3l-nhbLYrLk";


            //var options = new FirebaseOptions.Builder()
            //     .SetApplicationId(AppId)
            //     .SetProjectId(ProjectId)
            //     .SetApiKey(ApiKey)
            //     .SetGcmSenderId(SenderId)
            //     .Build();

            //        FirebaseApp.InitializeApp(Android.App.Application.Context, options);


            //        s.AddPush<MyPushDelegate>(new(
            //    true,
            //    AppId,
            //    SenderId,
            //    ProjectId,
            //    ApiKey
            //));


            s.AddPushAzureNotificationHubs<MyPushDelegate>(
                           "Endpoint=sb://airnotif.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nxTi2IJsFdJ6R6J9oZqX1HRounX8cwQ4O/VWoi0Zrio=",
                            "airnotificationhub"
                        );

            //            s.AddPushAzureNotificationHubs<MyPushDelegate>(
            //"Endpoint=sb://aitnotificationhubsandbox.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=znWm1i9w6J5CttUS5TIohh3iKKakb9DH68YATKDsWYU=",
            //   "aitnotificationhubsandbox");

#endif

#if IOS

            s.AddPushAzureNotificationHubs<MyPushDelegate>(
            "Endpoint=sb://aitnotificationhubsandbox.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=znWm1i9w6J5CttUS5TIohh3iKKakb9DH68YATKDsWYU=",
               "aitnotificationhubsandbox");
#endif

#endif
#if !DEBUG
            s.AddPushAzureNotificationHubs<MyPushDelegate>(
                           "Endpoint=sb://airnotif.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nxTi2IJsFdJ6R6J9oZqX1HRounX8cwQ4O/VWoi0Zrio=",
                            "airnotificationhub"
                        );

#endif
#endif


            return builder;
        }
        static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            var s = builder.Services;
            return builder;
        }
    }
}
