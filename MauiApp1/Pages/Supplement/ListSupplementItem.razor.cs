using MauiApp1.Areas.Supplement.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MauiApp1.Pages.Supplement
{
    public partial class ListSupplementItem
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

        private void SupplementItem_Click()
        {
            ViewSupplementPage();
        }

        #endregion

        #region [Methods :: Tasks]

        private string GetFormattedScheduleTime(DateTime scheduleDateTime)
        {
            string elapsed = string.Empty;

            string scheduleTime = scheduleDateTime.ToString("hh:mm");

            elapsed = string.Format("{0}", scheduleTime);

            return elapsed;
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
                await ViewSupplementOnClickCallback.InvokeAsync();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("List Supplement Item Page", "ViewSupplementOnClickCallback is not been set", "OK");
            }
            
        }


        #endregion

        #region [Fields :: Public]

        [Parameter]
        public EventCallback<Task> ViewSupplementOnClickCallback { get; set; }

        [Parameter]
        public SupplementPageViewModel SupplementPageViewModel { get; set; }

        #endregion

    }
}
