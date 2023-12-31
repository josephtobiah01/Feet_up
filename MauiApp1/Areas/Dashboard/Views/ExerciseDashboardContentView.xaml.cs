//using DeviceIntegration.Client.Business;
//using DeviceIntegration.Common.Data;
using FitnessData.Client.Business;
using FitnessData.Client.Business.Data;
using FitnessData.Common;
using FitnessData.Common.Data;
using MauiApp1.Areas.Dashboard.Resources.Drawables;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Business;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Exceptions;
using MauiApp1.Pages.Profile;
using Newtonsoft.Json;
using ParentMiddleWare;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class ExerciseDashboardContentView : ContentView
{
    #region [Fields]

    private CaloriesBurnedDataPointViewItem _caloriesBurnedDataPoint;
    private CaloriesBurnedDataManager _caloriesBurnedDataManager;
    private CaloriesBurnedPerDayBarChartDrawable _caloriesBurnedPerDayBarChartDrawable;

    private string _accessToken;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ExerciseDashboardContentView()
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
        string encryptedAccessToken = string.Empty;

        try
        {
            if (ConnectedDeviceDataStorageManager.googleFit != null)
            {
                _accessToken = ConnectedDeviceDataStorageManager.googleFit.AccessToken;
            }
            else if(ConnectedDeviceDataStorageManager.appleHealthKit != null)
            {
                _accessToken = ConnectedDeviceDataStorageManager.appleHealthKit.AccessToken;
            }
            else
            {
              
            }

            _caloriesBurnedPerDayBarChartDrawable = new CaloriesBurnedPerDayBarChartDrawable();

            if (string.IsNullOrWhiteSpace(_accessToken) == false)
            {
                ConnectApplicationDeviceManager.mobileUserAccountAdministrator =
                              new SecurityServices.Client.Business.MobileUserAccountAdministrator(
                                  MiddleWare.AppConnectEndpoint, _accessToken);

                _caloriesBurnedDataManager = new CaloriesBurnedDataManager(MiddleWare.FitnessDataEndpoint, _accessToken);
                if (ConnectApplicationDeviceManager.mobileUserAccountAdministrator.IsAlreadyConnected(MiddleWare.FkFederatedUser) == true)
                {
                    this.CaloriesBurnedPerDayFrame.IsVisible = true;
                    await LoadCaloriesExpendedPerDayStackedBarChart();
                }
                else
                {
                    if (this.CaloriesBurnedPerDayFrame != null)
                    {
                        this.CaloriesBurnedPerDayFrame.IsVisible = false;
                    }

                    if (this.LoadingActivityIndicator != null)
                    {
                        this.LoadingActivityIndicator.IsVisible = false;
                    }
                }
            }
            else
            {
                if (this.LoadingActivityIndicator != null)
                {
                    this.LoadingActivityIndicator.IsVisible = false;
                }
            }
        }
        catch (DeviceStorageException deviceStorageException)
        {
            await ForceSignOutUser();
        }
        catch (Exception exception)
        {

        }
        finally
        {

        }
    }


    private void InitializeControl()
    {
        //this.DatePicker.Date = DateTime.Now;
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void ConnectToDeviceButton_Clicked(object sender, EventArgs e)
    {
        //      ConnectApplicationToGoogleFit();
    }

    #endregion

    #region [Methods :: Tasks]

    private async Task LoadCaloriesExpendedPerDayStackedBarChart()
    {
        try
        {
            long totalRecords = 0;

            DateTimeOffset startDateTimeOffset = new DateTimeOffset(this.SelectedDateTime.Date.Year, this.SelectedDateTime.Date.Month, this.SelectedDateTime.Date.Day, 0, 0, 0, TimeSpan.FromHours(8));
            DateTimeOffset endDateTimeOffset = new DateTimeOffset(this.SelectedDateTime.Date.Year, this.SelectedDateTime.Date.Month, this.SelectedDateTime.Date.Day, 23, 59, 59, TimeSpan.FromHours(8));

            await Task.Run(() =>
            {

                Dispatcher.Dispatch(() =>
                {
                    if (this.LoadingActivityIndicator != null)
                    {
                        this.LoadingActivityIndicator.IsVisible = true;
                    }

                    if (this.DashboardScrollView != null)
                    {
                        this.DashboardScrollView.IsVisible = false;
                    }

                    if (this.DashboardVerticalStackLayout != null)
                    {
                        this.DashboardVerticalStackLayout.IsVisible = false;
                    }

                });

                List<CaloriesBurnedDataPointViewItem> caloriesBurnedPerDayDataPoints =
                  _caloriesBurnedDataManager.GetTotalCaloriesBurnedPerDay(MiddleWare.FkFederatedUser,
                  ref totalRecords, 1, 500, "", "");

                if (caloriesBurnedPerDayDataPoints != null)
                {
                    _caloriesBurnedDataPoint = caloriesBurnedPerDayDataPoints.Where(t => t.LocalStartDateTimeOffset.Date == startDateTimeOffset.Date).FirstOrDefault();
                }

                if (_caloriesBurnedDataPoint != null)
                {
                    // Make passive calories burned pro-rated
                    if (startDateTimeOffset.Date == DateTimeOffset.Now.Date)
                    {
                        if (_caloriesBurnedDataPoint.PassiveCalories.HasValue && _caloriesBurnedDataPoint.TotalCalories.HasValue)
                        {
                            int totalHoursInADay = 24;

                            // Hours
                            double hoursPassed = (DateTimeOffset.Now - startDateTimeOffset).TotalHours;
                            double roundedDownHoursPassed = Math.Floor(hoursPassed);



                            double currentPassiveCaloriesBurned = (roundedDownHoursPassed / totalHoursInADay) * _caloriesBurnedDataPoint.PassiveCalories.Value;
                            double newActiveCaloriesBurned = _caloriesBurnedDataPoint.TotalCalories.Value - currentPassiveCaloriesBurned;

                            if (newActiveCaloriesBurned <= 0)
                            {
                                newActiveCaloriesBurned = 0;
                            }

                            // Set new values for active and passive calories
                            _caloriesBurnedDataPoint.PassiveCalories = currentPassiveCaloriesBurned;
                            _caloriesBurnedDataPoint.ActiveCalories = newActiveCaloriesBurned;
                        }
                    }
                }

                // Report progress to UI.
                Dispatcher.Dispatch(() =>
                {
                    if (_caloriesBurnedPerDayBarChartDrawable != null)
                    {
                        _caloriesBurnedPerDayBarChartDrawable = new CaloriesBurnedPerDayBarChartDrawable();
                        _caloriesBurnedPerDayBarChartDrawable.CaloriesBurnedDataPoint = _caloriesBurnedDataPoint;
                        _caloriesBurnedPerDayBarChartDrawable.StartDateTimeOffset = startDateTimeOffset;
                        this.CaloriesExpendedPerDayGraphicsView.Drawable = _caloriesBurnedPerDayBarChartDrawable;
                        this.CaloriesExpendedPerDayGraphicsView.Invalidate();
                    }

                    if (this.DashboardScrollView != null)
                    {
                        this.DashboardScrollView.IsVisible = true;
                    }

                    if (this.DashboardVerticalStackLayout != null)
                    {
                        this.DashboardVerticalStackLayout.IsVisible = true;
                    }


                });

            });


        }
        catch (UnauthorizedAccessException ex)
        {
            //if (string.IsNullOrWhiteSpace(_accessToken) == false)
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "Age In Reverse has been disconnected from Google Fit. Please reconnect the app through the profile menu.", "OK");
            //}
            //else
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "App is not connected to Google Fit. Please connect the app through the profile menu.", "OK");
            //}
            ClearFitnessServiceData();

            if (this.DashboardScrollView != null)
            {
                this.DashboardScrollView.IsVisible = false;
            }

            if (this.DashboardVerticalStackLayout != null)
            {
                this.DashboardVerticalStackLayout.IsVisible = false;
            }

        }
        catch (Exception ex)
        {
            this.CaloriesExpendedPerDayGraphicsView.Drawable = new CaloriesBurnedPerDayBarChartDrawable();
            //await  App.Current.MainPage.DisplayAlert("Retrieve Calories Burned", "An error occured while fetching calories burned data." +
            //      " Please ensure that you are connected in the internet.","OK");

            //if (string.IsNullOrWhiteSpace(_accessToken) == false)
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "Age In Reverse has been disconnected from Google Fit. Please reconnect the app through the profile menu.", "OK");
            //}
            //else
            //{
            //    await App.Current.MainPage.DisplayAlert("Unauthorized Access", "App is not connected to Google Fit. Please connect the app through the profile menu.", "OK");
            //}

            ClearFitnessServiceData();

            //Preferences.Default.Set(ConnectApplicationDeviceManager.ACCESS_TOKEN_STRING, string.Empty);
            //Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_TOKEN_STRING, string.Empty);
            //Preferences.Default.Set(ConnectApplicationDeviceManager.EMAIL_ADDRESS_STRING, string.Empty);
            //Preferences.Default.Set(ConnectApplicationDeviceManager.EXPIRES_IN_STRING, string.Empty);
            //Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_EXPIRES_IN_STRING, string.Empty);

            if (this.DashboardScrollView != null)
            {
                this.DashboardScrollView.IsVisible = false;
            }

            if (this.DashboardVerticalStackLayout != null)
            {
                this.DashboardVerticalStackLayout.IsVisible = false;
            }

        }
        finally
        {
            if (this.LoadingActivityIndicator != null)
            {
                this.LoadingActivityIndicator.IsVisible = false;
            }


        }
         
    }

    private async void ClearFitnessServiceData()
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
        await DeviceUserAccountManager.ForceUserSignOut();
        if (App.Current.MainPage != null)
        {
            //await App.Current.MainPage.Navigation.PushAsync(new MVPLoginContentPage());
            App.Current.MainPage = new NavigationPage(new MVPLoginContentPage());
            await Application.Current.MainPage.Navigation.PopToRootAsync();
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

    #region [Methods :: Public Tasks] 

    public async void RefreshData()
    {
        try
        {
            await LoadCaloriesExpendedPerDayStackedBarChart();
        }
        catch (Exception ex)
        {
            ShowAlertBottomSheet("Refresh Calories Chart", "The system encountered a problem while refreshing calories chart.", "OK");
        }
        finally
        {

        }
    }

    #endregion

    #region [Methods :: Properties]

    public DateTime SelectedDateTime;

    #endregion

}