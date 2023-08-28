using FitnessData.Client.Business;
using FitnessData.Client.Business.Data;
using FitnessData.Common;
using FitnessData.Common.Data;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Business;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Exceptions;
using MauiApp1.Pages.Profile;
using Microsoft.Maui.Authentication;
using Newtonsoft.Json;
using ParentMiddleWare;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class ExerciseStatsContentView : ContentView
{
    #region [Fields]

    public event EventHandler Tapped;

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ExerciseStatsContentView));

    private CaloriesBurnedDataPointViewItem _caloriesBurnedDataPoint;
    private CaloriesBurnedDataManager _caloriesBurnedDataManager;

    private List<CaloriesBurnedDataPointViewItem> _caloriesBurnedPerDayDataPoints;
    private string _accessToken;

    public bool _isLoading = false;
    public bool _isLoadedAlready = false;
   
    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ExerciseStatsContentView()
    {
        InitializeComponent();
    }

    public object CommandParameter
    {
        get { return (object)GetValue(CommandParameterProperty); }
        set { SetValue(CommandParameterProperty, value); }
    }

    private void ContentView_Loaded(object sender, EventArgs e)
    {      
        if (_isLoadedAlready == false)
        {
            _isLoadedAlready = true;
        }
        else
        {
            return;
        }     

        InitializeData();
        InitializeControl();
    }

    private void ContentView_Unloaded(object sender, EventArgs e)
    {
       
    }

    private void InitializeData()
    {
        RefreshData();
    }

    private void InitializeControl()
    {

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
    {
        if (Tapped != null)
            Tapped(this, new EventArgs());
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        //App.Current.MainPage.DisplayAlert("Tapped", "CLick is tap", "OK");
        GotoExerciseDashboardContentPage();
    }

    #endregion

    #region [Methods :: Tasks]   

    private async void GotoExerciseDashboardContentPage()
    {
        ExerciseDashboardContentPage exerciseDashboardContentPage = new ExerciseDashboardContentPage();
        exerciseDashboardContentPage.SelectedDateTime = this.SelectedDateTime;
        await Navigation.PushAsync(exerciseDashboardContentPage);
    }



    #endregion

    #region [Methods :: Public Tasks] 

    public async void RefreshData()
    {
        string checkpoint = "";
        long totalRecords = 0;

        string caloriesBurnActiveAmount = " ";
        string caloriesBurnPassiveAmount = " ";
        string caloriesBurnTotalAmount = " ";
        string encryptedAccessToken = string.Empty;

        DateTimeOffset startDateTimeOffset = DateTimeOffset.Now;
        DateTimeOffset endDateTimeOffset = DateTimeOffset.Now;

        bool isConnected = false;

        if (_isLoadedAlready == false)
        {
            return;
        }

        if (_isLoading == true)
        {
            return;
        }
        else
        {
            _isLoading = true;
        }

        try
        {
            this.ExerciseStatFrame.IsVisible = false;
            this.LoadingActivityIndicator.IsVisible = true;

            if (ConnectedDeviceDataStorageManager.googleFit != null)
            {
                _accessToken = ConnectedDeviceDataStorageManager.googleFit.AccessToken;
            }
            else if (ConnectedDeviceDataStorageManager.appleHealthKit != null)
            {
                _accessToken = ConnectedDeviceDataStorageManager.appleHealthKit.AccessToken;
            }
            else
            {

            }

            if (!string.IsNullOrEmpty(_accessToken))
            {
                //_accessToken = JsonConvert.DeserializeObject(accessTokenKey);

                ConnectApplicationDeviceManager.mobileUserAccountAdministrator =
                                  new SecurityServices.Client.Business.MobileUserAccountAdministrator(
                                      MiddleWare.AppConnectEndpoint, _accessToken);

                _caloriesBurnedDataManager = new CaloriesBurnedDataManager(MiddleWare.FitnessDataEndpoint, _accessToken);

                await Task.Delay(2);
                isConnected = ConnectApplicationDeviceManager.mobileUserAccountAdministrator.IsAlreadyConnected(MiddleWare.FkFederatedUser);
               

                if (isConnected == true)
                {
                    checkpoint = "Checkpoint 2";
                    startDateTimeOffset = new DateTimeOffset(this.SelectedDateTime.Year, this.SelectedDateTime.Month, this.SelectedDateTime.Day, 0, 0, 0, TimeSpan.FromHours(8));
                    endDateTimeOffset = new DateTimeOffset(this.SelectedDateTime.Year, this.SelectedDateTime.Month, this.SelectedDateTime.Day, 23, 59, 59, TimeSpan.FromHours(8));

                    checkpoint = "Checkpoint 3";
                    await Task.Delay(2);
                    _caloriesBurnedPerDayDataPoints =
                        _caloriesBurnedDataManager.GetTotalCaloriesBurnedPerDay(MiddleWare.FkFederatedUser,
                        ref totalRecords, 1, 500, "", ""); ;

                    //await Task.Delay(2);
                    checkpoint = "Checkpoint 4";
                    if (_caloriesBurnedPerDayDataPoints != null)
                    {
                        checkpoint = "Checkpoint 5";
                        _caloriesBurnedDataPoint = _caloriesBurnedPerDayDataPoints.Where(t => t.LocalStartDateTimeOffset.Date == startDateTimeOffset.Date).FirstOrDefault();
                    }
                    checkpoint = "Checkpoint 6";
                    if (_caloriesBurnedDataPoint != null)
                    {
                        // Make passive calories burned pro-rated
                        if (startDateTimeOffset.Date == DateTimeOffset.Now.Date)
                        {
                            checkpoint = "Checkpoint 7";
                            if (_caloriesBurnedDataPoint.TotalCalories == null)
                            {
                                checkpoint = "Checkpoint 8";
                                _caloriesBurnedDataPoint.PassiveCalories = null;
                                _caloriesBurnedDataPoint.ActiveCalories = null;
                            }
                            else
                            {
                                checkpoint = "Checkpoint 9";
                                if (_caloriesBurnedDataPoint.PassiveCalories == null)
                                {
                                    checkpoint = "Checkpoint 10";
                                    _caloriesBurnedDataPoint.ActiveCalories = _caloriesBurnedDataPoint.TotalCalories;
                                }
                                else
                                {
                                    checkpoint = "Checkpoint 11";
                                    int totalMinutesInADay = 1440;

                                    checkpoint = "Checkpoint 12";
                                    // Compute pro-rated passive calories per minute
                                    double minutesPassed = (DateTimeOffset.Now - startDateTimeOffset).TotalMinutes;

                                    checkpoint = "Checkpoint 13";
                                    double roundedDownMinutesPassed = Math.Floor(minutesPassed);

                                    checkpoint = "Checkpoint 14";
                                    int currentPassiveCaloriesBurned = (int)((roundedDownMinutesPassed / totalMinutesInADay) * _caloriesBurnedDataPoint.PassiveCalories.Value);

                                    checkpoint = "Checkpoint 15";
                                    if (currentPassiveCaloriesBurned > _caloriesBurnedDataPoint.TotalCalories.Value)
                                    {
                                        checkpoint = "Checkpoint 16";
                                        currentPassiveCaloriesBurned = (int)_caloriesBurnedDataPoint.TotalCalories.Value;
                                        checkpoint = "Checkpoint 17";
                                        _caloriesBurnedDataPoint.PassiveCalories = currentPassiveCaloriesBurned;
                                    }

                                    checkpoint = "Checkpoint 18";
                                    double newActiveCaloriesBurned = _caloriesBurnedDataPoint.TotalCalories.Value - currentPassiveCaloriesBurned;

                                    checkpoint = "Checkpoint 19";
                                    // Set new values for active and passive calories
                                    _caloriesBurnedDataPoint.PassiveCalories = currentPassiveCaloriesBurned;

                                    checkpoint = "Checkpoint 20";
                                    _caloriesBurnedDataPoint.ActiveCalories = newActiveCaloriesBurned;
                                }
                            }
                        }


                        checkpoint = "Checkpoint 21";
                        // Active Calories
                        if (_caloriesBurnedDataPoint.ActiveCalories.HasValue == true)
                        {
                            checkpoint = "Checkpoint 22";
                            caloriesBurnActiveAmount = Math.Round(_caloriesBurnedDataPoint.ActiveCalories.Value).ToString();
                        }
                        else
                        {
                            checkpoint = "Checkpoint 23";
                            caloriesBurnActiveAmount = " ";
                        }

                        checkpoint = "Checkpoint 24";
                        // Passive Calories
                        if (_caloriesBurnedDataPoint.PassiveCalories.HasValue == true)
                        {
                            checkpoint = "Checkpoint 25";
                            caloriesBurnPassiveAmount = Math.Round(_caloriesBurnedDataPoint.PassiveCalories.Value).ToString();
                        }
                        else
                        {
                            checkpoint = "Checkpoint 26";
                            caloriesBurnPassiveAmount = " ";
                        }


                        // Total Calories
                        if (_caloriesBurnedDataPoint.TotalCalories.HasValue == true)
                        {
                            caloriesBurnTotalAmount = Math.Round(_caloriesBurnedDataPoint.TotalCalories.Value).ToString();
                        }
                        else
                        {
                            caloriesBurnTotalAmount = " ";
                        }
                    }
                    else
                    {
                        caloriesBurnActiveAmount = " ";
                        caloriesBurnPassiveAmount = " ";
                        caloriesBurnTotalAmount = " ";
                    }
                }


                this.CaloriesBurnActiveAmountLabelSpan.Text = caloriesBurnActiveAmount;
                this.CaloriesBurnPassiveAmountLabelSpan.Text = caloriesBurnPassiveAmount;
                this.CaloriesBurnTotalAmountLabelSpan.Text = caloriesBurnTotalAmount;

                this.ExerciseStatFrame.IsVisible = true;
            }

        }
        catch (DeviceStorageException deviceStorageException)
        {
            await ForceSignOutUser();
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            try
            {
                this.ExerciseStatFrame.IsVisible = false;

                ClearFitnessServiceStorage();
            }
            catch
            {

            }
            
        }
        catch (Exception exception)
        {
            try
            {
                this.CaloriesBurnActiveAmountLabelSpan.Text = " ";
                this.CaloriesBurnPassiveAmountLabelSpan.Text = " ";
                this.CaloriesBurnTotalAmountLabelSpan.Text = " ";

                if (this.ExerciseStatFrame != null)
                {
                    this.ExerciseStatFrame.IsVisible = false;
                }
                _accessToken = string.Empty;

                ClearFitnessServiceStorage();
            }
            catch
            {

            }
                     
        }
        finally
        {
            try
            {
                if (this.LoadingActivityIndicator != null)
                {
                    this.LoadingActivityIndicator.IsVisible = false;
                }
                _isLoading = false;
            }
            catch
            {

            }
            
        }

        //Task task = Task.Run(() =>
        //{
        //    // Now on background thread.           
        //    if (!string.IsNullOrEmpty(_accessToken))
        //    {
        //        isConnected = ConnectApplicationDeviceManager.mobileUserAccountTransactionManager.IsAlreadyConnected(MiddleWare.UserID);
        //        if (isConnected == true)
        //        {
        //            _caloriesBurnedPerDayDataPoints =
        //                _caloriesBurnedDataManager.GetTotalCaloriesBurnedPerDay(MiddleWare.UserID,
        //                ref totalRecords, 1, 500, "", "");                    
        //        }
        //    }

        //});

        //task.ContinueWith(
        // continuation =>
        // {
        //     try
        //     {
        //         if (continuation.IsFaulted)
        //         {
        //             if (continuation.Exception != null)
        //             {
        //                 HandleExceptions(continuation.Exception.InnerExceptions);
        //             }
        //         }
        //         else
        //         {
        //             if (_caloriesBurnedPerDayDataPoints != null)
        //             {
        //                 _caloriesBurnedDataPoint = _caloriesBurnedPerDayDataPoints.Where(t => t.LocalStartDateTimeOffset.Date == startDateTimeOffset.Date).FirstOrDefault();
        //             }

        //             if (_caloriesBurnedDataPoint != null)
        //             {
        //                 // Make passive calories burned pro-rated
        //                 if (startDateTimeOffset.Date == DateTimeOffset.Now.Date)
        //                 {
        //                     if (_caloriesBurnedDataPoint.TotalCalories == null)
        //                     {
        //                         _caloriesBurnedDataPoint.PassiveCalories = null;
        //                         _caloriesBurnedDataPoint.ActiveCalories = null;
        //                     }
        //                     else
        //                     {
        //                         if (_caloriesBurnedDataPoint.PassiveCalories == null)
        //                         {

        //                             _caloriesBurnedDataPoint.ActiveCalories = _caloriesBurnedDataPoint.TotalCalories;
        //                         }
        //                         else
        //                         {

        //                             int totalMinutesInADay = 1440;


        //                             // Compute pro-rated passive calories per minute
        //                             double minutesPassed = (DateTimeOffset.Now - startDateTimeOffset).TotalMinutes;

        //                             double roundedDownMinutesPassed = Math.Floor(minutesPassed);

        //                             int currentPassiveCaloriesBurned = (int)((roundedDownMinutesPassed / totalMinutesInADay) * _caloriesBurnedDataPoint.PassiveCalories.Value);

        //                             if (currentPassiveCaloriesBurned > _caloriesBurnedDataPoint.TotalCalories.Value)
        //                             {

        //                                 currentPassiveCaloriesBurned = (int)_caloriesBurnedDataPoint.TotalCalories.Value;

        //                                 _caloriesBurnedDataPoint.PassiveCalories = currentPassiveCaloriesBurned;
        //                             }


        //                             double newActiveCaloriesBurned = _caloriesBurnedDataPoint.TotalCalories.Value - currentPassiveCaloriesBurned;

        //                             // Set new values for active and passive calories
        //                             _caloriesBurnedDataPoint.PassiveCalories = currentPassiveCaloriesBurned;

        //                             _caloriesBurnedDataPoint.ActiveCalories = newActiveCaloriesBurned;
        //                         }
        //                     }
        //                 }

        //                 // Active Calories
        //                 if (_caloriesBurnedDataPoint.ActiveCalories.HasValue == true)
        //                 {
        //                     caloriesBurnActiveAmount = Math.Round(_caloriesBurnedDataPoint.ActiveCalories.Value).ToString();
        //                 }
        //                 else
        //                 {
        //                     caloriesBurnActiveAmount = " ";
        //                 }

        //                 // Passive Calories
        //                 if (_caloriesBurnedDataPoint.PassiveCalories.HasValue == true)
        //                 {
        //                     caloriesBurnPassiveAmount = Math.Round(_caloriesBurnedDataPoint.PassiveCalories.Value).ToString();
        //                 }
        //                 else
        //                 {
        //                     caloriesBurnPassiveAmount = " ";
        //                 }

        //                 // Total Calories
        //                 if (_caloriesBurnedDataPoint.TotalCalories.HasValue == true)
        //                 {
        //                     caloriesBurnTotalAmount = Math.Round(_caloriesBurnedDataPoint.TotalCalories.Value).ToString();
        //                 }
        //                 else
        //                 {
        //                     caloriesBurnTotalAmount = " ";
        //                 }
        //             }
        //             else
        //             {
        //                 caloriesBurnActiveAmount = " ";
        //                 caloriesBurnPassiveAmount = " ";
        //                 caloriesBurnTotalAmount = " ";
        //             }

        //             if (!string.IsNullOrEmpty(_accessToken))
        //             {
        //                 this.CaloriesBurnActiveAmountLabelSpan.Text = caloriesBurnActiveAmount;
        //                 this.CaloriesBurnPassiveAmountLabelSpan.Text = caloriesBurnPassiveAmount;
        //                 this.CaloriesBurnTotalAmountLabelSpan.Text = caloriesBurnTotalAmount;

        //                 this.ExerciseStatFrame.IsVisible = true;
        //             }

        //         }

        //         this.LoadingActivityIndicator.IsVisible = false;
        //     }
        //     catch(Exception ex)
        //     {

        //     }
        //     finally
        //     {
        //         _isLoading = false;
        //     }
        // },
        // TaskScheduler.FromCurrentSynchronizationContext());

    }

    private async void ClearFitnessServiceStorage()
    {
        try
        {
            DeviceStorageManager.ClearFitnessServiceStorage(MiddleWare.UseSecuredStorage);
        }
        catch (DeviceStorageException deviceStorageException)
        {
            await ForceSignOutUser();
        }
        catch (Exception ex)
        {

        }
        finally
        {

        }
    }

    private async Task ForceSignOutUser()
    {
        try
        {
            await DeviceUserAccountManager.ForceUserSignOut();
            if (App.Current.MainPage != null)
            {
                App.Current.MainPage = new NavigationPage(new MVPLoginContentPage());
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
        }
        catch(Exception ex)
        {

        }        
    }

    private void HandleExceptions(ICollection<Exception> exceptions)
    {

        /* 
         * Referrence : https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/exception-handling-task-parallel-library 
         */

        foreach (Exception ex in exceptions)
        {
            // Handle the custom exception.
            if (ex is DeviceStorageException)
            {
                Dispatcher.Dispatch(async () =>
                {
                    await ForceSignOutUser();
                });

            }
            else if (ex is UnauthorizedAccessException)
            {
                this.ExerciseStatFrame.IsVisible = false;
                ClearFitnessServiceStorage();
            }
            else
            {
                this.CaloriesBurnActiveAmountLabelSpan.Text = " ";
                this.CaloriesBurnPassiveAmountLabelSpan.Text = " ";
                this.CaloriesBurnTotalAmountLabelSpan.Text = " ";

                this.ExerciseStatFrame.IsVisible = false;

                ClearFitnessServiceStorage();
            }
        }

    }

    #endregion

    #region [Methods :: Properties]

    public DateTime SelectedDateTime;

    #endregion

    #region [Fields :: Public]

    public bool IsLoading
    {
        get { return _isLoading; }
    }

    #endregion

}