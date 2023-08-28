using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MauiApp1.Areas.Supplement.ViewModels;
using Microsoft.AspNetCore.Components;
using static System.Net.Mime.MediaTypeNames;

namespace MauiApp1.Pages.Supplement
{
    public partial class ViewSnoozePage
    {
        #region [Fields]

        //private bool _isViewSupplementItemHidden = true;

        private SupplementPageViewModel _supplementPageViewModel { get; set; }

        private List<SupplementPageViewModel> _supplementPageViewModels { get; set; }

        #endregion

        #region [Methods :: EventHandlers :: Class]

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            IntializeData();
        }

        private void IntializeData()
        {
            _supplementPageViewModel = this.SupplementPageViewModel;
            _supplementPageViewModels = this.SupplementPageViewModels;
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private void CloseButton_Click()
        {
            ClosePage();
        }

        private void ThirtyMinutesSnoozeButton_Click()
        {
            //AddThirtyMinutesSnoozeTime();
            HandleThirtyMinutesSnoozeButtonClick();
        }

        private void FortyFiveMinutesSnoozeButton_Click()
        {
            //AddFortyMinutesSnoozeTime();
            HandleFortyFiveMinutesSnoozeButtonClick();
        }

        private void OneHourSnoozeButton_Click()
        {
            //AddOneHourSnoozeTime();
            HandleOneHourSnoozeButtonClick();
        }

        private void CustomSnoozeButton_Click()
        {
            //AddCustomeSnoozeTime();
            HandleCustomSnoozeButtonClick();
        }

        #endregion

        #region [Methods :: Tasks]

        private async void ClosePage()
        {
            if (OnCloseClickCallback.HasDelegate == true)
            {
                await OnCloseClickCallback.InvokeAsync();
            }
            else
            {
                ShowAlertBottomSheet("Close Snooze Page", "OnCloseClickCallback is not been set", "OK");
            }
        }

        private async void InvokeSuccessEventCallback(long? id, int waitMinute)
        {
            if (SuccessEventCallback.HasDelegate == true)
            {
                await SuccessEventCallback.InvokeAsync(waitMinute);
            }
            else
            {

            }

            if (GetSupplementIdEventCallback.HasDelegate == true)
            {
                await GetSupplementIdEventCallback.InvokeAsync(id);
            }
            else
            {

            }
        }

        private async void HandleThirtyMinutesSnoozeButtonClick()
        {
            try
            {
                if (_supplementPageViewModel != null)
                {
                    await AddThirtyMinutesSnoozeDose();
                }
                else if (_supplementPageViewModels != null)
                {
                    await AddThirtyMinutesSnoozeDoses();
                }
                else
                {
                    //Error
                    ShowAlertBottomSheet("Add 30 minutes Snooze", "An error occurred while adding 30 minutes of time.", "OK");
                }
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 30 minutes Snooze", "An error occurred while adding 30 minutes of time.", "OK");
            }
            finally
            {

            }
        }

        private async void HandleFortyFiveMinutesSnoozeButtonClick()
        {
            try
            {
                if (_supplementPageViewModel != null)
                {
                    await AddFortyFiveMinutesSnoozeDose();
                }
                else if (_supplementPageViewModels != null)
                {
                    await AddFortyFiveMinutesSnoozeDoses();
                }
                else
                {
                    //Error
                    ShowAlertBottomSheet("Add 45 minutes Snooze", "An error occurred while adding 45 minutes of time.", "OK");
                }
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 45 minutes Snooze", "An error occurred while adding 45 minutes of time.", "OK");
            }
            finally
            {

            }
        }

        private async void HandleOneHourSnoozeButtonClick()
        {
            try
            {
                if (_supplementPageViewModel != null)
                {
                    await AddOneHourSnoozeDose();
                }
                else if (_supplementPageViewModels != null)
                {
                    await AddOneHourSnoozeDoses();
                }
                else
                {
                    //Error
                    ShowAlertBottomSheet("Add 1 hour Snooze", "An error occurred while adding 1 hour of time.", "OK");
                }
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 1 hour Snooze", "An error occurred while adding 1 hour of time.", "OK");
            }
            finally
            {

            }
        }

        private async void HandleCustomSnoozeButtonClick()
        {
            int hour = 0;
            int waitMinute = 0;
            try
            {
                string result = await App.Current.MainPage.DisplayPromptAsync("Snooze", "Snooze for (Enter in hours)");
                if (result != null)
                {
                    bool isNumber = int.TryParse(result, out hour);

                    if (isNumber == true)
                    {
                        waitMinute = hour * 60;

                        if (_supplementPageViewModel != null)
                        {
                            await AddCustomHourSnoozeDose(hour, waitMinute);
                        }
                        else if (_supplementPageViewModels != null)
                        {
                            await AddCustomHourSnoozeDoses(hour, waitMinute);
                        }
                        else
                        {
                            //Error
                            ShowAlertBottomSheet("Add Custom Snooze",
                           string.Format("An error occurred while adding {0} hour of time.", hour), "OK");
                        }
                    }
                    else
                    {
                        ShowAlertBottomSheet("Add Custom Snooze", "Enter a valid number", "OK");
                    }
                }
                else
                {

                }
            }
            catch(Exception ex)
            {
                ShowAlertBottomSheet(string.Format("Add Custom Snooze"),
                           string.Format("An error occurred while adding {0} hour of time.", hour), "OK");
            }
            finally
            {

            }          
        }

        private async Task AddThirtyMinutesSnoozeDose()
        {
            bool isSnoozeSuccessfully = false;
            int waitMinute = 30;
            string text = "Successfully snooze for 30 minutes.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);

                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);

                    IToast toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token);

                    //await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Successfully snooze for 30 minutes.", "OK");
                }
                else
                {
                    ShowAlertBottomSheet("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
            }
            finally
            {

            }
        }

        private async Task AddThirtyMinutesSnoozeDoses()
        {
            bool isSnoozeSuccessfully = false;
            bool isSnoozeAllSuccessfully = true;
            int waitMinute = 30;
            string text = "Successfully snooze for 30 minutes.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (_supplementPageViewModels.Count > 0)
                {
                    foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
                    {
                        isSnoozeSuccessfully = false;
                        isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

                        if (isSnoozeSuccessfully == true)
                        {
                            SnoozeSupplementPageViewModel(supplementPageViewModel, waitMinute);
                        }
                        else
                        {
                            isSnoozeAllSuccessfully = false;
                            break;
                        }
                    }

                    if (isSnoozeAllSuccessfully == true)
                    {
                        InvokeSuccessEventCallback(null, waitMinute);

                        
                        IToast toast = Toast.Make(text, duration, fontSize);
                        await toast.Show(cancellationTokenSource.Token);

                        //await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Successfully snooze for 30 minutes.", "OK");
                    }
                    else
                    {
                        ShowAlertBottomSheet("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
            }
            finally
            {

            }

        }

        private async Task AddFortyFiveMinutesSnoozeDose()
        {
            bool isSnoozeSuccessfully = false;
            int waitMinute = 45;
            string text = "Successfully snooze for 45 minutes.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);

                    IToast toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token);
                    //await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "Successfully snooze for 45 minutes.", "OK");
                }
                else
                {
                    ShowAlertBottomSheet("Add 45 minutes Snooze", "Failed to snooze for 45 minutes.", "OK");
                }


                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 45 minutes Snooze", "Failed to snooze for 45 minutes.", "OK");
            }
            finally
            {

            }
        }

        private async Task AddFortyFiveMinutesSnoozeDoses()
        {
            bool isSnoozeSuccessfully = false;
            bool isSnoozeAllSuccessfully = true;
            int waitMinute = 45;
            string text = "Successfully snooze for 45 minutes.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (_supplementPageViewModels.Count > 0)
                {
                    foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
                    {
                        isSnoozeSuccessfully = false;
                        isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

                        if (isSnoozeSuccessfully == true)
                        {
                            SnoozeSupplementPageViewModel(supplementPageViewModel, waitMinute);
                        }
                        else
                        {
                            isSnoozeAllSuccessfully = false;
                            break;
                        }
                    }

                    if (isSnoozeAllSuccessfully == true)
                    {
                        InvokeSuccessEventCallback(null, waitMinute);
                        IToast toast = Toast.Make(text, duration, fontSize);
                        await toast.Show(cancellationTokenSource.Token);
                        //await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "Successfully snooze for 45 minutes.", "OK");
                    }
                    else
                    {
                        ShowAlertBottomSheet("Add 45 minutes Snooze", "Failed to snooze for 45 minutes.", "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 45 minutes Snooze", "Failed to snooze for 45 minutes.", "OK");
            }
            finally
            {

            }
        }

        private async Task AddOneHourSnoozeDose()
        {
            bool isSnoozeSuccessfully = false;
            int waitMinute = 60;
            string text = "Successfully snooze for 1 hour.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);

                    IToast toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token);
                    //await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Successfully snooze for 1 hour.", "OK");
                }
                else
                {
                    ShowAlertBottomSheet("Add 1 hour Snooze", "Failed to snooze for 1 hour.", "OK");
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 1 hour Snooze", "Failed to snooze for 1 hour.", "OK");
            }
            finally
            {

            }
        }

        private async Task AddOneHourSnoozeDoses()
        {
            bool isSnoozeSuccessfully = false;
            bool isSnoozeAllSuccessfully = true;
            int waitMinute = 60;
            string text = "Successfully snooze for 1 hour.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (_supplementPageViewModels.Count > 0)
                {
                    foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
                    {
                        isSnoozeSuccessfully = false;
                        isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

                        if (isSnoozeSuccessfully == true)
                        {
                            SnoozeSupplementPageViewModel(supplementPageViewModel, waitMinute);
                        }
                        else
                        {
                            isSnoozeAllSuccessfully = false;
                            break;
                        }
                    }

                    if (isSnoozeAllSuccessfully == true)
                    {
                        InvokeSuccessEventCallback(null, waitMinute);
                        IToast toast = Toast.Make(text, duration, fontSize);
                        await toast.Show(cancellationTokenSource.Token);
                        //await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Successfully snooze for 1 hour.", "OK");
                    }
                    else
                    {
                        ShowAlertBottomSheet("Add 1 hour Snooze", "Failed to snooze for 1 hour", "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add 1 hour Snooze", "Failed to snooze for 1 hour", "OK");
            }
            finally
            {

            }
        }

        private async Task AddCustomHourSnoozeDose(int hour, int waitMinute)
        {
            bool isSnoozeSuccessfully = false;
            string text = string.Format("Successfully snooze for {0} hour.", hour);
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);

                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);
                    IToast toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token);
               //     await App.Current.MainPage.DisplayAlert("Add Custom Snooze",
               //string.Format("Successfully snooze for {0} hour.", hour), "OK");
                }
                else
                {
                    ShowAlertBottomSheet("Add Custom Snooze",
                string.Format("Failed to snooze for {0} hour.", hour), "OK");
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add Custom Snooze",
                 string.Format("Failed to snooze for {0} hour.", hour), "OK");
            }
            finally
            {

            }
        }

        private async Task AddCustomHourSnoozeDoses(int hour, int waitMinute)
        {
            bool isSnoozeSuccessfully = false;
            bool isSnoozeAllSuccessfully = true;
            string text = string.Format("Successfully snooze for {0} hour.", hour);
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (_supplementPageViewModels.Count > 0)
                {
                    foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
                    {
                        isSnoozeSuccessfully = false;
                        isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

                        if (isSnoozeSuccessfully == true)
                        {
                            SnoozeSupplementPageViewModel(supplementPageViewModel, waitMinute);
                        }
                        else
                        {
                            isSnoozeAllSuccessfully = false;
                            break;
                        }
                    }

                    if (isSnoozeAllSuccessfully == true)
                    {
                        InvokeSuccessEventCallback(null, waitMinute);
                        IToast toast = Toast.Make(text, duration, fontSize);
                        await toast.Show(cancellationTokenSource.Token);
              //          await App.Current.MainPage.DisplayAlert("Add Custom Snooze",
              //string.Format("Successfully snooze for {0} hour.", hour), "OK");
                    }
                    else
                    {
                        ShowAlertBottomSheet("Add Custom Snooze",
               string.Format("Failed to snooze for {0} hour.", hour), "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add Custom Snooze",
               string.Format("Failed to snooze for {0} hour.", hour), "OK");
            }
            finally
            {

            }

        }
        
        private void SnoozeSupplementPageViewModel(SupplementPageViewModel supplementPageViewModel, int snoozeInMinutes)
        {
            supplementPageViewModel.ScheduledTime = supplementPageViewModel.ScheduledTime.AddMinutes(snoozeInMinutes);
            supplementPageViewModel.IsSnoozed = true;
            supplementPageViewModel.SnoozedTimeMinutes = snoozeInMinutes;
        }

        private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
            }
        }

        #endregion

        #region [Fields :: Public]

        [Parameter]
        public EventCallback<Task> OnCloseClickCallback { get; set; }

        [Parameter]
        public EventCallback<int> SuccessEventCallback { get; set; }

        [Parameter]
        public EventCallback<long?> GetSupplementIdEventCallback { get; set; }

        [Parameter]
        public SupplementPageViewModel SupplementPageViewModel { get; set; }

        [Parameter]
        public List<SupplementPageViewModel> SupplementPageViewModels { get; set; }

        [Parameter]
        public bool Hidden { get; set; }

        #endregion
    }
}
