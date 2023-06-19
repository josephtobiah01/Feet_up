using FeedApi.Net7.Models;
using MauiApp1.Areas.Supplement.ViewModels;
using MauiApp1.Areas.Supplement.Views;
using MauiApp1.Business.DeviceServices;
using Microsoft.AspNetCore.Components;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using SupplementFeedItemViewModel = MauiApp1.Areas.Supplement.ViewModels.SupplementFeedItemViewModel;

namespace MauiApp1.Pages.Supplement
{
    public partial class ViewFeedItemSupplementPage
    {
        #region [Fields]

        //For Ui Fields
        private bool _isSupplementsToTakeTabDiv = false;
        private bool _isAllSupplementsTabDiv = true;
        private bool _isBlackCoverDivHidden = true;
        private bool _isViewSupplementItemHidden = true;
        private bool _isSnoozeSupplementPageHidden = true;
        private bool _isSnoozeMenuHidden = false;
        //private bool _loadFeedItem = true;

        private bool _isSnoozeSuccess = false;
        private long? _doseId = null;

        private int _waitMinutes = 0;

        private FeedItem _feedItem { get; set; }

        private SupplementFeedItemViewModel _supplementFeedItemViewModel { get; set; }

        private SupplementPageViewModel _supplementPageViewModel { get; set; }
        private List<SupplementPageViewModel> _supplementPageViewModels { get; set; }

        //private double _sheetHeight = 492;

        private string _markAllDoneText = "Mark all done";
        #endregion


        #region [Methods :: EventHandlers :: Class]

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (this.ShowFeedItemDetail == false)
            {
                IntializeData();
                InitializeControl();
            }
        }

        private void IntializeData()
        {
            //_loadFeedItem = this.ReloadFeedItemDetail;

            if (this.ReloadFeedItemDetail == true)
            {
                _feedItem = this.FeedItem;

                LoadSupplementFeedItem();

                InvokeReloadPageEventCallbeack(false);
                this.ReloadFeedItemDetail = false;
            }            
        }

        private void InitializeControl()
        {
            //_sheetHeight = (HTMLBridge.BrowserInnerHeight / 100) * 50;
            UpateMarkAllDoneLabelText();
            SelectSupplementToTakeTabItem();
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private void AddSupplementButton_Click()
        {
            ViewAddSupplementPage();
        }

        private void MarkAllDoneButton_Click()
        {
            HandleMarkAllDoneButtonClick();
        }

        private void CheckboxEvent_CallBack(SupplementPageViewModel supplement)
        {
            HandleCheckboxChange(supplement);
        }

        private void SupplementToTakeTabItem_Click()
        {
            SelectSupplementToTakeTabItem();
        }

        private void AllSupplementTabItem_Click()
        {
            SelectAllSupplementTabItem();
        }

        private void BlackCoverDiv_Click()
        {
            CloseDivToggle();
        }

        private void CloseSnoozeContainerDiv_Click()
        {
            CloseDivToggle();
        }

        private void CloseSupplementDetailsButton_Click()
        {
            CloseDivToggle();
        }

        private void BackToFeedButton_Click()
        {
            ClosePage();
        }

        private void ViewSupplementItem_Click(SupplementPageViewModel supplementPageViewModel, bool isSnoozeMenuHidden)
        {
            ViewSupplement(supplementPageViewModel, isSnoozeMenuHidden);
        }

        private void SnoozeButton_Click(SupplementPageViewModel supplementPageViewModel)
        {
            ViewSnoozePage(supplementPageViewModel);
        }

        private void SnoozeAllButton_Click()
        {
            ViewSnoozeAllPage();
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

        private string GetFormattedTimeElapsed(DateTime dateTime, FeedItemStatus status)
        {
            string elapsed = string.Empty;


            if (status == FeedItemStatus.Completed)
            {
                elapsed = string.Format("Completed");
                return elapsed;
            }

            if (status == FeedItemStatus.Skipped)
            {
                elapsed = string.Format("Skipped");
                return elapsed;
            }
            else if (status == FeedItemStatus.Snoozed)
            {
                elapsed = string.Format("Snoozed ({0})",
                  dateTime.ToString("hh:mm tt"));
                return elapsed;
            }

            if (status == FeedItemStatus.Ongoing)
            {
                elapsed = string.Format("Ongoing");
                return elapsed;
            }
            DateTime dateTimeNow = DateTime.Now.AddMinutes(30);
            //TimeZoneInfo localZone = TimeZoneInfo.Local;
            //DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, localZone);


            string timeElapsed = string.Empty;

            timeElapsed = GetElapsedTime(dateTime);
            if (DateTime.Now.AddMinutes(-MiddleWare.OverDueTime) > dateTime)
            {
                elapsed = string.Format("overdue ({0})",
                             dateTime.ToString("hh:mm tt"));
            }
            else if (dateTime <= dateTimeNow)
            {
                elapsed = string.Format("in {0} ({1})", timeElapsed,
                             dateTime.ToString("hh:mm tt"));
            }
            else
            {
                elapsed = string.Format("starts at {0}", dateTime.ToString("hh:mm tt"));
            }

            //elapsed = dateTime.ToString("dddd, dd MMMM yyyy hh:mm:ss tt") + elapsed;

            return elapsed;
        }

        private string GetElapsedTime(DateTimeOffset dateTimeOffsetStarted)
        {
            string elapsed = string.Empty;

            DateTimeOffset dateTimeOffsetEnded = DateTimeOffset.Now;
            TimeSpan difference = new TimeSpan();
            Int64 ticksElapsed = 0;

            //difference = dateTimeOffsetEnded.Subtract(dateTimeOffsetStarted);
            difference = dateTimeOffsetStarted.Subtract(dateTimeOffsetEnded);
            ticksElapsed = (Int64)(difference.Ticks);


            TimeSpan elapsedTimeSpan = TimeSpan.FromTicks(ticksElapsed);

            int totalDays = (int)elapsedTimeSpan.TotalDays;

            if (totalDays >= 1)
            {
                if (totalDays == 1)
                {
                    elapsed += "1 day ";
                }
                else
                {
                    elapsed += totalDays + " days ";
                }

                elapsedTimeSpan = elapsedTimeSpan.Subtract(TimeSpan.FromDays(totalDays));
            }


            int totalHours = (int)elapsedTimeSpan.TotalHours;

            if (totalHours >= 1)
            {
                if (totalHours == 1)
                {
                    elapsed += "1 hr ";
                }
                else
                {
                    elapsed += totalHours + " hrs ";
                }

                elapsedTimeSpan = elapsedTimeSpan.Subtract(TimeSpan.FromHours(totalHours));
            }

            if ((totalDays <= 0) || (totalHours <= 0))
            {
                int totalMinutes = (int)elapsedTimeSpan.TotalMinutes;

                if (totalMinutes >= 1)
                {
                    if (totalMinutes == 1)
                    {
                        elapsed += "1 min ";
                    }
                    else
                    {
                        elapsed += totalMinutes + " mins ";
                    }

                    elapsedTimeSpan = elapsedTimeSpan.Subtract(TimeSpan.FromMinutes(totalMinutes));
                }
                else
                {
                    int totalSeconds = (int)elapsedTimeSpan.TotalSeconds;

                    if (totalSeconds >= 1)
                    {
                        if (totalSeconds == 1)
                        {
                            elapsed += "1 sec ";
                        }
                        else
                        {
                            elapsed += totalSeconds + " secs ";
                        }

                        elapsedTimeSpan = elapsedTimeSpan.Subtract(TimeSpan.FromSeconds(totalSeconds));
                    }

                }
            }


            return elapsed.Trim();
        }


        private void LoadSupplementFeedItem()
        {
            _supplementFeedItemViewModel = new SupplementFeedItemViewModel();


            _supplementFeedItemViewModel.supplements = new List<SupplementPageViewModel>();

            _supplementFeedItemViewModel.IsSupplementToTake = true;
            _supplementFeedItemViewModel.IsAllSupplement = false;

            SupplementPageViewModel supplementPageViewModel = null;

            foreach (SupplementEntry supplementEntry in this._feedItem.SupplementFeedItem.SupplementEntries)
            {
                supplementPageViewModel = new SupplementPageViewModel();
                supplementPageViewModel.SupplementId = supplementEntry.SupplementId;
                supplementPageViewModel.SupplementName = supplementEntry.Supplementname;
                supplementPageViewModel.SupplementDoseId = supplementEntry.DoseId;
                supplementPageViewModel.ScheduledTime = supplementEntry.ScheduledTime;
                supplementPageViewModel.DoseWarningLimit = supplementEntry.DoseWarningLimit;
                supplementPageViewModel.DoseHardLimit = supplementEntry.DoseHardLimit;
                supplementPageViewModel.UnitName = supplementEntry.UnitName;
                supplementPageViewModel.UnitCount = supplementEntry.UnitCount;
                supplementPageViewModel.is_Weight = supplementEntry.is_Weight;
                supplementPageViewModel.is_Volume = supplementEntry.is_Volume;
                supplementPageViewModel.is_Count = supplementEntry.is_Count;
                supplementPageViewModel.Instructions = supplementEntry.Instructions;
                supplementPageViewModel.Requires_source_of_fat = supplementEntry.Requires_source_of_fat;
                supplementPageViewModel.Take_before_sleep = supplementEntry.Take_before_sleep;
                supplementPageViewModel.Take_after_meal = supplementEntry.Take_after_meal;
                supplementPageViewModel.Take_on_empty_stomach = supplementEntry.Take_on_empty_stomach;


                supplementPageViewModel.SnoozedTimeMinutes = supplementEntry.SnoozedTimeMinutes;
                supplementPageViewModel.IsSnoozed = supplementEntry.isSnoozed;
                supplementPageViewModel.IsDone = supplementEntry.isComplete;                

                _supplementFeedItemViewModel.supplements.Add(supplementPageViewModel);
            }

            LoadAllSupplement();
        }

        private async void LoadAllSupplement()
        {
            try
            {
                _supplementFeedItemViewModel.allSupplements = new List<SupplementPageViewModel>();

                List<NdSupplementList> ndSupplementLists = await SupplementApi.Net7.SupplementApi.GetAllSupplments();

                SupplementPageViewModel supplementPageViewModel = null;

                foreach (NdSupplementList ndSupplementList in ndSupplementLists)
                {
                    supplementPageViewModel = new SupplementPageViewModel();
                    //supplementPageViewModel.SupplementId = ndSupplementList.SupplementId;
                    supplementPageViewModel.SupplementName = ndSupplementList.SupplementName;
                    supplementPageViewModel.SupplementDoseId = ndSupplementList.SupplmentDoseId;
                    //supplementPageViewModel.ScheduledTime = DateTime.ParseExact(ndSupplementList.TimeString, "H:mm", null, System.Globalization.DateTimeStyles.None);
                    supplementPageViewModel.TimeString = ndSupplementList.TimeString;
                    supplementPageViewModel.UnitName = ndSupplementList.Type;
                    supplementPageViewModel.UnitCount = ndSupplementList.Ammount;
                    supplementPageViewModel.TimeRemark = ndSupplementList.TimeRemark;
                    supplementPageViewModel.FrequencyRemark = ndSupplementList.Frequency;
                    supplementPageViewModel.Instructions = ndSupplementList.FoodRemark;

                    foreach (ParentMiddleWare.Models.DayOfWeek dayOfWeek in ndSupplementList.DayOfWeek)
                    {
                        switch (dayOfWeek)
                        {
                            case ParentMiddleWare.Models.DayOfWeek.Monday:

                                supplementPageViewModel.is_Monday = true;
                                break;

                            case ParentMiddleWare.Models.DayOfWeek.Tuesday:

                                supplementPageViewModel.is_Tuesday = true;
                                break;

                            case ParentMiddleWare.Models.DayOfWeek.Wednesday:

                                supplementPageViewModel.is_Wednesday = true;
                                break;

                            case ParentMiddleWare.Models.DayOfWeek.Thursday:

                                supplementPageViewModel.is_Thrusday = true;
                                break;

                            case ParentMiddleWare.Models.DayOfWeek.Friday:

                                supplementPageViewModel.is_Friday = true;
                                break;

                            case ParentMiddleWare.Models.DayOfWeek.Saturday:

                                supplementPageViewModel.is_Saturday = true;
                                break;

                            case ParentMiddleWare.Models.DayOfWeek.Sunday:

                                supplementPageViewModel.is_Sunday = true;
                                break;

                            default:

                                //Log Error Should Not Be Possible
                                break;
                        }
                    }


                    supplementPageViewModel.SnoozedTimeMinutes = 0;
                    supplementPageViewModel.IsSnoozed = false;
                    supplementPageViewModel.IsDone = false;

                    _supplementFeedItemViewModel.allSupplements.Add(supplementPageViewModel);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }

        private async void ViewAddSupplementPage()
        {
            if (HTMLBridge.MainPageBlackStackLayout != null)
            {
                HTMLBridge.MainPageBlackStackLayout.IsVisible = true;
            }
            if (HTMLBridge.MainPageLoadingActivityIndicator != null)
            {
                HTMLBridge.MainPageLoadingActivityIndicator.IsVisible = true;
            }

            await App.Current.MainPage.Navigation.PushAsync(new AddSupplementContentPage(), true);
        }

        private void HandleMarkAllDoneButtonClick()
        {
            InvokeUpdateParentEventCallback();

            foreach (SupplementPageViewModel supplement in _supplementFeedItemViewModel.supplements)
            {
                AddSupplementDoneStatus(supplement);
            }
            ClosePage();
            _markAllDoneText = "Mark remaining done";
        }

        private async void AddSupplementDoneStatus(SupplementPageViewModel supplement)
        {
            bool isDoseTaken = supplement.IsDone;

            try
            {
                if (!isDoseTaken)
                {
                    //supplement.IsDone = true;
                    isDoseTaken = supplement.IsDone = await SupplementApi.Net7.SupplementApi.TakeDose(supplement.SupplementDoseId, supplement.UnitCount);
                }
                switch (isDoseTaken)
                {
                    case true:

                        //  _supplementPageViewModel.IsDone = true;
                        break;

                    case false:

                        //_supplementPageViewModel.IsDone = false;
                        //  throw new Exception("Failed to complete the status of the supplement.");
                        break;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Add Supplement Mark Done Status",
                    "An error occurred while adding done status in supplement.", "OK");
            }
            finally
            {

            }

        }

        private void HandleCheckboxChange(SupplementPageViewModel supplement)
        {
            ReloadAndUpdateSupplement(supplement);
            InvokeUpdateParentEventCallback();
            UpateMarkAllDoneLabelText();
            this.StateHasChanged();
        }
        private void ReloadAndUpdateSupplement(SupplementPageViewModel supplement)
        {
            foreach (SupplementPageViewModel supplementPageViewModel in _supplementFeedItemViewModel.supplements)
            {
                if (supplement.SupplementDoseId == supplementPageViewModel.SupplementDoseId)
                {
                    supplementPageViewModel.IsDone = supplement.IsDone;
                }
            }            
        }

        private async void UpateMarkAllDoneLabelText()
        {
            bool isAllMarkDone = false;

            try
            {
                foreach (SupplementPageViewModel supplement in _supplementFeedItemViewModel.supplements)
                {
                    if (supplement.IsDone == true)
                    {
                        isAllMarkDone = true;
                        break;
                    }
                }

                if (isAllMarkDone == false)
                {
                    _markAllDoneText = "Mark all done";
                }
                else
                {
                    _markAllDoneText = "Mark remaining done";
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }

        private void SelectSupplementToTakeTabItem()
        {
            _isSupplementsToTakeTabDiv = false;
            _isAllSupplementsTabDiv = true;
            _supplementFeedItemViewModel.IsAllSupplement = false;
            _supplementFeedItemViewModel.IsSupplementToTake = true;
        }

        private void SelectAllSupplementTabItem()
        {
            _isSupplementsToTakeTabDiv = true;
            _isAllSupplementsTabDiv = false;
            _supplementFeedItemViewModel.IsAllSupplement = true;
            _supplementFeedItemViewModel.IsSupplementToTake = false;
        }

        private void CloseDivToggle()
        {
            if (_isBlackCoverDivHidden == true)
            {
                _isBlackCoverDivHidden = false;
                _isViewSupplementItemHidden = false;
                _isSnoozeSupplementPageHidden = false;
            }
            else
            {
                _isBlackCoverDivHidden = true;
                _isViewSupplementItemHidden = true;
                _isSnoozeSupplementPageHidden = true;
                _supplementPageViewModel = null;
            }

            if (_isSnoozeSuccess == true)
            {
                if (_doseId.HasValue == true)
                {
                    UpdateSupplementEntryScheduledTime();
                }
                else
                {
                    UpdateSupplementEntriesScheduledTime();
                }

                LoadSupplementFeedItem();
                UpdateFeedItemDate();
                StateHasChanged();
            }

            if (_isSnoozeSuccess == true)
            {
                InvokeUpdateParentEventCallback();
            }

        }

       

        private void ViewSupplement(SupplementPageViewModel supplementPageViewModel, bool isSnoozeMenuHidden)
        {
            _isBlackCoverDivHidden = false;
            _isViewSupplementItemHidden = false;

            _supplementPageViewModel = supplementPageViewModel;

            _isSnoozeMenuHidden = isSnoozeMenuHidden;

        }

        private void ViewSnoozeAllPage()
        {
            _isBlackCoverDivHidden = false;
            _isSnoozeSupplementPageHidden = false;

            _supplementPageViewModel = null;
            _doseId = null;
            _supplementPageViewModels = _supplementFeedItemViewModel.supplements;

        }

        private void ViewSnoozePage(SupplementPageViewModel supplementPageViewModel)
        {
            _isBlackCoverDivHidden = false;
            _isSnoozeSupplementPageHidden = false;

            _doseId = null;
            _supplementPageViewModel = supplementPageViewModel;
            _supplementPageViewModels = null;

        }

        private async void ClosePage()
        {
            if (OnCloseClickCallback.HasDelegate == true)
            {
                await OnCloseClickCallback.InvokeAsync();
                InvokeReloadPageEventCallbeack(true);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Close Snooze Page", "OnCloseClickCallback is not been set", "OK");
            }
        }

        private async void InvokeReloadPageEventCallbeack(bool reloadFeedItemDetails)
        {
            if (ReloadFeedItemDetailsPageEventCallback.HasDelegate == true)
            {
                await ReloadFeedItemDetailsPageEventCallback.InvokeAsync(reloadFeedItemDetails);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Reload Supplement Details Page", "ReloadFeedItemDetailsPageEventCallback is not been set", "OK");
            }
        }

        private async void InvokeUpdateParentEventCallback()
        {
            if (UpdateParentEventCallback.HasDelegate == true)
            {
                await UpdateParentEventCallback.InvokeAsync();
            }
            else
            {
                //await App.Current.MainPage.DisplayAlert("Close Snooze Page", "OnCloseClickCallback is not been set", "OK");
            }
        }

        private void UpdateSupplementEntryScheduledTime()
        {
            if (_doseId.HasValue == true)
            {
                foreach (SupplementEntry supplementEntry in _feedItem.SupplementFeedItem.SupplementEntries)
                {
                    if (supplementEntry.DoseId == _doseId.Value)
                    {
                        supplementEntry.ScheduledTime = supplementEntry.ScheduledTime.AddMinutes(_waitMinutes);
                        supplementEntry.isSnoozed = true;
                        supplementEntry.SnoozedTimeMinutes = _waitMinutes;
                        break;
                    }
                }
            }
        }

        private void UpdateSupplementEntriesScheduledTime()
        {
            foreach (SupplementEntry supplementEntry in _feedItem.SupplementFeedItem.SupplementEntries)
            {
                supplementEntry.ScheduledTime = supplementEntry.ScheduledTime.AddMinutes(_waitMinutes);
                supplementEntry.isSnoozed = true;
                supplementEntry.SnoozedTimeMinutes = _waitMinutes;
            }

        }

        private void UpdateFeedItemDate()
        {
            DateTime highestDateTime = DateTime.MinValue;

            foreach (SupplementPageViewModel supplementPageViewModel in _supplementFeedItemViewModel.supplements)
            {

                if (_feedItem.Date < supplementPageViewModel.ScheduledTime)
                {
                    highestDateTime = supplementPageViewModel.ScheduledTime;
                    _feedItem.Date = new DateTime(highestDateTime.Year, highestDateTime.Month, highestDateTime.Day, highestDateTime.Hour,0,0);
                   
                }
            }            
        }

        #endregion

        #region [Fields :: Public]

        [Parameter]
        public EventCallback<int> OnClickCallback { get; set; }

        [Parameter]
        public EventCallback<int> OnCloseClickCallback { get; set; }

        [Parameter]
        public EventCallback<Task> UpdateParentEventCallback { get; set; }

        [Parameter]
        public FeedItem FeedItem { get; set; }

        [Parameter]
        public bool ShowFeedItemDetail { get; set; }

        [Parameter]
        public bool ReloadFeedItemDetail { get; set; }

        [Parameter]
        public EventCallback<bool> ReloadFeedItemDetailsPageEventCallback{ get; set; }


        #endregion

    }
}
