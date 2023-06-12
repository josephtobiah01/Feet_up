using Microsoft.Maui.Dispatching;
using ParentMiddleWare.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp1.Areas.Exercise.Views;

public class PopupViewModel : INotifyPropertyChanged
{
    public string TimerString { get; set; }
    public int TimerValue{ get; set; }

    public DateTime StartTimestamp { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


public partial class SetPopup : CommunityToolkit.Maui.Views.Popup
{

    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup
    // Have timer on popup (set it to 100 seconds for testing, display in hh:mm:ss)
    // Timer counts down
    // Have Label-button (button styled as label) or something else that works - uer can click to dismiss
    // if timer runs out the popup closes auto.
    // Style popup, make it smaller with white background with the shadow rectangle 


    IDispatcherTimer _dispatcherTimer;



    public PopupViewModel viewmodel;
    TimeSpan computedTimespan;

    public SetPopup(int TimerValue)
	{
		InitializeComponent();
		this.CanBeDismissedByTappingOutsideOfPopup = false;
        viewmodel = new PopupViewModel();
        viewmodel.StartTimestamp = DateTime.Now;
        viewmodel.TimerValue = TimerValue;
      //  TimerValue

      InitializeTimer();
    }

    void OnOKButtonClicked(object? sender, EventArgs e) => Close();


    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await InitializeTimer();
    }

    private async Task InitializeTimer()
    {
        try
        {
            computedTimespan = new TimeSpan(0, 0, viewmodel.TimerValue);
            this.TrainisngSessionTimerLabel.Text = string.Format("{0:00}:{1:00}", computedTimespan.Minutes, computedTimespan.Seconds);

            _dispatcherTimer = Dispatcher.CreateTimer();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
            _dispatcherTimer.Tick += Timer_Tick;
            _dispatcherTimer.Start();

        }
        catch (Exception ex)
        {
       
        }
        finally
        {

        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        TimerTick();
    }

    private void TimerTick()
    {
        if(computedTimespan.TotalSeconds <= 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (_dispatcherTimer != null)
                    {
                        _dispatcherTimer.Tick -= Timer_Tick;
                        _dispatcherTimer.Stop();
                        _dispatcherTimer = null;
                    }
                    this.Close();
                }
                catch { }
                return;
            });
        }

        computedTimespan = computedTimespan.Subtract(new TimeSpan(0, 0, 1));
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.TrainisngSessionTimerLabel.Text = string.Format("{0:00}:{1:00}", computedTimespan.Minutes, computedTimespan.Seconds);
        });
    }
}