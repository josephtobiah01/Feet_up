using CommunityToolkit.Maui;
using InputKit.Handlers;
#if ANDROID
using Firebase;
#endif
using Maui.FixesAndWorkarounds;
using Sample;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using DevExpress.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.Extensions.Logging;
using MauiApp1.Business.DeviceServices;
using ParentMiddleWare;
using MauiApp1.Interfaces;
using MauiApp1.Services;
using MauiApp1.Platforms.Services;

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
              .InitiliazeConnectApplicationData()
#if IOS
              .UseShiny()
#endif
              .SetupServices()
              .UseDevExpress(useLocalization: true)
              .UseMauiCommunityToolkit()
              .UseMauiCommunityToolkitMediaElement()
              .UseBarcodeReader()

              .ConfigureMauiHandlers(h =>
              {
                  h.AddInputKitHandlers();
                  h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView), typeof(CameraBarcodeReaderViewHandler));
                  h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraView), typeof(CameraViewHandler));
                  h.AddHandler(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView), typeof(BarcodeGeneratorViewHandler));
              })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Montserrat-VariableFont_wght.ttf", "Montserrat");
                fonts.AddFont("Montserrat-Italic-VariableFont_wght.ttf", "Montserrat-Italic");
                fonts.AddFont("Montserrat-Regular.ttf", "Montserrat-Regular");
                fonts.AddFont("Montserrat-Medium.ttf", "Montserrat-Medium");
                fonts.AddFont("Montserrat-Bold.ttf", "Montserrat-Bold");
                //fonts here are set for .net maui native only, not on.razor stuff.fonts there are set through wwwroot css
            })
            .RegisterInfrastructure()
            .RegisterAppServices()
            .RegisterViews()
            .DevExpressInitialize()
            .Build();

        static MauiAppBuilder SetupServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ISelectedImageService, SelectedImageService>();

#if ANDROID || IOS
            //builder.Services.AddTransient<IAudioPlayerService, AudioPlayerService>();
            builder.Services.AddTransient<IRecordAudioService, RecordAudioService>();
#endif
            return builder;
        }
        static MauiAppBuilder DevExpressInitialize(this MauiAppBuilder builder)
        {
            DevExpress.Maui.Controls.Initializer.Init();
            DevExpress.Maui.Editors.Initializer.Init();
            return builder;
        }
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

            //   builder.Configuration.AddJsonPlatformBundle();
#if DEBUG
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Logging.AddDebug();
#endif





#if ANDROID
           // s.AddNotifications<Sample.Notifications.MyNotificationDelegate>();
            //s.AddPushAzureNotificationHubs<NewPushDelegate>(
            //               "Endpoint=sb://airnotif.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nxTi2IJsFdJ6R6J9oZqX1HRounX8cwQ4O/VWoi0Zrio=",
            //                "airnotificationhub"
            //            );

            //            s.AddPushAzureNotificationHubs<NewPushDelegate>(
            //"Endpoint=sb://aitnotificationhubsandbox.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=znWm1i9w6J5CttUS5TIohh3iKKakb9DH68YATKDsWYU=",
            //   "aitnotificationhubsandbox");

#endif

#if IOS

#if !DEBUG
            s.AddNotifications<Sample.Notifications.MyNotificationDelegate>();

            s.AddPushAzureNotificationHubs<NewPushDelegate>(
                           "Endpoint=sb://airnotif.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nxTi2IJsFdJ6R6J9oZqX1HRounX8cwQ4O/VWoi0Zrio=",
                            "airnotificationhub"
                             );

#elif DEBUG
            s.AddNotifications<Sample.Notifications.MyNotificationDelegate>();

            s.AddPushAzureNotificationHubs<NewPushDelegate>(
            "Endpoint=sb://aitnotificationhubsandbox.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=znWm1i9w6J5CttUS5TIohh3iKKakb9DH68YATKDsWYU=",
               "aitnotificationhubsandbox");
#endif

#endif





            return builder;
        }
        static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            var s = builder.Services;
#if IOS
            ScrollViewHandler.Mapper.AppendToMapping("ContentSize", (handler, view) =>
            {
                handler.PlatformView.UpdateContentSize(handler.VirtualView.ContentSize);
                handler.PlatformArrange(handler.PlatformView.Frame.ToRectangle());
            });
#endif
            return builder;
        }

        static MauiAppBuilder InitiliazeConnectApplicationData(this MauiAppBuilder builder)
        {
            Task.Run(() =>
            {
                MiddleWare.UseSecuredStorage = true;

                try
                {
                    DeviceStorageManager.InitializeGoogleFitData();
                    DeviceStorageManager.InitializeAppleHealthKitData();

                    ConnectedDevices.GetConnectedDevice(MiddleWare.UserID + "");

                    if (ConnectedDevices.isGoogleConnected == false)
                    {

                    }

                    if (ConnectedDevices.isAppleConnected == false)
                    {

                    }
                }
                catch(Exception ex)
                {

                }
                


                //GoogleFit googleFit = (GoogleFit)ConnectedDevices.GetConnectedDevice(
                //   ConnectedDevicesEnumeration.ConnectedDevice.Google_Fit, ConnectedDeviceDataStorageManager.googleFit);

                //AppleHealthKit appleHealthKit = (AppleHealthKit)ConnectedDevices.GetConnectedDevice(
                //    ConnectedDevicesEnumeration.ConnectedDevice.Apple_Health_Kit, ConnectedDeviceDataStorageManager.appleHealthKit);

                //if(googleFit == null)
                //{

                //}

                //if (appleHealthKit == null)
                //{

                //}
            });
           

            return builder;
        }
    }
}
