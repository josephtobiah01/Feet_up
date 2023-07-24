using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using MauiApp1.Areas.Exercise.Resources;
using MauiApp1.Areas.Exercise.ViewModels;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows.Input;

namespace MauiApp1.Areas.Exercise.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ViewExerciseContentPage : ContentPage
{


    #region [Fields]

    public static ObservableCollection<ExercisePageViewModel> _exerciseViewModels;
    public EmTrainingSession _emTrainingSession;
    public SetPageViewModel _setPageViewModel;

    private static double _totalSetUnitCompleted = 0;
    private static double _totalSetCompleted = 0;
    private static string _unitInMeasurement = string.Empty;

    private static double _totalSet = 0;


    IDispatcherTimer _dispatcherTimer;
    int _hours = 0;
    int _minutes = 0;
    int _seconds = 0;

    ProgressArc _progressArc = new ProgressArc();
    
    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewExerciseContentPage(EmTrainingSession emTrainingSession)
    {

        _emTrainingSession = emTrainingSession;
        this.Loaded += (s, e) =>
        {
       //     _doApiCalls = false;
            BindableLayout.SetItemsSource(this.ExerciseListView, _exerciseViewModels);
            ComputeCollectionViewHeight();
          //  _doApiCalls = true;
        };
        InitializeComponent();

        _setPageViewModel = new SetPageViewModel();
    }

    

    public ViewExerciseContentPage()
    {
        InitializeComponent();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartTimer();

    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        StopTimer();
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await InitializeData();
        await InitializeDrawables();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        DisposeControls();
    }

    public static bool _doApiCalls = false;

    public async Task InitializeData()
    {
        _totalSet = 0;
        _totalSetUnitCompleted = 0;
        _totalSetCompleted = 0;
        _unitInMeasurement = string.Empty;

        this.UnitOfMeasurementLabel.Text = string.Format("{0}{1}", _totalSetUnitCompleted,
                                "Kg");
        await InitializeTimer();

       _setPageViewModel = new SetPageViewModel();

        
       
    }

    public async Task InitializeDrawables()
    {
        double percentage = (_totalSetCompleted / _totalSet) * 100;
        _progressArc.setPercentage(percentage);
        this.ProgressArcGraphicsView.Drawable = _progressArc;
        this.ProgressNeedleGraphicsView.Rotation = Math.Min(180,Math.Max((int)Math.Round(percentage * 1.8),0));
        this.ProgressArcGraphicsView.Invalidate();
    }


    private async Task InitializeTimer()
    {
        try
        {
            TimeSpan computedTimespan;
            if (_emTrainingSession.StartTimestamp.HasValue == true && _emTrainingSession.ExerciseDuration.HasValue && _emTrainingSession.ExerciseDuration.Value > 0)
            {
                computedTimespan = TimeSpan.FromSeconds(_emTrainingSession.ExerciseDuration.Value); //DateTime.UtcNow - _emTrainingSession.StartTimestamp.Value;
                _hours = computedTimespan.Hours;
                _minutes = computedTimespan.Minutes;
                _seconds = computedTimespan.Seconds;
            }
            else
            {
                _hours = 0;
                _minutes = 0;
                _seconds = 0;
            }
            this.TrainisngSessionTimerLabel.Text = string.Format("{0:00}:{1:00}:{2:00}", _hours, _minutes, _seconds);

            _dispatcherTimer = Dispatcher.CreateTimer();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
            _dispatcherTimer.Tick += Timer_Tick;

            if (_emTrainingSession.EndTimeStamp == null)
            {
                _dispatcherTimer.Start();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Starting Timer", "An error occurred while starting the timer.", "OK");
        }
        finally
        {

        }

    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        TimerTick();
    }

    private void StartTimer()
    {
        if (_dispatcherTimer != null)
        {
            if (_dispatcherTimer.IsRunning == false)
            {
                _dispatcherTimer.Start();
            }
        }
    }

    private void StopTimer()
    {
        if (_dispatcherTimer != null)
        {
            _dispatcherTimer.Stop();
        }
    }

    private void DisposeControls()
    {
        if (_dispatcherTimer != null)
        {
            _dispatcherTimer.Stop();
            _dispatcherTimer = null;
        }

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private async void AddSetButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        var k = button.Parent;

        ExercisePageViewModel setViewModel = (ExercisePageViewModel)button.BindingContext;

        HandleAddNewSet(setViewModel.ExerciseId);
    }

    private async void AddExerciseButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        HandleAddNewExercise();

    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        PauseTrainingSession();
        //// api call to save exerciseduration
        //await ExerciseApi.Net7.ExerciseApi.PauseTrainingSession(_emTrainingSession.Id, _hours * 3600 + _minutes * 60 + _seconds);

        //await Navigation.PopAsync();

    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = (RadioButton)sender;
    }

    //private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    //{
    //    CheckBox checkBox = (CheckBox)sender;
    //    HandleSetCheckChange(checkBox);
    //}

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        TimerTick();
    }

    private void MediaElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      //  PauseMediaElement(mediaElement);
    }

    private void FinishTraningSessionButton_Clicked(object sender, EventArgs e)
    {
        AddNewCompletedTrainingSesstionStatus();
    }

    private void PlayVideoButton_Clicked(object sender, EventArgs e)
    {
        //ImageButton imageButton = (ImageButton)sender;

        //ShowVideoPage(imageButton);
    }

    #endregion

    #region [Methods :: Tasks]


    public static async Task LoadExerciseViewModel(EmTrainingSession session)
    {
       
        foreach (EmExercise emExercise in session.emExercises)
        {
            await LoadExerciseInCurrentList(emExercise);
        }
    }

    public static async Task<string> GetSetHistory(long exerciseTypeId, short setnumber)
    {
        var k = await AirMemoryCache.GetUserHistory();
        var history = k.Where(t=>t.ExerciseTypeId == exerciseTypeId && t.SetNumber == setnumber).FirstOrDefault();
        if (history == null) return "";
        else return history.SetString;
    }

    public static async Task LoadSetInCurrentList(ExercisePageViewModel ExerciseModel, EmSet emSet)
    {
        SetPageViewModel setmodel = new SetPageViewModel();
        _totalSet = _totalSet + 1;

        setmodel.SetId = emSet.Id;
        setmodel.ExerciseId = emSet.ExerciseId;
        setmodel.TimeOffset = emSet.TimeOffset;
        setmodel.Exercise_type_Id = ExerciseModel.Exercise_type_id;

        setmodel.SetSequenceNumber = emSet.SetSequenceNumber == 1 ? "W" : emSet.SetSequenceNumber.ToString();
        setmodel.IsComplete = emSet.IsComplete;
        setmodel.IsSkipped = emSet.IsSkipped;
        setmodel.IsCustomerAddedSet = emSet.IsCustomerAddedSet;
        setmodel.EndTimeStamp = emSet.EndTimeStamp;
        setmodel.TimeOffset = emSet.TimeOffset;
        setmodel.SetName = await GetSetHistory(setmodel.Exercise_type_Id, emSet.SetSequenceNumber);




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
        ExerciseModel.SetviewModel.Add(setmodel);
    }

    public static async Task LoadExerciseInCurrentList(EmExercise emExercise)
    {
        ExercisePageViewModel viewmodel = null;
        viewmodel = new ExercisePageViewModel();
        viewmodel.SetviewModel = new ObservableCollection<SetPageViewModel>();

        viewmodel.ExerciseName = emExercise.EmExerciseType.Name;
        viewmodel.IsExerciseSkipped = emExercise.IsSkipped;
        viewmodel.IsExerciseComplete = emExercise.IsComplete;
        viewmodel.ExerciseEndTimeStamp = emExercise.EndTimeStamp;
        viewmodel.IsCustomerAddedExercise = emExercise.IsCustomerAddedExercise;

        viewmodel.ExplainerText = emExercise.EmExerciseType.ExplainerTextFr;
        viewmodel.Exercise_type_id = emExercise.FkExerciseTypeId;
        viewmodel.IsRecordExerciseTabContentVisible = true;
        viewmodel.IsHistoryTabContentVisible = false;
        viewmodel.IsSummaryTabContentVisible = false;
        viewmodel.ExerciseId = emExercise.Id;
        viewmodel.IsExerciseComplete = emExercise.IsComplete;
        viewmodel.IsCustomerAddedExercise = emExercise.IsCustomerAddedExercise;

        viewmodel.IsRecordExerciseTabContentVisible = true;
        viewmodel.IsHistoryTabContentVisible = false;
        viewmodel.IsSummaryTabContentVisible = false;

        viewmodel.MetricsName1 = "";
        viewmodel.MetricsName2 = "";

        foreach (EmSet emSet in emExercise.EmSet)
        {
            await LoadSetInCurrentList(viewmodel, emSet);

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
                            viewmodel.MetricsName1 = item.EmSetMetricTypes.Name;
                        }
                        else if (i == 1)
                        {
                            viewmodel.MetricsName2 = item.EmSetMetricTypes.Name;
                        }
                        i++;
                    }
                }
            }
        }
        _exerciseViewModels.Add(viewmodel);

    }

    private void ComputeCollectionViewHeight()
    {
#if IOS
        double screenHeight = Application.Current.MainPage.Height;
        int computedHeight = (int)Math.Round(screenHeight * 0.62);

        if (this.ExerciseContentRowDefinition != null)
        {
            this.ExerciseContentRowDefinition.Height = computedHeight;
        }
#endif
    }

    private async void HandleSetCheckChange(InputKit.Shared.Controls.CheckBox checkBox)
    {
        try
        {
            SetPageViewModel setPageViewModel = (SetPageViewModel)checkBox.BindingContext;
            switch (checkBox.IsChecked)
            {
                case true:
                    setPageViewModel.IsComplete = true;
                    if (_doApiCalls)
                    {
                        if (setPageViewModel.SetRestTimeSecs > 0)
                        {
                        //    var popup = new SetPopup(setPageViewModel.SetRestTimeSecs);
                          //  this.ShowPopup(popup);
                        }
                        await ExerciseApi.Net7.ExerciseApi.EndSet(setPageViewModel.SetId);
                        try
                        {
                            short SetNumber = 1;
                            string SetStats = setPageViewModel.MetricsValue1 + setPageViewModel.MetricsName1 + "x" + setPageViewModel.MetricsValue2 + setPageViewModel.MetricsName2;

                            if (setPageViewModel.SetSequenceNumber != "W") SetNumber = short.Parse(setPageViewModel.SetSequenceNumber.Trim());
                            // await MiddleWare.InsertSetHistory(setPageViewModel.Exercise_type_Id, SetNumber, SetStats);
                            MiddleWare.InsertSetHistory(setPageViewModel.Exercise_type_Id, SetNumber, SetStats);
                        }
                        catch(Exception e)
                        {

                        }
                    }
                    _totalSetCompleted = _totalSetCompleted + 1;

                    if (!String.IsNullOrEmpty(setPageViewModel.MetricsName1) && setPageViewModel.MetricsName1 == "kg")
                    {
                        if (!String.IsNullOrEmpty(setPageViewModel.MetricsName2) && setPageViewModel.MetricsName2 == "Reps")
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted + double.Parse(setPageViewModel.MetricsValue1) * double.Parse(setPageViewModel.MetricsValue2);
                        }
                        else
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted + double.Parse(setPageViewModel.MetricsValue1);
                        }
                    }
                    else if (!String.IsNullOrEmpty(setPageViewModel.MetricsName2) && setPageViewModel.MetricsName2 == "kg")
                    {
                        if (!String.IsNullOrEmpty(setPageViewModel.MetricsName1) && setPageViewModel.MetricsName1 == "Reps")
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted + double.Parse(setPageViewModel.MetricsValue1) * double.Parse(setPageViewModel.MetricsValue2);
                        }
                        else
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted + double.Parse(setPageViewModel.MetricsValue2);
                        }
                    }
                    break;
                case false:
                    setPageViewModel.IsComplete = false;
                    if (_doApiCalls)
                    {
                        await ExerciseApi.Net7.ExerciseApi.UndoEndSet(setPageViewModel.SetId);
                    }
                    _totalSetCompleted = _totalSetCompleted - 1;

                    if (!String.IsNullOrEmpty(setPageViewModel.MetricsName1) && setPageViewModel.MetricsName1 == "kg")
                    {
                        if (!String.IsNullOrEmpty(setPageViewModel.MetricsName2) && setPageViewModel.MetricsName2 == "Reps")
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted - double.Parse(setPageViewModel.MetricsValue1) * double.Parse(setPageViewModel.MetricsValue2);
                        }
                        else
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted - double.Parse(setPageViewModel.MetricsValue1);
                        }
                    }
                    else if (!String.IsNullOrEmpty(setPageViewModel.MetricsName2) && setPageViewModel.MetricsName2 == "kg")
                    {
                        if (!String.IsNullOrEmpty(setPageViewModel.MetricsName1) && setPageViewModel.MetricsName1 == "Reps")
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted - double.Parse(setPageViewModel.MetricsValue1) * double.Parse(setPageViewModel.MetricsValue2);
                        }
                        else
                        {
                            _totalSetUnitCompleted = _totalSetUnitCompleted - double.Parse(setPageViewModel.MetricsValue2);
                        }
                    }
                    break;

                default:
            }

            this.UnitOfMeasurementLabel.Text = string.Format("{0}{1}", _totalSetUnitCompleted,
                                    _unitInMeasurement);

            RefreshProgressLabel();

            this.TotalSetLabel.Text = _totalSetCompleted.ToString();


            AddNewCompleteExerciseStatus();
        }
        catch
        {
            await DisplayAlert("Start Set", "An error occurred while starting a set.", "OK");
        }
        finally
        {
        }


    }

    //private void PauseMediaElement(MediaElement mediaElement)
    //{
    //    if (mediaElement.IsVisible == false)
    //    {
    //        switch (mediaElement.CurrentState)
    //        {
    //            case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing:
    //                mediaElement.Pause();
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        //    mediaElement.Play();
    //    }
    //}

    private async void PauseTrainingSession()
    {
        try
        {
            await ExerciseApi.Net7.ExerciseApi.PauseTrainingSession(_emTrainingSession.Id, _hours * 3600 + _minutes * 60 + _seconds);
        }
        catch
        {
            await DisplayAlert("Pause Training Session", "An error occurred while pausing the training session.", "OK");
        }
        finally
        {
            await Navigation.PopAsync();
        }
    }

    private void TimerTick()
    {
        _seconds++;
        if (_seconds == 59)
        {
            _minutes++;
            _seconds = 0;
        }
        if (_minutes == 59)
        {
            _hours++;
            _minutes = 0;
        }
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.TrainisngSessionTimerLabel.Text = string.Format("{0:00}:{1:00}:{2:00}", _hours, _minutes, _seconds);
        });
    }

    //private void UnloadMediaElement()
    //{
    //    try
    //    {
    //        foreach (Frame frame in this.ExerciseListView.Children)
    //        {
    //            if (frame != null)
    //            {
    //                MediaElement mediaElement = frame.FindByName("MediaElement") as MediaElement;
    //                if (mediaElement != null)
    //                {
    //                    if (mediaElement.Handler != null)
    //                    {
    //                        switch (mediaElement.CurrentState)
    //                        {
    //                            case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing:
    //                            case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Paused:
    //                                mediaElement.Stop();
    //                                break;
    //                            default:
    //                                break;
    //                        }
    //                        mediaElement.Handler.DisconnectHandler();
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch
    //    {
    //        //Log error
    //    }
    //    finally
    //    {
    //    }
    //}

    private void CheckExerciseSets()
    {

    }

    private async void AddNewCompletedTrainingSesstionStatus()
    {
        try
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            string text = "Congratulation you have complete this training session.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;

            if (_totalSet == _totalSetCompleted)
            {
                bool completedTraningSession = await ExerciseApi.Net7.ExerciseApi.EndTrainingSession(_emTrainingSession.Id, _hours * 3600 + _minutes * 60 + _seconds);
                if (completedTraningSession == true)
                {
                    IToast toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token);
                    AirMemoryCache.RefreshUserHistory();
                    await Navigation.PushAsync(new ViewSummaryTrainingSessionContentPage(_emTrainingSession.Id));
                }
            }
            else
            {
                bool answer = true;

                //bool answer = await DisplayAlert("Notification", "There are still some exercises that need to be completed." + Environment.NewLine +
                //    "The sets that are not completed will be skipped" + Environment.NewLine +
                //    "Are you sure you want to finish the exercise?", "Yes", "No");

                if (answer == true)
                {
                    bool completedTraningSession = await ExerciseApi.Net7.ExerciseApi.EndTrainingSession(_emTrainingSession.Id, _hours * 3600 + _minutes * 60 + _seconds);
                    if (completedTraningSession == true)
                    {
                       // await DisplayAlert("Notification", "Congratulation you have finish this training session.", "OK");
                        await Navigation.PushAsync(new ViewSummaryTrainingSessionContentPage(_emTrainingSession.Id));
                    }
                }
            }
        }
        catch
        {
            await DisplayAlert("Add New Complete Training Session Status", "An error occurred while completing the training session.", "OK");
        }
        finally { }
    }

    private async void AddNewCompleteExerciseStatus()
    {
        if (!_doApiCalls) return;
        try
        {
            bool exerciseCompleted = false;

            if (_exerciseViewModels != null)
            {
                foreach (ExercisePageViewModel exercisePageViewModel in _exerciseViewModels.ToList())
                {
                    if (exercisePageViewModel.SetviewModel != null)
                    {
                        foreach (SetPageViewModel viewExerciseContentPageSetViewModel in exercisePageViewModel.SetviewModel)
                        {
                            if (viewExerciseContentPageSetViewModel.IsComplete)
                            {
                                exerciseCompleted = true;
                            }
                            else
                            {
                                exerciseCompleted = false;
                            }
                        }

                        if (exerciseCompleted == true)
                        {
                            await ExerciseApi.Net7.ExerciseApi.EndExercise(exercisePageViewModel.ExerciseId);
                        }
                        else
                        {
                        }
                    }
                }
            }
        }
        catch
        {
            await DisplayAlert("Add New Complete Exercise Status", "An error occurred while completing the exercise.", "OK");
        }
        finally { }
    }


    private AddExerciseContentPage addExerciseContentPage;
    private async void HandleAddNewExercise()
    {
        try
        {
            addExerciseContentPage = new AddExerciseContentPage(_emTrainingSession);
            addExerciseContentPage.ExerciseAdded += AddExerciseContentPage_ExerciseAdded;
            await Application.Current.MainPage.Navigation.PushModalAsync(addExerciseContentPage);
            RefreshExerciseListView();
        }
        catch
        {
            await DisplayAlert("Add New Exercise", "An error occurred while adding new exercise.", "OK");
        }
        finally
        {

        }
    }

    private async void AddExerciseContentPage_ExerciseAdded(object sender, EmExerciseSelectedHandler e)
    {
        try
        {
            addExerciseContentPage.ExerciseAdded -= AddExerciseContentPage_ExerciseAdded;
            var emExercise = await ExerciseApi.Net7.ExerciseApi.GetDefaultExerciseFromExerciseType(e.ExerciseId, _emTrainingSession.Id);
            await LoadExerciseInCurrentList(emExercise);
            RefreshProgressLabel();
        }
        catch
        {

        }
    }

    private void RefreshExerciseListView()
    {

    }

    private void SwipeItem_Invoked(object sender, EventArgs e)
    {
        SwipeItemView swipeItem = sender as SwipeItemView;
        SetPageViewModel setPageViewModel = swipeItem.BindingContext as SetPageViewModel;
        HandleDeleteSet(setPageViewModel);
    }

    private async void HandleAddNewSet(long ExerciseId)
    {
        try
        {
            EmSet emSet = await ExerciseApi.Net7.ExerciseApi.AddNewSetAndReturn(ExerciseId);
            var exerciseViewModel = _exerciseViewModels.Where(t => t.ExerciseId == ExerciseId).First();

            if (emSet != null)
            {
                await LoadSetInCurrentList(exerciseViewModel, emSet);
                RefreshExerciseListView();
                RefreshProgressLabel();
            }
        }
        catch
        {
        }
        finally
        {

        }
    }


    private async void HandleDeleteSet(SetPageViewModel setPageViewModel)
    {
        try
        {
            switch (setPageViewModel.IsComplete)
            {
                case true:

                    await DisplayAlert("Skip Set", "The set can not be skip because the set is already completed", "OK");
                    break;

                case false:
                    if (_doApiCalls)
                    {
                        bool setSkipped = await ExerciseApi.Net7.ExerciseApi.SkipSet(setPageViewModel.SetId);
                        if (setSkipped == true)
                        {
                            RemoveSetFromCurrentExerciseList(setPageViewModel);
                            RefreshProgressLabel();
                        }
                    }
                    break;

                default:
            }
        }
        catch
        {
        }
        finally
        {

        }
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        SetPageViewModel setPageViewModel = (SetPageViewModel)((Entry)sender).BindingContext;
        double newValue = 0;
        if (!double.TryParse(((Entry)sender).Text, out newValue)) return;
        if (_doApiCalls)
        {
            var success = ExerciseApi.Net7.ExerciseApi.ChangeSetMetrics(setPageViewModel.SetId, setPageViewModel.MetricId1, newValue);
            setPageViewModel.IsComplete = false;
        }
    }

    private void Entry_Completed_1(object sender, EventArgs e)
    {
        SetPageViewModel setPageViewModel = (SetPageViewModel)((Entry)sender).BindingContext;
        double newValue = 0;
        if (!double.TryParse(((Entry)sender).Text, out newValue)) return;
        if (_doApiCalls)
        {
            var success = ExerciseApi.Net7.ExerciseApi.ChangeSetMetrics(setPageViewModel.SetId, setPageViewModel.MetricId2, newValue);
            setPageViewModel.IsComplete = false;
        }
        ((Entry)sender).Unfocus();
    }

    private void Label_Unfocused(object sender, FocusEventArgs e)
    {
        SetPageViewModel setPageViewModel = (SetPageViewModel)((Entry)sender).BindingContext;
        double newValue = 0;
        if (!double.TryParse(((Entry)sender).Text, out newValue)) return;
        if (_doApiCalls)
        {
            var success = ExerciseApi.Net7.ExerciseApi.ChangeSetMetrics(setPageViewModel.SetId, setPageViewModel.MetricId1, newValue);
        }
       ((Entry)sender).Unfocus();
    }

    private void Label_Unfocused_1(object sender, FocusEventArgs e)
    {
        SetPageViewModel setPageViewModel = (SetPageViewModel)((Entry)sender).BindingContext;
        double newValue = 0;
        if (!double.TryParse(((Entry)sender).Text, out newValue)) return;
        if (_doApiCalls)
        {
            var success = ExerciseApi.Net7.ExerciseApi.ChangeSetMetrics(setPageViewModel.SetId, setPageViewModel.MetricId2, newValue);
        }
       ((Entry)sender).Unfocus();
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ExercisePageViewModel setPageViewModel = (ExercisePageViewModel)((StackLayout)sender).Parent.BindingContext;
        setPageViewModel.IsRecordExerciseTabContentVisible = true;
        setPageViewModel.IsSummaryTabContentVisible = false;
        setPageViewModel.IsHistoryTabContentVisible = false;
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        ExercisePageViewModel setPageViewModel = (ExercisePageViewModel)((StackLayout)sender).Parent.BindingContext;
        setPageViewModel.IsRecordExerciseTabContentVisible = false;
        setPageViewModel.IsSummaryTabContentVisible = true;
        setPageViewModel.IsHistoryTabContentVisible = false;
    }

    private void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        ExercisePageViewModel setPageViewModel = (ExercisePageViewModel)((StackLayout)sender).Parent.BindingContext;
        setPageViewModel.IsRecordExerciseTabContentVisible = false;
        setPageViewModel.IsSummaryTabContentVisible = false;
        setPageViewModel.IsHistoryTabContentVisible = true;
    }

    private void TapGestureRecognizer_Tapped_3(object sender, TappedEventArgs e)
    {
        SwipeItemView swipeItem = sender as SwipeItemView;
        if (swipeItem?.BindingContext is SetPageViewModel setPageViewModel)
        {
            HandleDeleteSet(setPageViewModel);
        }
        //_setPageViewModel = swipeItem.BindingContext as SetPageViewModel;
        
        
    }

    private void RemoveSetFromCurrentExerciseList(SetPageViewModel setPageViewModel)
    {

        ExercisePageViewModel exerciseViewModel = _exerciseViewModels.Where(exercise => exercise.ExerciseId == setPageViewModel.ExerciseId).First();
        exerciseViewModel.SetviewModel.Remove(setPageViewModel);
        _totalSet = _totalSet - 1;


    }

    private void RefreshProgressLabel()
    {
        double percentage = (_totalSetCompleted / _totalSet) * 100;
        this.ProgressLabel.Text = string.Format("{0}%", Math.Round(percentage));

        _progressArc.setPercentage(percentage);
        this.ProgressNeedleGraphicsView.Rotation = Math.Min(180, Math.Max((int)Math.Round(percentage * 1.8), 0));
        this.ProgressArcGraphicsView.Invalidate();
    }

    private async void ShowVideoPage(ImageButton imageButton)
    {
     //   ExercisePageViewModel exercisePageViewModel = (ExercisePageViewModel)imageButton.BindingContext;
      //  string videoUrl = exercisePageViewModel.VideoUrl;
      //  string exerciseDescription = exercisePageViewModel.ExplainerText;
      //  await Navigation.PushModalAsync(new ViewVideoContentPage(videoUrl, exerciseDescription));
    }


    #endregion

    private void CheckBox_CheckChanged(object sender, EventArgs e)
    {
        InputKit.Shared.Controls.CheckBox checkBox = (InputKit.Shared.Controls.CheckBox)sender;
        HandleSetCheckChange(checkBox);
    }

    
}