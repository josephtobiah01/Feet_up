using MauiApp1.Areas.Supplement.ViewModels;
using Microsoft.AspNetCore.Components;

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
                await App.Current.MainPage.DisplayAlert("Close Snooze Page", "OnCloseClickCallback is not been set", "OK");
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
                    await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "An error occurred while adding 30 minutes of time.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "An error occurred while adding 30 minutes of time.", "OK");
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
                    await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "An error occurred while adding 45 minutes of time.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "An error occurred while adding 45 minutes of time.", "OK");
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
                    await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "An error occurred while adding 1 hour of time.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "An error occurred while adding 1 hour of time.", "OK");
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
                            await App.Current.MainPage.DisplayAlert("Add Custom Snooze",
                           string.Format("An error occurred while adding {0} hour of time.", hour), "OK");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Add Custom Snooze", "Enter a valid number", "OK");
                    }
                }
                else
                {

                }
            }
            catch(Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(string.Format("Add Custom Snooze"),
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

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);

                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);
                    await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Successfully snooze for 30 minutes.", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
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
                        await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Successfully snooze for 30 minutes.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }

        }

        private async Task AddFortyFiveMinutesSnoozeDose()
        {
            bool isSnoozeSuccessfully = false;
            int waitMinute = 45;

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);

                    await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "Successfully snooze for 45 minutes.", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "Failed to snooze for 45 minutes.", "OK");
                }


                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
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
                        await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "Successfully snooze for 45 minutes.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Add 45 minutes Snooze", "Failed to snooze for 45 minutes.", "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }

        private async Task AddOneHourSnoozeDose()
        {
            bool isSnoozeSuccessfully = false;
            int waitMinute = 60;

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);

                    await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Successfully snooze for 1 hour.", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Failed to snooze for 1 hour.", "OK");
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
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
                        await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Successfully snooze for 1 hour.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Failed to snooze for 1 hour", "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }

        private async Task AddCustomHourSnoozeDose(int hour, int waitMinute)
        {
            bool isSnoozeSuccessfully = false;

            try
            {
                isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

                if (isSnoozeSuccessfully == true)
                {
                    SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);

                    InvokeSuccessEventCallback(_supplementPageViewModel.SupplementDoseId, waitMinute);

                    await App.Current.MainPage.DisplayAlert("Add Custom Snooze",
               string.Format("Successfully snooze for {0} hour.", hour), "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Add Custom Snooze",
                string.Format("Failed to snooze for {0} hour.", hour), "OK");
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }

        private async Task AddCustomHourSnoozeDoses(int hour, int waitMinute)
        {
            bool isSnoozeSuccessfully = false;
            bool isSnoozeAllSuccessfully = true;
          
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

                        await App.Current.MainPage.DisplayAlert("Add Custom Snooze",
              string.Format("Successfully snooze for {0} hour.", hour), "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Add Custom Snooze",
               string.Format("Failed to snooze for {0} hour.", hour), "OK");
                    }
                }

                ClosePage();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }

        }

        // TO BE DELETED
        //private async void AddThirtyMinutesSnoozeTime()
        //{
        //    bool isSnoozeSuccessfully = false;
        //    bool isSnoozeAllSuccessfully = true;

        //    int waitMinute = 30;

        //    try
        //    {
        //        if (_supplementPageViewModel != null)
        //        {
        //            isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

        //            if (isSnoozeSuccessfully == true)
        //            {
        //                //_supplementPageViewModel.ScheduledTime = _supplementPageViewModel.ScheduledTime.AddMinutes(waitMinute);
        //                //_supplementPageViewModel.IsSnoozed= true;
        //                SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);

        //                InvokeSuccessEventCallback(_supplementPageViewModel.SupplementId, waitMinute);
        //                await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Successfully snooze for 30 minutes.", "OK");
        //            }
        //            else
        //            {
        //                await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
        //            }

        //            ClosePage();
        //        }
        //        else if (_supplementPageViewModels != null)
        //        {
        //            if (_supplementPageViewModels.Count > 0)
        //            {
        //                foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
        //                {
        //                    isSnoozeSuccessfully = false;
        //                    //supplementPageViewModel.ScheduledTime = supplementPageViewModel.ScheduledTime.AddMinutes(30);
        //                    isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

        //                    if (isSnoozeSuccessfully == true)
        //                    {
        //                        //supplementPageViewModel.ScheduledTime = supplementPageViewModel.ScheduledTime.AddMinutes(30);
        //                        SnoozeSupplementPageViewModel(supplementPageViewModel, waitMinute);
        //                    }
        //                    else
        //                    {
        //                        isSnoozeAllSuccessfully = false;
        //                        break;
        //                    }
        //                }

        //                if (isSnoozeAllSuccessfully == true)
        //                {
        //                    InvokeSuccessEventCallback(null, waitMinute);
        //                    await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Successfully snooze for 30 minutes.", "OK");
        //                }
        //                else
        //                {
        //                    await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "Failed to snooze for 30 minutes.", "OK");
        //                }
        //            }

        //            ClosePage();
        //        }
        //        else
        //        {
        //            //Error
        //            await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "An error occurred while adding 30 minutes of time.", "OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await App.Current.MainPage.DisplayAlert("Add 30 minutes Snooze", "An error occurred while adding 30 minutes of time.", "OK");
        //    }
        //    finally
        //    {

        //    }

        //}

        //private async void AddFortyMinutesSnoozeTime()
        //{
        //    bool isSnoozeSuccessfully = false;
        //    bool isSnoozeAllSuccessfully = true;

        //    int waitMinute = 40;

        //    try
        //    {
        //        if (_supplementPageViewModel != null)
        //        {
        //            isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

        //            if (isSnoozeSuccessfully == true)
        //            {
        //                //_supplementPageViewModel.ScheduledTime = _supplementPageViewModel.ScheduledTime.AddMinutes(waitMinute);
        //                SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
        //                InvokeSuccessEventCallback(_supplementPageViewModel.SupplementId, waitMinute);

        //                await App.Current.MainPage.DisplayAlert("Add 40 minutes Snooze", "Successfully snooze for 40 minutes.", "OK");
        //            }
        //            else
        //            {
        //                await App.Current.MainPage.DisplayAlert("Add 40 minutes Snooze", "Failed to snooze for 40 minutes.", "OK");
        //            }


        //            ClosePage();
        //        }
        //        else if (_supplementPageViewModels != null)
        //        {
        //            if (_supplementPageViewModels.Count > 0)
        //            {
        //                foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
        //                {
        //                    isSnoozeSuccessfully = false;
        //                    isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

        //                    if (isSnoozeSuccessfully == true)
        //                    {
        //                        //supplementPageViewModel.ScheduledTime = supplementPageViewModel.ScheduledTime.AddMinutes(40);
        //                        SnoozeSupplementPageViewModel(supplementPageViewModel, waitMinute);
        //                    }
        //                    else
        //                    {
        //                        isSnoozeAllSuccessfully = false;
        //                        break;
        //                    }
        //                }

        //                if (isSnoozeAllSuccessfully == true)
        //                {
        //                    InvokeSuccessEventCallback(null, waitMinute);
        //                    await App.Current.MainPage.DisplayAlert("Add 40 minutes Snooze", "Successfully snooze for 40 minutes.", "OK");
        //                }
        //                else
        //                {
        //                    await App.Current.MainPage.DisplayAlert("Add 40 minutes Snooze", "Failed to snooze for 40 minutes.", "OK");
        //                }
        //            }

        //            ClosePage();
        //        }
        //        else
        //        {
        //            //Error
        //            await App.Current.MainPage.DisplayAlert("Add 40 minutes Snooze", "An error occurred while adding 40 minutes of time.", "OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await App.Current.MainPage.DisplayAlert("Add 40 minutes Snooze", "An error occurred while adding 40 minutes of time.", "OK");
        //    }
        //    finally
        //    {

        //    }
        //}

        //private async void AddOneHourSnoozeTime()
        //{

        //    bool isSnoozeSuccessfully = false;
        //    bool isSnoozeAllSuccessfully = true;

        //    int waitMinute = 60;

        //    try
        //    {
        //        if (_supplementPageViewModel != null)
        //        {
        //            isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

        //            if (isSnoozeSuccessfully == true)
        //            {
        //                _supplementPageViewModel.ScheduledTime = _supplementPageViewModel.ScheduledTime.AddMinutes(waitMinute);

        //                //InvokeSuccessEventCallback(_supplementPageViewModel.SupplementId, waitMinute);
        //                SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
        //                await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Successfully snooze for 1 hour.", "OK");
        //            }
        //            else
        //            {
        //                await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Failed to snooze for 1 hour.", "OK");
        //            }

        //            ClosePage();
        //        }
        //        else if (_supplementPageViewModels != null)
        //        {
        //            if (_supplementPageViewModels.Count > 0)
        //            {
        //                foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
        //                {
        //                    isSnoozeSuccessfully = false;
        //                    isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

        //                    if (isSnoozeSuccessfully == true)
        //                    {
        //                        //supplementPageViewModel.ScheduledTime = supplementPageViewModel.ScheduledTime.AddMinutes(waitMinute);
        //                        SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
        //                    }
        //                    else
        //                    {
        //                        isSnoozeAllSuccessfully = false;
        //                        break;
        //                    }
        //                }

        //                if (isSnoozeAllSuccessfully == true)
        //                {
        //                    InvokeSuccessEventCallback(null, waitMinute);
        //                    await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Successfully snooze for 1 hour.", "OK");
        //                }
        //                else
        //                {
        //                    await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "Failed to snooze for 1 hour", "OK");
        //                }
        //            }

        //            ClosePage();
        //        }
        //        else
        //        {
        //            //Error
        //            await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "An error occurred while adding 1 hour of time.", "OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await App.Current.MainPage.DisplayAlert("Add 1 hour Snooze", "An error occurred while adding 1 hour of time.", "OK");
        //    }
        //    finally
        //    {

        //    }
        //}

        //private async void AddCustomeSnoozeTime()
        //{
        //    int hour = 0;
        //    int waitMinute = 0;
        //    bool isSnoozeSuccessfully = false;
        //    bool isSnoozeAllSuccessfully = true;
        //    string result = await App.Current.MainPage.DisplayPromptAsync("Snooze", "Snooze for (Enter in hours)");
        //    if (result != null)
        //    {
        //        bool isNumber = int.TryParse(result, out hour);

        //        if (isNumber == true)
        //        {
        //            try
        //            {
        //                waitMinute = hour * 60;

        //                if (_supplementPageViewModel != null)
        //                {
        //                    isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(_supplementPageViewModel.SupplementDoseId, waitMinute);

        //                    if (isSnoozeSuccessfully == true)
        //                    {

        //                        //_supplementPageViewModel.ScheduledTime = _supplementPageViewModel.ScheduledTime.AddMinutes(waitMinute);
        //                        SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);

        //                        InvokeSuccessEventCallback(_supplementPageViewModel.SupplementId, waitMinute);

        //                        await App.Current.MainPage.DisplayAlert(string.Format("Add {0} hour Snooze", hour),
        //                   string.Format("Successfully snooze for {0} hour.", hour), "OK");
        //                    }
        //                    else
        //                    {
        //                        await App.Current.MainPage.DisplayAlert(string.Format("Add {0} hour Snooze", hour),
        //                    string.Format("Failed to snooze for {0} hour.", hour), "OK");
        //                    }

        //                    ClosePage();
        //                }
        //                else if (_supplementPageViewModels != null)
        //                {
        //                    if (_supplementPageViewModels.Count > 0)
        //                    {
        //                        foreach (SupplementPageViewModel supplementPageViewModel in _supplementPageViewModels)
        //                        {
        //                            isSnoozeSuccessfully = false;
        //                            isSnoozeSuccessfully = await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementPageViewModel.SupplementDoseId, waitMinute);

        //                            if (isSnoozeSuccessfully == true)
        //                            {
        //                                //supplementPageViewModel.ScheduledTime = supplementPageViewModel.ScheduledTime.AddMinutes(waitMinute);
        //                                SnoozeSupplementPageViewModel(_supplementPageViewModel, waitMinute);
        //                            }
        //                            else
        //                            {
        //                                isSnoozeAllSuccessfully = false;
        //                                break;
        //                            }
        //                        }

        //                        if (isSnoozeAllSuccessfully == true)
        //                        {
        //                            InvokeSuccessEventCallback(null, waitMinute);

        //                            await App.Current.MainPage.DisplayAlert(string.Format("Add {0} hour Snooze", hour),
        //                  string.Format("Successfully snooze for {0} hour.", hour), "OK");
        //                        }
        //                        else
        //                        {
        //                            await App.Current.MainPage.DisplayAlert(string.Format("Add {0} hour Snooze", hour),
        //                   string.Format("Failed to snooze for {0} hour.", hour), "OK");
        //                        }
        //                    }

        //                    ClosePage();
        //                }
        //                else
        //                {
        //                    //Error
        //                    await App.Current.MainPage.DisplayAlert(string.Format("Add {0} hour Snooze", hour),
        //                   string.Format("An error occurred while adding {0} hour of time.", hour), "OK");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                await App.Current.MainPage.DisplayAlert(string.Format("Add {0} hour Snooze", hour),
        //                   string.Format("An error occurred while adding {0} hour of time.", hour), "OK");
        //            }
        //            finally
        //            {

        //            }
        //        }
        //        else
        //        {
        //            await App.Current.MainPage.DisplayAlert("Add Custom Snooze", "Enter a valid number", "OK");
        //        }
        //    }
        //    else
        //    {

        //    }
        //}

        
        private void SnoozeSupplementPageViewModel(SupplementPageViewModel supplementPageViewModel, int snoozeInMinutes)
        {
            supplementPageViewModel.ScheduledTime = supplementPageViewModel.ScheduledTime.AddMinutes(snoozeInMinutes);
            supplementPageViewModel.IsSnoozed = true;
            supplementPageViewModel.SnoozedTimeMinutes = snoozeInMinutes;
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
