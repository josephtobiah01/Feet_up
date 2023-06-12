using CommunityToolkit.Maui;
using Maui.FixesAndWorkarounds;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sample;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp() => MauiApp
          .CreateBuilder()
              .UseMauiApp<App>()
               .UseShiny()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            // .UseShiny() // <-- add this line (this is important)
            //Initialize Barcode Scanner (ZXing.net maui)
            .UseBarcodeReader()
            //Barcode Scanner Bug Hack/Workaround
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
                //fonts here are set for .net maui native only, not on .razor stuff. fonts there are set through wwwroot css
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


        static MauiAppBuilder RegisterInfrastructure(this MauiAppBuilder builder)
        {
#if Android||IOS
            builder.Configuration.AddJsonPlatformBundle();
#if DEBUG
            //  builder.Logging.SetMinimumLevel(LogLevel.Trace);
            //  builder.Logging.AddDebug();
#endif
            var s = builder.Services;
            // TODO: Please make sure to add your proper connection string and hub name to appsettings.json or this will error on startup
            s.AddPushAzureNotificationHubs<MyPushDelegate>(
                "Endpoint=sb://airns1.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=SCUdmZyFysYJ8q8zmYfBIyzM2MP+Ocwiy2k3kAzEnjg=",
                "airnotificationhub"
            );
#endif
            return builder;
        }

        static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            var s = builder.Services;
            return builder;
        }
    }

    //public static class ServiceCollectionExtensions
    //{
    //    public static IServiceCollection AddPushAzureNotificationHubs(this IServiceCollection services, string listenerConnectionString, string hubName)
    //    {
    //        services.AddPush();
    //        services.AddSingleton(new AzureNotificationConfig(listenerConnectionString, hubName));
    //        services.AddShinyService<AzureNotificationHubsPushProvider>();
    //        return services;
    //    }


    //    public static IServiceCollection AddPushAzureNotificationHubs<TPushDelegate>(this IServiceCollection services, string listenerConnectionString, string hubName)
    //        where TPushDelegate : class, IPushDelegate
    //    {
    //        services.AddSingleton<IPushDelegate, TPushDelegate>();
    //        services.AddPushAzureNotificationHubs(listenerConnectionString, hubName);
    //        return services;
    //    }
    // }
}