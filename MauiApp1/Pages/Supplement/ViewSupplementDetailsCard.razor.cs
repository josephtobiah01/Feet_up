using MauiApp1.Areas.Supplement.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MauiApp1.Pages.Supplement
{
    public partial class ViewSupplementDetailsCard
    {

        #region [Fields]

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
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]


        #endregion

        #region [Methods :: Tasks]

        private string GetFormattedScheduleTime(DateTime scheduleDateTime, bool isSnooze)
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

        #endregion

        #region [Fields :: Public]

        [Parameter]
        public EventCallback<SupplementPageViewModel> OnClickCallback { get; set; }

        [Parameter]
        public SupplementPageViewModel SupplementPageViewModel { get; set; }

        #endregion

    }
}
