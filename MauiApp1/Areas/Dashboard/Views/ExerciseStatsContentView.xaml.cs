using FitnessData.Client.Business;
using FitnessData.Client.Business.Data;
using FitnessData.Common;
using FitnessData.Common.Data;
using MauiApp1.Business;
using MauiApp1.Pages.Profile;
using Microsoft.Maui.Authentication;
using Newtonsoft.Json;
using ParentMiddleWare;
using System.Net;
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

    private string _accessToken;
    //private string _endpoint = "https://fitnessdata-development.ageinreverse.me/en-US/v1/REST";
    private string _endpoint = "https://fitnessdata.ageinreverse.me/en-US/v1/REST";

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
        InitializeData();
        InitializeControl();
    }

    private async void InitializeData()
    {
        await RefreshData();      
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

    public async Task RefreshData()
    {
        string checkpoint = "Checkpoint 1";
        long totalRecords = 0;

        string caloriesBurnActiveAmount = " ";
        string caloriesBurnPassiveAmount = " ";
        string caloriesBurnTotalAmount = " ";
        string encryptedAccessToken = string.Empty; 

        try
        {
            this.ExerciseStatFrame.IsVisible = false;
            this.LoadingActivityIndicator.IsVisible = true;

            await Task.Run(() =>
            {
                // Now on background thread.
                encryptedAccessToken = Preferences.Default.Get(ConnectApplicationDeviceManager.ACCESS_TOKEN_STRING, string.Empty);
                _accessToken = EncryptDecryptManager.EncryptDecrypt(encryptedAccessToken, 300);

                if (!string.IsNullOrEmpty(_accessToken))
                {
                    //_accessToken = JsonConvert.DeserializeObject(accessTokenKey);

                    ConnectApplicationDeviceManager.mobileUserAccountTransactionManager =
                                      new SecurityServices.Client.Business.Transactions.MobileUserAccountTransactionManager(
                                          ConnectApplicationDeviceManager.APPCONNECTENDPOINT, _accessToken);

                    _caloriesBurnedDataManager = new CaloriesBurnedDataManager(_endpoint, _accessToken);

                    if (ConnectApplicationDeviceManager.mobileUserAccountTransactionManager.IsAlreadyConnected(MiddleWare.UserID) == true)
                    {
                        DateTimeOffset startDateTimeOffset = DateTimeOffset.Now;
                        DateTimeOffset endDateTimeOffset = DateTimeOffset.Now;

                        checkpoint = "Checkpoint 2";
                        startDateTimeOffset = new DateTimeOffset(this.SelectedDateTime.Year, this.SelectedDateTime.Month, this.SelectedDateTime.Day, 0, 0, 0, TimeSpan.FromHours(8));
                        endDateTimeOffset = new DateTimeOffset(this.SelectedDateTime.Year, this.SelectedDateTime.Month, this.SelectedDateTime.Day, 23, 59, 59, TimeSpan.FromHours(8));

                        checkpoint = "Checkpoint 3";
                        List<CaloriesBurnedDataPointViewItem> caloriesBurnedPerDayDataPoints =
                            _caloriesBurnedDataManager.GetTotalCaloriesBurnedPerDay(MiddleWare.UserID,
                            ref totalRecords, 1, 500, "", ""); ;

                        checkpoint = "Checkpoint 4";
                        if (caloriesBurnedPerDayDataPoints != null)
                        {
                            checkpoint = "Checkpoint 5";
                            _caloriesBurnedDataPoint = caloriesBurnedPerDayDataPoints.Where(t => t.LocalStartDateTimeOffset.Date == startDateTimeOffset.Date).FirstOrDefault();
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

                            checkpoint = "Checkpoint 27";
                            // Total Calories
                            if (_caloriesBurnedDataPoint.TotalCalories.HasValue == true)
                            {
                                checkpoint = "Checkpoint 28";
                                caloriesBurnTotalAmount = Math.Round(_caloriesBurnedDataPoint.TotalCalories.Value).ToString();
                            }
                            else
                            {
                                checkpoint = "Checkpoint 29";
                                caloriesBurnTotalAmount = " ";
                            }
                        }
                        else
                        {
                            checkpoint = "Checkpoint 30";
                            caloriesBurnActiveAmount = " ";
                            caloriesBurnPassiveAmount = " ";
                            caloriesBurnTotalAmount = " ";
                        }
                    }


                    // Report progress to UI.
                    Dispatcher.Dispatch(() =>
                    {
                        this.CaloriesBurnActiveAmountLabelSpan.Text = caloriesBurnActiveAmount;
                        this.CaloriesBurnPassiveAmountLabelSpan.Text = caloriesBurnPassiveAmount;
                        this.CaloriesBurnTotalAmountLabelSpan.Text = caloriesBurnTotalAmount;

                        this.ExerciseStatFrame.IsVisible = true;
                    });

                }

            });
           
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            //if (string.IsNullOrWhiteSpace(_accessToken) == false)
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "Age In Reverse has been disconnected from Google Fit. Please reconnect the app through the profile menu.", "OK");
            //}
            //else
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "App is not connected to Google Fit. Please connect the app through the profile menu.", "OK");
            //}
            ViewProfile.ClearFitnessServiceStorage();

            this.ExerciseStatFrame.IsVisible = false;
        }
        catch (Exception exception)
        {
            //if (string.IsNullOrWhiteSpace(_accessToken) == false)
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "Age In Reverse has been disconnected from Google Fit. Please reconnect the app through the profile menu.", "OK");
            //}
            //else
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "App is not connected to Google Fit. Please connect the app through the profile menu.", "OK");
            //}

            Preferences.Default.Set(ConnectApplicationDeviceManager.ACCESS_TOKEN_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_TOKEN_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.EMAIL_ADDRESS_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.EXPIRES_IN_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_EXPIRES_IN_STRING, string.Empty);

            this.CaloriesBurnActiveAmountLabelSpan.Text = " ";
            this.CaloriesBurnPassiveAmountLabelSpan.Text = " ";
            this.CaloriesBurnTotalAmountLabelSpan.Text = " ";

            this.ExerciseStatFrame.IsVisible = false;


            //App.Current.MainPage.DisplayAlert("Retrieve Calories Burned" + checkpoint, "The system encountered a problem while retrieving calories burned.", "OK");
            //App.Current.MainPage.DisplayAlert("Retrieve Calories Burned" + checkpoint, exception.Message, "OK");
            //await App.Current.MainPage.DisplayAlert(exception.Message, exception.StackTrace, "OK");
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