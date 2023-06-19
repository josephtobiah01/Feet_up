using Plugin.FirebasePushNotification;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MauiApp1
{
    public partial class App : Application
    {

        // private readonly BackgroundWorker _worker;
        public App()
        {
            InitializeComponent();

            Task<bool> task = Task.Run<bool>(async () => await MauiApp1.Pages.Index.SetupUser());
            var serviceResult = task.Result;
            if (serviceResult == true) { }
            //MainPage = new MainPage();

            //Task<bool> task = Task.Run<bool>(async () => await MauiApp1.Pages.Index.SetupUser());
            //var serviceResult = task.Result;

#if ANDROID

            //Change for your default notification channel id here
            FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

            //Change for your default notification channel name here
            FirebasePushNotificationManager.DefaultNotificationChannelName = "General";


            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
            };
            // Push message received event  
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {

                System.Diagnostics.Debug.WriteLine("Received");

            };
            //Push message received event  
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }

            };
#endif

            MainPage = new NavigationPage(new MainPage());
        }


        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            try
            {
            }
            catch (Exception ex)
            {
                //Log error
                window.Page.DisplayAlert("Application StartUp Error", "An error occured while trying to login on start." + ex.Message + ex.StackTrace, "OK");

            }
            finally
            {

            }

            return window;
        }

    }
}