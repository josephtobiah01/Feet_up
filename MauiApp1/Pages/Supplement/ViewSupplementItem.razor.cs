using MauiApp1.Areas.Supplement.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MauiApp1.Pages.Supplement
{
    public partial class ViewSupplementItem
    {

        #region [Fields]

        //For Ui Fields
        private bool _isBlackCoverDivHidden = true;
        private bool _isSnoozeSupplementPageHidden = true;
        private bool _isSnoozeButtonHidden = false;
        private string _displaySkipPopup = "none";


        private bool _isSnoozeSuccess = false;
        private long? _doseId = null;
        private int _waitMinutes = 0;


        private SupplementPageViewModel _supplementPageViewModel { get; set; }

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
            //_isSnoozeButtonHidden = this.SnoozeButtonHidden;
            //_isSnoozeButtonHidden = _supplementPageViewModel.IsSnoozed;
            if(this.SnoozeButtonHidden == true)
            {
                _isSnoozeButtonHidden = this.SnoozeButtonHidden;
            }
            else
            {
                _isSnoozeButtonHidden = _supplementPageViewModel.IsSnoozed;
            }
            
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private void CloseButton_Click()
        {
            ClosePage();
        }

        private void CloseSnoozeContainerDiv_Click()
        {
            CloseSnoozeDivToggle();
        }

        private void SnoozeButton_Click(SupplementPageViewModel supplementPageViewModel)
        {
            ViewSnoozePage(supplementPageViewModel);
        }

        private void SkipButton_Click()
        {
            _displaySkipPopup = "inline";
        }

        private void SnoozeDialogIsSuccess_EventCallback(int waitMinutes)
        {
            _isSnoozeSuccess = true;
            _waitMinutes = waitMinutes;
        }

        private void GetUpdatedSupplement_EventCallback(long? doseId)
        {
            _doseId = doseId;
        }

        #endregion

        #region [Methods :: Tasks]

        private async void ClosePage()
        {
            if (OnCloseClickCallback.HasDelegate == true)
            {
                if(_isSnoozeSuccess == true)
                {
                    InvokeSuccessEventCallback(_doseId, _waitMinutes);
                }

                await OnCloseClickCallback.InvokeAsync();
            }
            else
            {
                ShowAlertBottomSheet("Close Supplement Item Page", "OnCloseClickCallback is not been set", "OK");
            }
        }

        private void CloseSnoozeDivToggle()
        {
            if (_isBlackCoverDivHidden == true)
            {
                _isBlackCoverDivHidden = false;
                _isSnoozeSupplementPageHidden = false;
            }
            else
            {
                _isBlackCoverDivHidden = true;
                _isSnoozeSupplementPageHidden = true;
            }
        }

        private void ViewSnoozePage(SupplementPageViewModel supplementPageViewModel)
        {
            _isBlackCoverDivHidden = false;
            _isSnoozeSupplementPageHidden = false;

            _supplementPageViewModel = supplementPageViewModel;

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
        public SupplementPageViewModel SupplementPageViewModel { get; set; }

        [Parameter]
        public bool Hidden { get; set; }

        [Parameter]
        public bool SnoozeButtonHidden { get; set; }

        [Parameter]
        public EventCallback<int> SuccessEventCallback { get; set; }

        [Parameter]
        public EventCallback<long?> GetSupplementIdEventCallback { get; set; }

        #endregion

    }
}
