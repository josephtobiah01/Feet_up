using FeedApi.Net7.Models;
using MauiApp1.Areas.Supplement.ViewModels;
using MauiApp1.Business;
using Microsoft.AspNetCore.Components;
using ParentMiddleWare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages.Supplement
{
    public partial class ListUpdateSupplementItem
    {
        #region [Fields]

        private SupplementPageViewModel _supplementPageViewModel { get; set; }

        private bool _isSnoozeClick = false;
        private bool _isCheckBoxClick = false;

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
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private void SupplementItem_Click()
        {
            ViewSupplementPage();
        }

        private void Checkbox_Click()
        {
            _isCheckBoxClick = true;
            HandleCheckboxOnChange();
        }

        private void Checkbox_OnChanged()
        {
            HandleCheckboxOnChange();
        }

        private void SnoozeButton_Click()
        {
            ViewSnoozePage();
        }

        #endregion

        #region [Methods :: Tasks]

        private string GetFormattedScheduleTime(DateTime scheduleDateTime,bool isSnooze)
        {
            string elapsed = string.Empty;

            string scheduleTime = scheduleDateTime.ToString("hh:mm");
            string snoozeFormattedTime = string.Empty;

            switch (isSnooze)
            {
                case true:
                    snoozeFormattedTime = GetSnoozeFormattedTime(_supplementPageViewModel.SnoozedTimeMinutes);
                    elapsed = string.Format("Snoozed for {0} ({1})", snoozeFormattedTime, scheduleTime);
                    break;

                case false:

                    elapsed = string.Format("{0}", scheduleTime);
                    break;
            }
            //elapsed = string.Format("{0}", scheduleTime);

            return elapsed;
        }

        private string GetSnoozeFormattedTime(int SnoozedTimeMinutes)
        {
            string snoozeTime = string.Empty;
            //decimal hours = 0;
            //decimal minutes = 0;

            TimeSpan time = TimeSpan.FromMinutes(SnoozedTimeMinutes);

            if (time.Hours > 0)
            {
                if (time.Minutes > 0)
                {
                    snoozeTime = string.Format("{0} hour and {1} minutes", time.Hours, time.Minutes);

                }
                else
                {
                    snoozeTime = string.Format("{0} hour", time.Hours);
                }
            }
            else
            {
                snoozeTime = string.Format("{0} minutes", time.Minutes);
            }

            //if(SnoozedTimeMinutes < 60)
            //{
            //    minutes = SnoozedTimeMinutes;
            //    snoozeTime = string.Format("{0} minutes", minutes);
            //}
            //else
            //{
            //    hours = SnoozedTimeMinutes / 60;
            //    minutes = SnoozedTimeMinutes % 60;

            //    if (minutes > 0)
            //    {
            //        snoozeTime = string.Format("{0} hours and {1} minutes", hours, minutes);
            //    }
            //    else
            //    {
            //        snoozeTime = string.Format("{0} hours", hours);
            //    }                
            //}


            return snoozeTime;
        }

        private string GetFormattedFromToTime(DateTime fromDateTime, DateTime toDateTime)
        {
            string elapsed = string.Empty;

            string fromTime = fromDateTime.ToString("hh:mm");
            string toTime = toDateTime.ToString("hh:mm");

            elapsed = string.Format("{0} to {1}", fromTime, toTime);

            return elapsed;
        }

        private async void ViewSupplementPage()
        {
            if (ViewSupplementOnClickCallback.HasDelegate == true)
            {
                if (_isSnoozeClick == false && _isCheckBoxClick == false)
                {
                    await ViewSupplementOnClickCallback.InvokeAsync();
                }
                else
                {
                    _isSnoozeClick = false;
                    _isCheckBoxClick = false;
                }

            }
            else
            {
                ShowAlertBottomSheet("View Supplement Page", "ViewSupplementOnClickCallback is not been set", "OK");
            }

        }

        private async void HandleCheckboxOnChange()
        {
            try
            {
                switch (_supplementPageViewModel.IsDone)
                {
                    case true:

                        //_supplementPageViewModel.IsDone = false;
                        await AddUndoCompletedStatusTakeDose();
                        break;

                    case false:

                        //_supplementPageViewModel.IsDone = true;
                        await AddCompleteStatusTakeDose();
                        break;
                }

                if (OnCheckCallback.HasDelegate == true)
                {
                    await OnCheckCallback.InvokeAsync(_supplementPageViewModel);
                }
            }
            catch (Exception ex)
            {
                ShowAlertBottomSheet("Add Supplement Mark Done Status",
                   "An error occurred while adding done status in supplement.", "OK");
            }
            finally
            {

            }

        }

        private async Task AddCompleteStatusTakeDose()
        {
            bool isDoseTaken = false;

            try
            {
                isDoseTaken = await SupplementApi.Net7.SupplementApi.TakeDose(_supplementPageViewModel.SupplementDoseId, _supplementPageViewModel.UnitCount);

                switch (isDoseTaken)
                {
                    case true:

                        _supplementPageViewModel.IsDone = true;
                        break;

                    case false:

                        //_supplementPageViewModel.IsDone = false;
                        throw new Exception("Failed to complete the status of the supplement.");
                        break;
                }
            }
            catch (Exception ex)
            {
                App.alertBottomSheetManager.ShowAlertMessage("Error", "Failed to complete the status of the supplement.", "OK");
            }
            finally
            {

            }
        }

        private async Task AddUndoCompletedStatusTakeDose()
        {
            bool isDoseTakenUndo = false;
            try
            {
                isDoseTakenUndo = await SupplementApi.Net7.SupplementApi.TakeDoseUndo(_supplementPageViewModel.SupplementDoseId);

                switch (isDoseTakenUndo)
                {
                    case true:

                        _supplementPageViewModel.IsDone = false;
                        break;

                    case false:

                        //_supplementPageViewModel.IsDone = true;
                        throw new Exception("Failed to undo the status of the supplement.");
                        break;
                }
            }
            catch (Exception ex)
            {

                App.alertBottomSheetManager.ShowAlertMessage("Error", "Failed to undo the status of the supplement", "OK");
            }
            finally
            {

            }
        }

        private async void ViewSnoozePage()
        {
            if (SnoozeButtonOnClickCallback.HasDelegate == true)
            {
                _isSnoozeClick = true;
                await SnoozeButtonOnClickCallback.InvokeAsync();
            }
            else
            {
                ShowAlertBottomSheet("View Snooze Page", "SnoozeButtonOnClickCallback is not been set", "OK");
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

        #region [Fields :: Public]

        [Parameter]
        public EventCallback<SupplementPageViewModel> OnClickCallback { get; set; }

        [Parameter]
        public EventCallback<Task> SnoozeButtonOnClickCallback { get; set; }

        [Parameter]
        public EventCallback<Task> ViewSupplementOnClickCallback { get; set; }

        [Parameter]
        public SupplementPageViewModel SupplementPageViewModel { get; set; }

        [Parameter]
        public EventCallback<SupplementPageViewModel> OnCheckCallback { get; set; }

        #endregion
    }
}
