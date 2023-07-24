using FeedApi.Net7.Models;
using FitnessData.Client.Business;
using FitnessData.Common;
using MauiApp1.Areas.Dashboard.ViewModel;
using System.Collections.ObjectModel;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class ExerciseFeedListContentView : ContentView
{

    #region [Fields]

    private List<FeedItem> _feedItems;
    private ObservableCollection<ExerciseFeedItemDashboardViewModel> _exerciseFeedItemDashboardViewModel;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ExerciseFeedListContentView()
	{
		InitializeComponent();
	}

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        InitializeData();
        InitializeControl();
    }

    private async void InitializeData()
    {
        _exerciseFeedItemDashboardViewModel = new ObservableCollection<ExerciseFeedItemDashboardViewModel>();

        BindableLayout.SetItemsSource(this.ExerciseFeedItemList, _exerciseFeedItemDashboardViewModel);

        RefreshData();
    }

    private void InitializeControl()
    {

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    #endregion

    #region [Methods :: Tasks]

    private async Task<List<FeedItem>> GetFeedItems()
    {
        List<FeedItem> traniningSessionFeedItem = new List<FeedItem>();
        try
        {
            List<FeedItem> feedItems = await FeedApi.Net7.FeedApi.GetDailyFeedAsync(SelectedDateTime);

            foreach(FeedItem feedItem in feedItems)
            {
                if(feedItem.ItemType == FeedItemType.TrainingSessionFeedItem)
                {
                    if (feedItem.TrainingSessionFeedItem != null)
                    {
                        traniningSessionFeedItem.Add(feedItem);
                    }
                }                
            }

            return traniningSessionFeedItem;
        }
        catch
        {
            await Application.Current.MainPage.DisplayAlert("Retrieve Feed Item", "An error occurred while retrieving feed items. Please check internet connection and try again", "OK");
            return null;
        }
        finally
        {
        }
    }

    private ExerciseFeedItemDashboardViewModel GetExerciseFeedItemDashboardViewModelFromFeedItem(FeedItem feedItem)
    {
        TimeSpan workoutTimeElapsed = TimeSpan.MinValue;
        //int totalDaysToHours = 0;
        int hours = 0;
        int minutes = 0;
        int seconds = 0;

        ExerciseFeedItemDashboardViewModel exerciseFeedItemDashboardViewModel = new ExerciseFeedItemDashboardViewModel();

        exerciseFeedItemDashboardViewModel.Title = feedItem.Title;
        exerciseFeedItemDashboardViewModel.Status = feedItem.Status.ToString();

        if (feedItem.TrainingSessionFeedItem.TraningSession.StartTimestamp.HasValue == true && feedItem.TrainingSessionFeedItem.TraningSession.EndTimeStamp.HasValue == true)
        {
            workoutTimeElapsed = feedItem.TrainingSessionFeedItem.TraningSession.EndTimeStamp.Value - feedItem.TrainingSessionFeedItem.TraningSession.StartTimestamp.Value;
            //totalDaysToHours = workoutTimeElapsed.Days * 24;

            //hours = (workoutTimeElapsed.Hours + totalDaysToHours);
            hours = workoutTimeElapsed.Hours;
            minutes = workoutTimeElapsed.Minutes;
            seconds = workoutTimeElapsed.Seconds;
        }

        //exerciseFeedItemDashboardViewModel.TimeCompleted = (workoutTimeElapsed.Hours + totalDaysToHours) + " hours " + workoutTimeElapsed.Minutes + " Minutes " + workoutTimeElapsed.Seconds + " seconds";

        exerciseFeedItemDashboardViewModel.TimeCompleted = "Workout time: ";
        
        if (hours > 0)
        {
            exerciseFeedItemDashboardViewModel.TimeCompleted += hours + " hrs ";
        }

        if (minutes > 0)
        {
            exerciseFeedItemDashboardViewModel.TimeCompleted += minutes + " mins ";
        }

        if (seconds > 0)
        {
            exerciseFeedItemDashboardViewModel.TimeCompleted += seconds + " secs ";
        }

        return exerciseFeedItemDashboardViewModel;
    }
    #endregion

    #region [Methods :: Public Tasks] 

    public async void RefreshData()
    {
        try
        {
            if (_exerciseFeedItemDashboardViewModel != null)
            {
                _exerciseFeedItemDashboardViewModel.Clear();
            }

            _feedItems = await GetFeedItems();

            if (_feedItems != null)
            {
                ExerciseFeedItemDashboardViewModel exerciseFeedItemDashboardViewModel = null;
                await Task.Delay(10);
                foreach (FeedItem feedItem in _feedItems)
                {
                    await Task.Delay(1);
                    exerciseFeedItemDashboardViewModel = GetExerciseFeedItemDashboardViewModelFromFeedItem(feedItem);
                    _exerciseFeedItemDashboardViewModel.Add(exerciseFeedItemDashboardViewModel);
                }
            }
            
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Retreive Feed Item", "The system encountered a problem while retrieving training session feed item.", "OK");
        }
        finally
        {
            this.LoadingActivityIndicator.IsVisible = false;
        }
    }
    #endregion
        
    #region [Methods :: Properties]

    public DateTime SelectedDateTime;

    #endregion

}