using MauiApp1.Threads;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MauiApp1
{
    public partial class App : Application
    {

       // private readonly BackgroundWorker _worker;
        public App()
        {
            InitializeComponent();
            //MainPage = new MainPage();
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