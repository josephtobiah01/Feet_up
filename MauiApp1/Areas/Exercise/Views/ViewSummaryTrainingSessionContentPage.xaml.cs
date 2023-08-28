using MauiApp1._Push;
using MauiApp1.Areas.Exercise.ViewModels;
using ParentMiddleWare.Models;
using System.Collections.ObjectModel;

namespace MauiApp1.Areas.Exercise.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ViewSummaryTrainingSessionContentPage : ContentPage
{

    #region [Fields]

    private long _TrainingSessionID = 0;

    public EmTrainingSession _emTrainingSession;

    private ObservableCollection<ExercisePageViewModel> _exerciseViewModels;

    private double _totalSetCompleted = 0;
    private double _totalSetUnitCompleted = 0;

    private const float VERY_EASY_RATING = 0.0f;
    private const float EASY_RATING = 0.25f;
    private const float AVERAGE = 0.5f;
    private const float HARD_RATING = 0.75f;
    private const float VERY_HARD_RATING = 1.0f;

    private const string VERY_EASY_TEXT = "Very Easy";
    private const string EASY_TEXT = "Easy";
    private const string AVERAGE_TEXT = "Average";
    private const string HARD_TEXT = "Hard";
    private const string VERY_HARD_TEXT = "Very Hard";

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewSummaryTrainingSessionContentPage(long trainingSessionID)
    {
        InitializeComponent();

        _TrainingSessionID = trainingSessionID;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await IntializeData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //InitializeConnectivityManager();
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        SaveFeedBack();
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        SaveFeedBack();
    }

    private void RatingSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        double sliderValue = e.NewValue;
        SetRatingSelectedValueChange(sliderValue);
    }

    #endregion

    #region [Methods :: Tasks]

    public async Task IntializeData()
    {
        _exerciseViewModels = new ObservableCollection<ExercisePageViewModel>();
        
        _totalSetCompleted = 0;

        try
        {
            _emTrainingSession = await ExerciseApi.Net7.ExerciseApi.GetTrainingSession(_TrainingSessionID);

            LoadExerciseViewModel();

            BindableLayout.SetItemsSource(this.ExerciseVerticalStackLayout, _exerciseViewModels);

            InitializeControls();
        }
        catch
        {
            //await DisplayAlert("Retrieve Training Session", "An error occured while retrieving the training session." +
            //    " Please check the internet connection and try again.", "OK"); 
            ShowAlertBottomSheet("Retrieve Training Session", "An error occured while retrieving the training session." +
                " Please check the internet connection and try again.", "OK");

        }
        finally
        {

        }
        
    }

    private void InitializeControls()
    {
        try
        {
            int hours = 0, minutes = 0, seconds = 0;
            TimeSpan computedTimespan;

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                this.FeedbackEditor.Placeholder = "Add comment here. Tell us how was " + Environment.NewLine + " your exercise.";
            }
            else
            {
                this.FeedbackEditor.Placeholder = "Add comment here. Tell us how was your exercise.";
            }

            if (_emTrainingSession.StartTimestamp.HasValue == true && _emTrainingSession.EndTimeStamp.HasValue == true)
            {
                computedTimespan = _emTrainingSession.EndTimeStamp.Value - _emTrainingSession.StartTimestamp.Value;
                hours = computedTimespan.Hours;
                minutes = computedTimespan.Minutes;
                seconds = computedTimespan.Seconds;
            }
            this.TrainisngSessionTimerLabel.Text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

            this.TotalSetLabel.Text = _totalSetCompleted.ToString();

            this.UnitOfMeasurementLabel.Text = string.Format("{0}{1}", _totalSetUnitCompleted,
                               "Kg");

            ComputeCollectionViewHeight();
            SetRatingSliderWidthRequest();
        }
        catch
        {
        }
        finally 
        { 
        }
    }

    private void SetRatingSliderWidthRequest()
    {
#if ANDROID
        this.RatingSlider.WidthRequest = 336;
#endif

#if WINDOWS
        this.RatingSlider.WidthRequest = 311;
#endif

#if IOS
        this.RatingSlider.WidthRequest = 320;
#endif

    }

    private void ComputeCollectionViewHeight()
    {
#if IOS
        double screenHeight = Application.Current.MainPage.Height;
        int computedHeight = (int)Math.Round(screenHeight * 0.60);

        if (this.ExerciseContentRowDefinition != null)
        {
            this.ExerciseContentRowDefinition.Height = computedHeight;
        }
#endif
    }

    private void LoadExerciseViewModel()
    {
        foreach (EmExercise emExercise in _emTrainingSession.emExercises)
        {
            LoadExerciseInCurrentList(emExercise);
        }
    }

    public void LoadExerciseInCurrentList(EmExercise emExercise)
    {
        ExercisePageViewModel exercisePageViewModel = null;
        exercisePageViewModel = new ExercisePageViewModel();
        exercisePageViewModel.SetviewModel = new ObservableCollection<SetPageViewModel>();
        exercisePageViewModel.Exercise_type_id = emExercise.FkExerciseTypeId;


        exercisePageViewModel.ExerciseName = emExercise.EmExerciseType.Name;
        exercisePageViewModel.IsExerciseSkipped = emExercise.IsSkipped;
        exercisePageViewModel.IsExerciseComplete = emExercise.IsComplete;
        exercisePageViewModel.ExerciseEndTimeStamp = emExercise.EndTimeStamp;
        exercisePageViewModel.IsCustomerAddedExercise = emExercise.IsCustomerAddedExercise;

        exercisePageViewModel.IsRecordExerciseTabContentVisible = true;
        exercisePageViewModel.IsHistoryTabContentVisible = false;
        exercisePageViewModel.IsSummaryTabContentVisible = false;
        exercisePageViewModel.ExerciseId = emExercise.Id;
        exercisePageViewModel.IsExerciseComplete = emExercise.IsComplete;
        exercisePageViewModel.IsCustomerAddedExercise = emExercise.IsCustomerAddedExercise;

        exercisePageViewModel.IsRecordExerciseTabContentVisible = true;
        exercisePageViewModel.IsHistoryTabContentVisible = false;
        exercisePageViewModel.IsSummaryTabContentVisible = false;

        exercisePageViewModel.MetricsName1 = "";
        exercisePageViewModel.MetricsName2 = "";

        foreach (EmSet emSet in emExercise.EmSet)
        {

            if (emSet.EmSetMetrics != null)
            {
                int i = 0;
                foreach (var item in emSet.EmSetMetrics)
                {
                    if (item.EmSetMetricTypes != null)
                    {
                        try
                        {
                            if (item.EmSetMetricTypes.Name.Trim() == "Rest")
                            {
                                continue;
                            }
                        }
                        catch { }
                        if (i == 0)
                        {
                            exercisePageViewModel.MetricsName1 = item.EmSetMetricTypes.Name;
                        }
                        else if (i == 1)
                        {
                            exercisePageViewModel.MetricsName2 = item.EmSetMetricTypes.Name;
                        }
                        i++;
                    }
                }
            }

            if (emSet.IsComplete == true)
            {
                LoadSetInCurrentList(exercisePageViewModel, emSet);
            }           
        }
        _exerciseViewModels.Add(exercisePageViewModel);

    }

    private void LoadSetInCurrentList(ExercisePageViewModel ExerciseModel, EmSet emSet)
    {
        SetPageViewModel setmodel = new SetPageViewModel();       

        setmodel.SetId = emSet.Id;
        setmodel.ExerciseId = emSet.ExerciseId;
        setmodel.Exercise_type_Id = ExerciseModel.Exercise_type_id;
        setmodel.TimeOffset = emSet.TimeOffset;

        if(emSet.SetSequenceNumber == 1)
        {
            setmodel.SetSequenceNumber = "W";
        }
        else
        {
            setmodel.SetSequenceNumber = emSet.SetSequenceNumber.ToString();
        }
       
        setmodel.IsComplete = emSet.IsComplete;
        setmodel.IsSkipped = emSet.IsSkipped;
        setmodel.IsCustomerAddedSet = emSet.IsCustomerAddedSet;
        setmodel.EndTimeStamp = emSet.EndTimeStamp;
        setmodel.TimeOffset = emSet.TimeOffset;
    //    setmodel.SetName = emSet.GetTextAsync();
        setmodel.IsComplete = emSet.IsComplete;
        setmodel.IsCustomerAddedSet = emSet.IsCustomerAddedSet;

        setmodel.SetRestTimeSecs = (int)emSet.GetRestTime();

        if (emSet.EmSetMetrics != null)
        {
            int i = 0;
            foreach (var item in emSet.EmSetMetrics)
            {
                try
                {
                    if (item.EmSetMetricTypes.Name.Trim() == "Rest")
                    {
                        continue;
                    }
                }
                catch { }
                if (i == 0)
                {
                    setmodel.MetricsValue1 = item.ActualCustomMetric.HasValue ? item.ActualCustomMetric.Value.ToString() : item.TargetCustomMetric.ToString();
                    setmodel.MetricsName1 = item.EmSetMetricTypes.Name;
                    setmodel.MetricId1 = item.Id;
                }
                else if (i == 1)
                {
                    setmodel.MetricsValue2 = item.ActualCustomMetric.HasValue ? item.ActualCustomMetric.Value.ToString() : item.TargetCustomMetric.ToString();
                    setmodel.MetricsName2 = item.EmSetMetricTypes.Name;
                    setmodel.MetricId2 = item.Id;
                }
                i++;
            }
        }

        if(emSet.IsComplete == true)
        {
            _totalSetCompleted = _totalSetCompleted + 1;

            if (setmodel.MetricsName1 == "kg" || setmodel.MetricsName2 == "kg")
            {
                if (setmodel.MetricsName2 == "Reps" || setmodel.MetricsName1 == "Reps")
                {
                    _totalSetUnitCompleted = _totalSetUnitCompleted + double.Parse(setmodel.MetricsValue1) * double.Parse(setmodel.MetricsValue2);
                }
                else
                {
                    _totalSetUnitCompleted = _totalSetUnitCompleted + double.Parse(setmodel.MetricsValue2);
                }
            }
        }

        ExerciseModel.SetviewModel.Add(setmodel);


    }

    private void SetRatingSelectedValueChange(double sliderValue)
    {

        if(sliderValue >= 0 && sliderValue < 0.22)
        {
            this.RatingLabel.Text = VERY_EASY_TEXT;
        }
        else if (sliderValue >= 0.22 && sliderValue < 0.48)
        {
            this.RatingLabel.Text = EASY_TEXT;
        }
        else if (sliderValue >= 0.48 && sliderValue < 0.74)
        {
            this.RatingLabel.Text = AVERAGE_TEXT;
        }
        else if (sliderValue >= 0.739 && sliderValue < 1)
        {
            this.RatingLabel.Text = HARD_TEXT;
        }
        else if (sliderValue == 1)
        {
            this.RatingLabel.Text = VERY_HARD_TEXT;
        }        
        else
        {
            //error
        }
    }


    private async void SaveFeedBack()
    {
        try
        {
            string ratingText = this.RatingLabel.Text;
            string feedback = this.FeedbackEditor.Text;
            float rating = 0;

            switch (ratingText)
            {
                case AVERAGE_TEXT:

                    rating = AVERAGE;
                    break;

                case VERY_EASY_TEXT:

                    rating = VERY_EASY_RATING;
                    break;

                case EASY_TEXT:

                    rating = EASY_RATING;
                    break;

                case HARD_TEXT:

                    rating = HARD_RATING;
                    break;

                case VERY_HARD_TEXT:

                    rating = VERY_HARD_RATING;
                    break;

                default:

                    throw new Exception("Selected rating was not in the list of ratings");
            }
            
            await ExerciseApi.Net7.ExerciseApi.SetFeedBack(_TrainingSessionID, rating, feedback);

            try
            {
                await Navigation.PopToRootAsync();
                //if (PushNavigationHelper.RootPage != null)
                //{
                //    await PushNavigationHelper.RootPage.RefreshPage();
                //}
            }
            catch { }
           
        }
        catch
        {
            //await DisplayAlert("Saving Feedback", "An error occured while saving feedback." +
            //    " Please check the internet connection and try again.", "OK");
            ShowAlertBottomSheet("Saving Feedback", "An error occured while saving feedback." +
                " Please check the internet connection and try again.", "OK");

        }
        finally 
        { 
        }

        
    }

    private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
    {
        if (App.alertBottomSheetManager != null)
        {
            App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
        }
    }

    #endregion
}