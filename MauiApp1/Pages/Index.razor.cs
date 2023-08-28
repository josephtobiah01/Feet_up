#if ANDROID
using Android.Graphics;
#endif
#if IOS
using UIKit;
using Foundation;
#endif
using FeedApi.Net7.Models;
using ImageApi.Net7;
using MauiApp1._Push;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Areas.Exercise.ViewModels;
using MauiApp1.Areas.Exercise.Views;
using MauiApp1.Areas.Nutrient.Views;
using MauiApp1.Areas.Overview.Views;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Areas.Supplement.ViewModels;
using MauiApp1.Business;
using MauiApp1.Business.BrowserServices;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Exceptions;
using MauiApp1.Helpers;
using MauiApp1.Pages.Nutrient;
using MauiApp1.Shared;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Maui.Graphics.Platform;
using NativeMedia;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.ApiModels;
using ParentMiddleWare.Models;
using Statics;
using System.Collections.ObjectModel;
#if WINDOWS
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
using System.Text;
using UserApi.Net7;
using UserApi.Net7.Models;
using MauiApp1.Interfaces;
using MauiApp1.Business.UserServices;
using MauiApp1.Services;
using MauiApp1.Pages.Chat;

namespace MauiApp1.Pages
{
    #region NavigationIntercept
    public class NavigationIntercept
    {
        public string Action { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
    }

    #endregion

    public partial class Index
    //in order to be referenced by the parent .razor, class has to be PARTIAL
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        #region [Fields]
        //intercept
        public static NavigationIntercept navigationIntercept { get; set; }
        //navigation
        private IDisposable registration;
        //For Ui Fields
        public static string DisplayFooter = "none";
        public static string DisplayNutrientPopup = "none";
        public static string DisplayAddDishPopup = "none";
        public static string DisplayMindfulnessPopup = "none";
        public static string DisplayAddNewPopup = "none";
        public static string DisplayFavoritePopup = "none";
        public static string DisplaySnoozePopup = "none";
        public static string DisplayMenuPopup = "none";
        public static string DisplaySkipPopup = "none";
        public bool DisplayMenuOnSnoozeClose = false;

        private bool _isBlackCoverDivHidden = true;
        private bool _isFeedItemDetailsDivHidden = true;
        private bool _isExerciseWhatsNewTabDiv = true;
        private bool _isChangeDateButtonDisable = false;

        private bool _isFeedItemStartClick = false;
        private bool _isFeedItemSnoozeClick = false;
        private bool _isFeedItemUndoClick = false;
        private bool _isFeedItemSummaryClick = false;
        private bool _isFeedItemMenuClick = false;

        private bool _isRefreshFeedItemNeeded = false;
        private bool _ReloadFeedItemDetailsPage = true;

        private DateTime _dateSelected;
        private string _selectedDate;

        private List<FeedItem> _beforeFeedItems;
        private List<FeedItem> _nowFeedItems;
        private List<FeedItem> _laterFeedItems;

        private FeedItem _feedItem;

        //nutrient fields
        public static FeedItem NutrientPopupCurrentFeedItem;
        public NutrientrecipesForMeal NutrientPopupRecipesDisplayed;
        public bool NutrientIsCustomAddedDish = false;
        public int NutrientServings = 1;
        public double NutrientPortion = 1;
        public string NutrientNotes = "";
        public string NutrientDishName = "";
        public string NutrientCustomDishImageUrl = "";
        public static string NutrientImageData = "";
        public static string NutrientImageType = "";
        public bool NutrientIsFavorite = false;
        public NutrientRecipeModel NutrientRecipe = null;
        public bool ShowAgainCheckedValue = true;

        public static List<NutritionUploadModel> NutritionUploadModel = new List<NutritionUploadModel>();

        private int _sheetHeight = 492;
        private double _sheetFeedItemListInitialHeight = 0;
        private double _sheetFeedItemListMaxHeight = 0;
        private double _sheetFeedItemListHeight = 600; //295;
        public double SheetFeedItemListHeight
        {
            get { return _sheetFeedItemListHeight; }
            set
            {
                if (value < _sheetFeedItemListInitialHeight)
                {
                    _sheetFeedItemListHeight = _sheetFeedItemListInitialHeight;
                }
                else
                {
                    _sheetFeedItemListHeight = value;
                }
            }
        }

        private double _InnerHeight { get; set; }
        private double _InnerWidth { get; set; }
        private string _greeting { get; set; }
        bool _TestMode = true;
        private bool LockScroll = false;
        #endregion

        #region [Fields :: Custom]

        private string _DateSelected
        {
            get { return _selectedDate; }

            set
            {
                SelectDateOnChange(value);
            }
        }



        #endregion

        #region[Initialization]


        public async Task DoNavigationIntercept(string aaction, string parameter)
        {
            try
            {
                switch (aaction)
                {
                    case Strings.NOTIF_NUTRIENT:
                        {
                            await ScrollToFeedID(parameter, aaction);
                            break;
                        }
                    case Strings.NOTIF_SUPPLEMENT:
                        {
                            await ScrollToFeedID(parameter, aaction);
                            break;
                        }
                    case Strings.NOTIF_TRAINING:
                        {
                            await ScrollToFeedID(parameter, aaction);
                            break;
                        }
                    case Strings.NOTIF_TRANSCRIPT:
                        {
                            await ScrollToFeedID(parameter, aaction);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch { }
        }

        public async Task ScrollToFeedID(string ID, string Action)
        {
            try
            {
                FeedItem Target;
                if (_nowFeedItems.Count == 0 && _laterFeedItems.Count == 0 && _beforeFeedItems.Count == 0)
                {
                    await GetFeedItems();
                }
                Target = _nowFeedItems.Where(t => t.ID == ID).FirstOrDefault();
                if (Target == null)
                {
                    Target = _laterFeedItems.Where(t => t.ID == ID).FirstOrDefault();
                }
                if (Target == null)
                {
                    Target = _beforeFeedItems.Where(t => t.ID == ID).FirstOrDefault();
                }
                if (Target == null)
                {
                    if (Action == Strings.NOTIF_NUTRIENT)
                    {
                        Target = await FeedApi.Net7.FeedApi.GetFeedItem(ID);
                        if (Target != null && Target.Date != DateTime.Today)
                        {
                            await SetDate(Target.Date);
                            if (_nowFeedItems.Count == 0 && _laterFeedItems.Count == 0 && _beforeFeedItems.Count == 0)
                            {
                                await GetFeedItems();
                            }
                        }
                    }
                }
                if (Target == null)
                {
                    return;
                }
                if (Action == Strings.NOTIF_TRANSCRIPT && Target.ItemType == FeedItemType.NutrientsFeedItem)
                {
                    await Task.Delay(250);
                    await GoToOverviewPage(Target, true);
                }
                else if (Target.Status != FeedItemStatus.Completed && Target.Status != FeedItemStatus.Skipped)
                {
                    if (Target.ItemType == FeedItemType.NutrientsFeedItem)
                    {
                        await Task.Delay(250);
                        await OpenNutrientPopup(Target);
                    }
                    else if (Target.ItemType == FeedItemType.SupplementItem)
                    {
                        await Task.Delay(250);
                        await ViewSupplementFeedItemDetails(Target);
                    }
                    else if (Target.ItemType == FeedItemType.TrainingSessionFeedItem)
                    {
                        await Task.Delay(250);
                        await FeedItem_Click(Target);
                    }
                    else
                    {
                        return;
                    }
                }
                await JSRuntime.InvokeVoidAsync("ScrollToItemID", ID);
                // LockScroll will prevent the first call to scrolltonow. It resets automatically
                LockScroll = true;
                await StateHasChanged();
            }
            catch { }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                PushNavigationHelper.RootPage = this;
                await JSRuntime.InvokeVoidAsync("renderJqueryComponentsinIndex");
                //await JSRuntime.InvokeVoidAsync("JavaScriptInteropDatepicker");
                await JSRuntime.InvokeVoidAsync("blazorjs.dragable");
                //await JSRuntime.InvokeVoidAsync("blazorjsFeedItemContentGroupDrag.dragable");
                await LoadBrowserDimensions();
                await IntializeData();
            }
        }

        #endregion

        #region [Methods :: EventHandlers :: Class]

        private async Task IntializeData()
        {
            try
            {
                PushNavigationHelper.RootPage = this;
                _nowFeedItems = new List<FeedItem>();
                _beforeFeedItems = new List<FeedItem>();
                _laterFeedItems = new List<FeedItem>();

                _dateSelected = DateTime.Now;

                InitializeSelectDate();

               // await GetFeedItems();

                //GetGreeting();

                if (HTMLBridge.RefreshData != null)
                {
                    HTMLBridge.RefreshData -= RefreshData_OnRefresh;
                }
                HTMLBridge.RefreshData += RefreshData_OnRefresh;

                if (navigationIntercept != null)
                {
                    await PushNavigationHelper.Navigate_to_feed_item(navigationIntercept);
                    navigationIntercept = null;

                    // LockScroll will prevent the first call to scrolltonow. It resets automatically
                    LockScroll = true;
                }
                else
                {
                   await LoadData();
                  //  await StateHasChanged();
                }
            }
            catch { }
        }

        private async Task LoadData()
        {
            await GetFeedItems();
            await StateHasChanged();
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        public static void EnableFooter()
        {
            DisplayFooter = "inline";
        }
        public static void DisableFooter()
        {
            DisplayFooter = "none";
        }
        private async void NewButton_Click()
        {
           await OpenAddNewPopup();
        }

        private void GoToPreviousDateButton_Click()
        {
            GoToPreviousDate();
        }

        private void GoToNextDateButton_Click()
        {
            GoToNextDate();
        }

        private void FeedItemSummaryButton_Click(FeedItem feedItem)
        {
            ViewTrainingSessionSummaryDetail(feedItem.TrainingSessionFeedItem);
        }

        private void FeedItemStartButton_Click(FeedItem feedItem)
        {
            HandleFeedItemStartButtonClick(feedItem);
        }

        private void FeedItemUndoButton_Click(FeedItem feedItem)
        {
            HandleFeedItemUndoButtonClick(feedItem);
        }

        private async Task FeedItem_Click(FeedItem feedItem)
        {
            await HandleFeedItemClick(feedItem);
        }


        private void TabItemExerciseWhatsInsideInput_Click()
        {
            _isExerciseWhatsNewTabDiv = false;
        }

        private void TabItemExerciseHistoryInput_Click()
        {
            _isExerciseWhatsNewTabDiv = true;

        }

        private async void BlackCoverDiv_Click()
        {
            await BlackCoverDivToggle();
        }
        //StopTraningSession

        private async void StopTraningSession(FeedItem feedItem)
        {
            ShowLoadingActivityIndicator();
            int vale = feedItem.TrainingSessionFeedItem.TraningSession.ExerciseDuration.HasValue ? feedItem.TrainingSessionFeedItem.TraningSession.ExerciseDuration.Value : 30;
            try
            {
                bool success = await ExerciseApi.Net7.ExerciseApi.EndTrainingSession(feedItem.TrainingSessionFeedItem.TraningSession.Id, vale);
                if (!success)
                {
                    ShowAlertBottomSheet("Error", "Error finishing training session.", "OK");
                }
                // await StateHasChanged();
            }
            catch
            {
                ShowAlertBottomSheet("Error", "Error finishing training session.", "OK");
            }
            HideLoadingActivityIndicator();
            await RefreshPageWithoutClearingFeedItem();
        }


        private void FeedItemSnooze_Click(FeedItem feedItem)
        {
            HandleFeedItemSnoozeButtonClick(feedItem);
        }

        private async Task FeedItemMenu_Click(FeedItem feedItem)
        {
            await HandleFeedItemMenuButtonClick(feedItem);
        }

        private void FeedItemDetailsStart_Click(FeedItem feedItem)
        {
            HandleFeedItemStartButtonClick(feedItem);
        }

        private void FeedItemListSheet_OnTouchMove(Microsoft.AspNetCore.Components.Web.TouchEventArgs args)
        {
            /*
            this.SheetFeedItemListHeight = Convert.ToInt32(_InnerHeight - args.ChangedTouches[0].ClientY);

            if (this.SheetFeedItemListHeight <= _sheetFeedItemListInitialHeight)
            {
                this.SheetFeedItemListHeight = _sheetFeedItemListInitialHeight;
            }

            if (this.SheetFeedItemListHeight >= _sheetFeedItemListMaxHeight)
            {
                this.SheetFeedItemListHeight = _sheetFeedItemListMaxHeight;
            }*/
        }

        private async void FeedItemDetailsBottomSheet_OnTouchMove(Microsoft.AspNetCore.Components.Web.TouchEventArgs args)
        {
            int maxSheetDragHeight = (int)((_InnerHeight - 60));
            _sheetHeight = Convert.ToInt32(_InnerHeight - args.ChangedTouches[0].ClientY);

            if (_sheetHeight <= 300)
            {
                //_sheetHeight = 492;
                await JSRuntime.InvokeVoidAsync("HideExerciseDetail");
                _isBlackCoverDivHidden = true;
                _isFeedItemDetailsDivHidden = true;
                _isExerciseWhatsNewTabDiv = true;

            }
            if (_sheetHeight >= maxSheetDragHeight)
            {
                _sheetHeight = maxSheetDragHeight;
            }
        }

        private async void FeedButton_Click()
        {
            await GoToCurrentDate();
        }

        private async void DashboardButton_Click()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RazorHomeContentPage());
        }

        private async void BiodataButton_Click()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RazorHomeContentPage());
        }

        private async void Dashboard_Click()
        {
            await GoToCurrentDate();
        }

        public async void AddNewExercise()
        {
            var result = await UserMiddleware.AddCustomTrainingSession();
            if (result == null)
            {
                await CloseAddNewPopup();
                return;
            }
           await CloseAddNewPopup();
            ViewTrainingSessionContentPage.DoApiCalls = false;
            ViewTrainingSessionContentPage.ExerciseViewModels = new ObservableCollection<ExercisePageViewModel>();
            var TrainingPage = new ViewTrainingSessionContentPage(result);
            ViewTrainingSessionContentPage.DoApiCalls = true;
            TrainingPage.HideLoadingActivity();
            await App.Current.MainPage.Navigation.PushAsync(TrainingPage, true);
            
        }

        public async void GoToChatPage()
        {
            ISelectedImageService selectedImageService = new SelectedImageService();
            ViewHybridChatContentPage viewHybridChatContentPage = new ViewHybridChatContentPage(selectedImageService);
            await Application.Current.MainPage.Navigation.PushAsync(viewHybridChatContentPage);
            await CloseAddNewPopup();
        }


        private async Task Page_Unloaded(object sender, EventArgs e)
        {
            await RefreshPage();
            await StateHasChanged();
        }

        private async Task Page_CLosing(object sender, EventArgs e)
        {
            await RefreshPage();
            await StateHasChanged();
        }
        private async Task Page_Disappearing(object sender, EventArgs e)
        {
            await RefreshPage();
            await StateHasChanged();
        }
        private async Task Page_Unfocused(object sender, EventArgs e)
        {
            await RefreshPage();
            await StateHasChanged();
        }
        protected virtual async void RefreshData_OnRefresh(object sender, EventArgs e)
        {
            await RefreshPage();
        }

        //To Be Deleted
        private async Task SelectDate_OnChange(ChangeEventArgs args)
        {
            string selectedDate = args.Value.ToString();
            await SelectDateOnChange(selectedDate);
        }
        #region NUTRIENT_METHODS
        public async Task OpenNutrientPopup(FeedItem currentfeeditem, bool IsCustomDish = false)
        {
            if (IsCustomDish)
            {
                FeedItem temporaryFeedItem = new FeedItem();
                temporaryFeedItem.NutrientsFeedItem = new NutrientsFeedItem();
                temporaryFeedItem.NutrientsFeedItem.Meal = new NutrientMeal();
                temporaryFeedItem.Title = "New Meal";
                temporaryFeedItem.NutrientsFeedItem.Meal.MealId = -1;
                temporaryFeedItem.ItemType = FeedItemType.NutrientsFeedItem;
                temporaryFeedItem.Status = FeedItemStatus.Scheduled;
                temporaryFeedItem.Date = DateTime.Now;
                temporaryFeedItem.NutrientsFeedItem.Meal.IsCustom = true;
                temporaryFeedItem.NutrientsFeedItem.Meal.TargetKiloCalories = 420;
                NutrientPopupCurrentFeedItem = temporaryFeedItem;
            }
            else
            {
                NutrientPopupCurrentFeedItem = currentfeeditem;
            }
            Index.NutritionUploadModel = new List<NutritionUploadModel>();
            try
            {
                NutrientPopupRecipesDisplayed = await ImageApi.Net7.NutritionApi.GetFavoritesAndHistory();
            }
            catch
            {
                ShowAlertBottomSheet("Error", "Error getting favorites and history.", "OK");
            }
            DisplayNutrientPopup = "inline";
        }
        public async Task CloseNutrientPopup()
        {
            DisplayNutrientPopup = "none";
        }
        public async Task OpenAddDishPopup(string photo = null, NutrientRecipeModel recipe = null)
        {
            DisplayAddDishPopup = "inline";
            NutrientServings = 1;
            NutrientIsFavorite = false;
            await OneHundredPercentPortion();
            NutrientDishName = "";
            NutrientNotes = "";
            if (photo == null)
            {
                NutrientIsCustomAddedDish = false;
                NutrientRecipe = recipe;
                NutrientIsFavorite = recipe.IsFavorite;
                NutrientDishName = recipe.RecipeName;
                NutrientCustomDishImageUrl = recipe.DisplayImageUrl;
            }
            else
            {
                NutrientIsCustomAddedDish = true;
                NutrientCustomDishImageUrl = String.Format("data:image/png;base64,{0}", NutrientImageData);
            }
        }

        public async Task CloseAddDishPopup()
        {
            try
            {
                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                if (accessType == NetworkAccess.Internet)
                {
                    try
                    {
                        NutrientPopupRecipesDisplayed = await ImageApi.Net7.NutritionApi.GetFavoritesAndHistory();
                    }
                    catch
                    {
                        ShowAlertBottomSheet("Error", "Error finishing training session.", "OK");
                    }
                }
            }
            catch
            {

            }
            DisplayAddDishPopup = "none";
        }
        public void OpenFavoritePopup()
        {
            DisplayFavoritePopup = "inline";
        }
        public void CloseFavoritePopup()
        {
            if (ShowAgainCheckedValue)
            {
                ParentMiddleWare.MiddleWare.SetShowFavoriteMsg(false);
            }
            DisplayFavoritePopup = "none";
        }
        public void AddServing()
        {
            NutrientServings += 1;
        }
        public void SubtractServing()
        {
            NutrientServings -= 1;
            NutrientServings = Math.Max(1, NutrientServings);
        }
        public void TwentyPercentPortion()
        {
            NutrientPortion = 0.2;
        }
        public void FourtyPercentPortion()
        {
            NutrientPortion = 0.4;
        }
        public void SixtyPercentPortion()
        {
            NutrientPortion = 0.6;
        }
        public void EightyPercentPortion()
        {
            NutrientPortion = 0.8;
        }
        public async Task OneHundredPercentPortion()
        {
            NutrientPortion = 1;
            await JSRuntime.InvokeVoidAsync("setDefaultNutrientRadioButton");
        }
        public async Task AddDish(bool image = false, NutrientRecipeModel recipe = null)
        {
            try
            {
                NutrientDish dishAdded = new NutrientDish();
                dishAdded.NumberOfServings = NutrientServings;
                dishAdded.PercentageEaten = NutrientPortion;
                dishAdded.Notes = NutrientNotes;

                NutritionUploadModel uploadModel = new NutritionUploadModel();

                if (image == false && recipe != null)
                {
                    dishAdded.Recipe = recipe;
                    uploadModel.IsFavorite = NutrientIsFavorite;
                    uploadModel.MealId = NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId;
                    uploadModel.RecipeId = recipe.RecipeID;
                    uploadModel.NumberOfServings = NutrientServings;
                    uploadModel.NutrientPortion = NutrientPortion;
                    uploadModel.UploadType = NutritionUploadModel_Type.ByRecipe;
                    NutritionUploadModel.Add(uploadModel);
                }
                else
                {
                    uploadModel.UploadType = NutritionUploadModel_Type.PhotoUpload;
                    uploadModel.NumberOfServings = NutrientServings;
                    uploadModel.NutrientPortion = NutrientPortion;
                    uploadModel.FoodImage64String = Index.NutrientImageData;
                    uploadModel.FoodImageType = Index.NutrientImageType;
                    uploadModel.NutrientNotes = NutrientNotes;
                    uploadModel.IsFavorite = NutrientIsFavorite;
                    if (NutrientDishName == null || NutrientDishName == "")
                    {
                        //await App.Current.MainPage.DisplayAlert("Error", "Please enter a name for the dish.", "OK");
                        ShowAlertBottomSheet("Error", "Please enter a name for the dish.", "OK");
                        return;
                    }
                    uploadModel.NutrientDishName = NutrientDishName;
                    uploadModel.MealId = Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId;


                    NutritionUploadModel.Add(uploadModel);

                    NutrientRecipeModel temprecipe = new NutrientRecipeModel();
                    temprecipe.NutrientInformation = new NutrientRecipeModel.RecipeNutrientInformation();

                    temprecipe.RecipeName = NutrientDishName;
                    temprecipe.RecipeID = 42069;
                    temprecipe.DisplayImageUrl = String.Format("data:image/png;base64,{0}", NutrientImageData);

                    dishAdded.Recipe = temprecipe;
                }
                if (NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.DishesEaten == null && NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.IsCustom != true)
                {
                    try
                    {
                        NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.DishesEaten = await ImageApi.Net7.NutritionApi.GetMealDishes(NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId);
                    }
                    catch
                    {
                        ShowAlertBottomSheet("Error", "Error getting meals.", "OK");
                    }
                }
                NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.DishesEaten.Add(dishAdded);
                await GoToOverviewPage(NutrientPopupCurrentFeedItem);
                await CloseAddDishPopup();
                await CloseNutrientPopup();
                await CloseAddNewPopup();
            }
            catch
            {
                App.alertBottomSheetManager.ShowAlertMessage("Error", "Error adding dish.", "OK");
            }
        }
        public async Task FavoriteDish()
        {
            NutrientIsFavorite = !NutrientIsFavorite;
            if (NutrientIsFavorite && NutrientRecipe != null)
            {
                try
                {
                    await ImageApi.Net7.NutritionApi.FavoriteDish(NutrientRecipe.RecipeID);
                }
                catch
                {
                    ShowAlertBottomSheet("Error", "Error favoriting dish.", "OK");
                }
                await StateHasChanged();
            }
            else if (NutrientRecipe != null)
            {
                try
                {
                    await ImageApi.Net7.NutritionApi.UnFavoriteDish(NutrientRecipe.RecipeID);
                }
                catch
                {
                    ShowAlertBottomSheet("Error", "Error unfavoriting dish.", "OK");
                }
                await StateHasChanged();
            }
            else
            {
                if (NutrientIsFavorite && ParentMiddleWare.MiddleWare.ShowFavoriteMsg)
                {
                    OpenFavoritePopup();
                }
                await StateHasChanged();
            }
        }
        #endregion
        public async Task OpenAddNewPopup()
        {
            DisplayAddNewPopup = "inline";
             await StateHasChanged();
        }
        private async Task CloseAddNewPopup()
        {
            DisplayAddNewPopup = "none";
            await StateHasChanged();
        }

        private void SetIsRefreshFeedItemNeeded_EventCallback()
        {
            _isRefreshFeedItemNeeded = true;
        }

        private void LoadFeedItemNeeded_EventCallback(bool reloadFeedItemDetailsPage)
        {
            _ReloadFeedItemDetailsPage = reloadFeedItemDetailsPage;
        }


        private void CloseMindfulnessPopup()
        {
            DisplayMindfulnessPopup = "none";
        }
        public void Dispose()
        {
            registration?.Dispose();
        }
        public async void GoToSearchRecipesPage(FeedItem feeditem, long status)
        {
            await CloseAddNewPopup();
            //CloseNutrientPopup();
            await App.Current.MainPage.Navigation.PushAsync(new SearchRecipesPage(feeditem, status));
        }
        public async Task GoToOverviewPage(FeedItem feeditem, bool IsSubmitted = false)
        {
             await CloseAddNewPopup();
             await CloseNutrientPopup();
            var Page = new OverviewPage(feeditem, IsSubmitted);
            await App.Current.MainPage.Navigation.PushAsync(Page);
        }
        #endregion

        #region [Methods :: Tasks]

        private void InitializeSelectDate()
        {
            string formattedDate = string.Empty;
            string numberSuffix = string.Empty;
            string monthShort = string.Empty;
            monthShort = _dateSelected.ToString("MMM");
            numberSuffix = GetDayNumberSuffix(_dateSelected);
            formattedDate = string.Format("{0} {1}, {2}", _dateSelected.Day + numberSuffix, monthShort, _dateSelected.DayOfWeek);
            _selectedDate = formattedDate;
        }
        private void ShowCalendar()
        {
#if !WINDOWS
            if (HTMLBridge.DXCalenderPopup != null)
            {
                HTMLBridge.DXCalenderPopup.IsOpen = true;
            }
#endif
        }
        public async Task SetDate(DateTime? DateInput)
        {
            string formattedDate = string.Empty;
            string numberSuffix = string.Empty;
            string monthShort = string.Empty;
            if (DateInput != null)
            {
                //System.Diagnostics.Debug.WriteLine(DateInput.ToString());

                _dateSelected = DateInput.GetValueOrDefault();

                monthShort = _dateSelected.ToString("MMM");

                numberSuffix = GetDayNumberSuffix(_dateSelected);

                formattedDate = string.Format("{0} {1}, {2}", _dateSelected.Day + numberSuffix, monthShort, _dateSelected.DayOfWeek);

                _selectedDate = formattedDate;

                _beforeFeedItems.Clear();
                _nowFeedItems.Clear();
                _laterFeedItems.Clear();

                await GetFeedItems();
                _isChangeDateButtonDisable = false;

                await StateHasChanged();
            }
        }
        private async Task GetFeedItems()
        {
            try
            {
                if (MiddleWare.FkFederatedUser == string.Empty)
                {
                    HideLoadingActivityIndicator();
                    return;
                }

                isblocked = false;
                if (!MiddleWare.IsInit && MiddleWare.FkFederatedUser != string.Empty)
                {
                    MiddleWare.IsInit = true;

                    if (MiddleWare.FkFederatedUser != string.Empty)
                    {
                        bool IsNotificationAllowed = await CheckNotificationPermissions();
                        if (IsNotificationAllowed)
                        {
#if IOS
                            // not async by design
                            UserMiddleware.RegisterDevice(await PushRegistration.CheckPermission(), PushRegistration.GetPlatform());
#endif
#if ANDROID
                            App.SetupFireBase();
#endif
                        }
                        else
                        {
                            OpenRequestNotificationPermissionsPopup();
                        }
                        if (MiddleWare.FkFederatedUser != string.Empty)
                        {
                            // not async by design
                            UserMiddleware.UpdateOffset();
                        }
                    }

                }

                ShowLoadingActivityIndicator();


                _beforeFeedItems = new List<FeedItem>();
                _nowFeedItems = new List<FeedItem>();
                _laterFeedItems = new List<FeedItem>();

                try
                {
                    List<FeedItem> feedItems = await FeedApi.Net7.FeedApi.GetDailyFeedAsync(_dateSelected);
                    var nowIsFilled = false;
                    FeedItem LaterPlaceholder = null;
                    bool isLaterPlaceHolderFilled = false;

                    if (_dateSelected.Date != DateTime.Now.Date) nowIsFilled = true;

                    foreach (FeedItem feedItem in feedItems)
                    {
                        if (feedItem.Date <= DateTime.Now.AddMinutes(-1))
                        {
                            if (feedItem.Date >= DateTime.Now.AddMinutes(-15) && feedItem.Status != FeedItemStatus.Completed && feedItem.Status != FeedItemStatus.Skipped)
                            {
                                _nowFeedItems.Add(feedItem);
                                nowIsFilled = true;
                                continue;
                            }
                            _beforeFeedItems.Add(feedItem);
                        }
                        else if (feedItem.Date < DateTime.Now.AddMinutes(15)
                            && feedItem.Status != FeedItemStatus.Completed && feedItem.Status != FeedItemStatus.Skipped)
                        {
                            _nowFeedItems.Add(feedItem);
                        }
                        else
                        {
                            _laterFeedItems.Add(feedItem);
                            if (!isLaterPlaceHolderFilled)
                            {
                                LaterPlaceholder = feedItem;
                                isLaterPlaceHolderFilled = true;
                            }
                        }
                    }

                    if (!nowIsFilled && LaterPlaceholder != null)
                    {
                        _laterFeedItems.Remove(LaterPlaceholder);
                        _nowFeedItems.Add(LaterPlaceholder);
                    }

                }
                catch
                {
                    ShowAlertBottomSheet("Error", "Error getting feed.", "OK");
                }
                HideLoadingActivityIndicator();
            }
            catch(Exception e)
            {
                try
                {
                    HideLoadingActivityIndicator();
                }
                catch { }
                //await App.Current.MainPage.DisplayAlert("Retrieve Feed Item", "An error occurred while retrieving feed items. Please check internet connection and try again", "OK");
                ShowAlertBottomSheet("Retrieve Feed Item", "An error occurred while retrieving feed items. Please check internet connection and try again", "OK");
            }
            finally
            {
           
            }
        }

        public string GetStatuswithFormattedTimeElapsed(FeedItemType feedItemType, DateTime dateTime, FeedItemStatus status)
        {
            string formattedText = string.Empty;
            string timeElapsed = string.Empty;
            //formattedText = dateTime.ToString("hh:mm tt");
            switch (status)
            {
                case FeedItemStatus.Completed:

                    formattedText = string.Format("Completed");
                    break;

                case FeedItemStatus.Ongoing:

                    if (feedItemType == FeedItemType.SupplementItem)
                    {
                        timeElapsed = GetElapsedTime(dateTime);
                        formattedText = string.Format("Ongoing {0}",
                                    dateTime.ToString("hh:mm tt"));
                    }
                    else
                    {
                        formattedText = string.Format("Ongoing");
                    }
                    break;

                case FeedItemStatus.Skipped:

                    formattedText = string.Format("Skipped");
                    break;

                case FeedItemStatus.Partly_Skipped:

                    formattedText = string.Format("Partly Skipped");
                    break;

                case FeedItemStatus.Snoozed:

                    formattedText = string.Format("Snoozed ({0})",
       dateTime.ToString("hh:mm tt"));
                    break;

                default:

                    DateTime dateTimeNow = DateTime.Now.AddMinutes(30);
                    timeElapsed = GetElapsedTime(dateTime);
                    if (DateTime.Now.AddMinutes(-MiddleWare.OverDueTime) > dateTime)
                    {
                        formattedText = string.Format("overdue ({0})",
                                     dateTime.ToString("hh:mm tt"));
                    }
                    else if (dateTime <= dateTimeNow)
                    {
                        formattedText = string.Format("in {0} ({1})", timeElapsed,
                                     dateTime.ToString("hh:mm tt"));
                    }
                    else
                    {
                        formattedText = string.Format("starts at {0}", dateTime.ToString("hh:mm tt"));
                    }
                    break;
            }
            return formattedText;

            //if (status == FeedItemStatus.Completed)
            //{
            //    formattedText = string.Format("Completed");
            //    return formattedText;
            //}

            //if (status == FeedItemStatus.Skipped)
            //{
            //    formattedText = string.Format("Skipped");
            //    return formattedText;
            //}
            //else if (status == FeedItemStatus.Partly_Skipped)
            //{
            //    formattedText = string.Format("Partly Skipped");
            //    return formattedText;
            //}
            //else if (status == FeedItemStatus.Snoozed)
            //{
            //    formattedText = string.Format("Snoozed ({0})",
            //      dateTime.ToString("hh:mm tt"));
            //    return formattedText;
            //}

            //if (status == FeedItemStatus.Ongoing)
            //{
            //    formattedText = string.Format("Ongoing");
            //    return formattedText;
            //}

            //DateTime dateTimeNow = DateTime.Now.AddMinutes(30);

            //timeElapsed = GetElapsedTime(dateTime);
            //if (DateTime.Now.AddMinutes(-MiddleWare.OverDueTime) > dateTime)
            //{
            //    formattedText = string.Format("overdue ({0})",
            //                 dateTime.ToString("hh:mm tt"));
            //}
            //else if (dateTime <= dateTimeNow)
            //{
            //    formattedText = string.Format("in {0} ({1})", timeElapsed,
            //                 dateTime.ToString("hh:mm tt"));
            //}
            //else
            //{
            //    formattedText = string.Format("starts at {0}", dateTime.ToString("hh:mm tt"));
            //}

            //return formattedText;
        }

        private async void GoToPreviousDate()
        {

            string formattedDate = string.Empty;
            string numberSuffix = string.Empty;
            string monthShort = string.Empty;

            _isChangeDateButtonDisable = true;

            _dateSelected = _dateSelected.AddDays(-1);
            _dateSelected = new DateTime(_dateSelected.Year, _dateSelected.Month, _dateSelected.Day, 0, 0, 0);

            monthShort = _dateSelected.ToString("MMM");

            numberSuffix = GetDayNumberSuffix(_dateSelected);

            formattedDate = string.Format("{0} {1}, {2}", _dateSelected.Day + numberSuffix, monthShort, _dateSelected.DayOfWeek);

            _selectedDate = formattedDate;

            if (_dateSelected.Date == DateTime.Now.Date)
            {
                _dateSelected = DateTime.Now;
            }
            _beforeFeedItems.Clear();
            _nowFeedItems.Clear();
            _laterFeedItems.Clear();

            await GetFeedItems();

            _isChangeDateButtonDisable = false;

            await StateHasChanged();

        }

        private async void GoToNextDate()
        {
            string formattedDate = string.Empty;
            string numberSuffix = string.Empty;
            string monthShort = string.Empty;

            _isChangeDateButtonDisable = true;

            _dateSelected = _dateSelected.AddDays(1);
            _dateSelected = new DateTime(_dateSelected.Year, _dateSelected.Month, _dateSelected.Day, 0, 0, 0);

            monthShort = _dateSelected.ToString("MMM");

            numberSuffix = GetDayNumberSuffix(_dateSelected);

            formattedDate = string.Format("{0} {1}, {2}", _dateSelected.Day + numberSuffix, monthShort, _dateSelected.DayOfWeek);

            _selectedDate = formattedDate;

            if (_dateSelected.Date == DateTime.Now.Date)
            {
                _dateSelected = DateTime.Now;
            }
            _beforeFeedItems.Clear();
            _nowFeedItems.Clear();
            _laterFeedItems.Clear();

            await GetFeedItems();

            _isChangeDateButtonDisable = false;

            await StateHasChanged();
        }

        private async Task GoToCurrentDate()
        {
            string formattedDate = string.Empty;
            string numberSuffix = string.Empty;
            string monthShort = string.Empty;

            _isChangeDateButtonDisable = true;

            _dateSelected = DateTime.Now;
            _dateSelected = new DateTime(_dateSelected.Year, _dateSelected.Month, _dateSelected.Day, 0, 0, 0);

            monthShort = _dateSelected.ToString("MMM");

            numberSuffix = GetDayNumberSuffix(_dateSelected);

            formattedDate = string.Format("{0} {1}, {2}", _dateSelected.Day + numberSuffix, monthShort, _dateSelected.DayOfWeek);

            _selectedDate = formattedDate;

            if (_dateSelected.Date == DateTime.Now.Date)
            {
                _dateSelected = DateTime.Now;
            }

            _beforeFeedItems.Clear();
            _nowFeedItems.Clear();
            _laterFeedItems.Clear();

            await GetFeedItems();
            _isChangeDateButtonDisable = false;

            await StateHasChanged();
        }

        private string GetDayNumberSuffix(DateTime date)
        {
            int day = date.Day;

            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";

                case 2:
                case 22:
                    return "nd";

                case 3:
                case 23:
                    return "rd";

                default:
                    return "th";
            }
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

        private string GetExerciseSetCount(ICollection<EmSet> emSets)
        {
            string setCount = string.Empty;

            if (emSets != null)
            {
                if (emSets.Count == 1)
                {
                    setCount = string.Format("{0} set", emSets.Count.ToString());
                }
                else
                {
                    setCount = string.Format("{0} sets", emSets.Count.ToString());
                }
            }
            else
            {
                setCount = "0 set";
            }
            return setCount;
        }

        private async Task BlackCoverDivToggle()
        {
            /*
            if (_isBlackCoverDivHidden == true)
            {

                await JSRuntime.InvokeVoidAsync("ShowExerciseDetail");
                _isBlackCoverDivHidden = false;
                _isFeedItemDetailsDivHidden = false;
            }
            else
            {*/
            await JSRuntime.InvokeVoidAsync("HideExerciseDetail");
            _isBlackCoverDivHidden = true;
            _isFeedItemDetailsDivHidden = true;
            _ReloadFeedItemDetailsPage = true;
            /*}*/

            if (_isRefreshFeedItemNeeded == true)
            {
                await RefreshPage();
                _isRefreshFeedItemNeeded = false;
            }
            else
            {

            }
        }

        private async Task ScrollDivToNowSection()
        {
            if (LockScroll)
            {
                LockScroll = false;
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("ScrollToNow");
            }
            //   await JSRuntime.InvokeVoidAsync("setupDebounce");
        }

        private bool is_user_interaction = false;
        private async Task HandleFeedItemClick(FeedItem feedItem)
        {
            is_user_interaction = true;
            if (_isFeedItemStartClick == true)
            {
                _isFeedItemStartClick = false;
            }
            else if (_isFeedItemSnoozeClick == true)
            {
                _isFeedItemSnoozeClick = false;
            }
            else if (_isFeedItemUndoClick == true)
            {
                _isFeedItemUndoClick = false;
            }
            else if (_isFeedItemSummaryClick == true)
            {
                _isFeedItemSummaryClick = false;
            }
            else if (_isFeedItemMenuClick == true)
            {
                _isFeedItemMenuClick = false;
            }
            else
            {
                await ViewFeedItemDetail(feedItem);
            }
            is_user_interaction = false;
        }

        private async Task ViewFeedItemDetail(FeedItem feedItem)
        {
            is_user_interaction = true;
            switch (feedItem.ItemType)
            {
                case FeedItemType.TrainingSessionFeedItem:
                    await ViewTrainingSessionFeedItemDetails(feedItem);
                    break;

                case FeedItemType.NutrientsFeedItem:
                    break;

                case FeedItemType.SupplementItem:
                    break;

                default:
                    break;
            }
            is_user_interaction = false;
        }

        private async Task ViewTrainingSessionFeedItemDetails(FeedItem feedItem)
        {
            _feedItem = feedItem;
            _sheetHeight = 495;
            await JSRuntime.InvokeVoidAsync("ShowExerciseDetail");
            _isBlackCoverDivHidden = false;
            _isFeedItemDetailsDivHidden = false;
            _isExerciseWhatsNewTabDiv = false;
        }

        private async Task ViewSupplementFeedItemDetails(FeedItem feedItem)
        {
            _isBlackCoverDivHidden = false;
            _isFeedItemDetailsDivHidden = false;
            _isExerciseWhatsNewTabDiv = false;
            _feedItem = feedItem;
            _sheetHeight = ((int)_InnerHeight / 100) * 86;
            await JSRuntime.InvokeVoidAsync("ShowExerciseDetail");
            
        }
        private async Task HandleFeedItemMenuButtonClick(FeedItem selectedFeedItem)
        {
            _isFeedItemMenuClick = true;
            _feedItem = selectedFeedItem;
            await OpenMenuPopup();
        }
        public async Task OpenMenuPopup()
        {
            DisplayMenuPopup = "inline";
            await StateHasChanged();
        }

        public async Task CloseMenuPopup()
        {
            DisplayMenuPopup = "none";
            await StateHasChanged();
        }

        #region SNOOZING
        private async void HandleFeedItemSnoozeButtonClick(FeedItem selectedFeedItem)
        {
            _isFeedItemSnoozeClick = true;
            _feedItem = selectedFeedItem;
            await OpenSnoozePopup();

        }

        public async Task OpenSnoozePopup(bool OpenMenuOnClose = false)
        {
            if (OpenMenuOnClose)
            {
                DisplayMenuOnSnoozeClose = true;
                await CloseMenuPopup();
            }
            DisplaySnoozePopup = "inline";
            await StateHasChanged();
        }

        public async Task CloseSnoozePopup()
        {
            if (DisplayMenuOnSnoozeClose)
            {
                DisplayMenuOnSnoozeClose = false;
                await OpenMenuPopup();
            }
            DisplaySnoozePopup = "none";
            await StateHasChanged();
        }

        public async Task SnoozeByAmount(int amount)
        {
            try
            {
                _feedItem.Date.AddMinutes(amount);

                if (amount > 0)
                {
                    ShowLoadingActivityIndicator();

                    switch (_feedItem.ItemType)
                    {
                        case FeedItemType.TrainingSessionFeedItem:

                            await SnoozeTrainingSessionFeedItem(_feedItem, amount);
                            break;

                        case FeedItemType.NutrientsFeedItem:

                            await SnoozeNutrientFeedItem(_feedItem, amount);
                            break;

                        case FeedItemType.SupplementItem:

                            await SnoozeSupplementFeedItem(_feedItem, amount);
                            break;

                        case FeedItemType.HabitsFeedItem:

                            await SnoozeHabitFeedItem(_feedItem, amount);
                            break;

                        case FeedItemType.MedicationFeedItem:

                            await SnoozeMedicalFeedItem(_feedItem, amount);
                            break;

                        default:
                            break;

                    }
                    HideLoadingActivityIndicator();
                    await RefreshPageWithoutClearingFeedItem();
                }
            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Reschedule Feed Item", "An error occurred while rescheduling feed items.", "OK");
                ShowAlertBottomSheet("Reschedule Feed Item", "An error occurred while rescheduling feed items.", "OK");
            }
        }
        private async Task SnoozeTrainingSessionFeedItem(FeedItem feedItem, int waitTimeInMinutes)
        {
            try
            {
                await ExerciseApi.Net7.ExerciseApi.RescheduleTrainingSession(feedItem.TrainingSessionFeedItem.TraningSession.Id, waitTimeInMinutes);
            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Reschedule Training Session Feed Item", "An error occurred while rescheduling training session feed items.", "OK");
                ShowAlertBottomSheet("Reschedule Training Session Feed Item", "An error occurred while rescheduling training session feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task SnoozeNutrientFeedItem(FeedItem feedItem, int waitTimeInMinutes)
        {
            try
            {
                await NutritionApi.SnoozeMeal(feedItem.NutrientsFeedItem.Meal.MealId, waitTimeInMinutes);
            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Reschedule Nutrient Feed Item", "An error occurred while rescheduling supplement feed items.", "OK");
                ShowAlertBottomSheet("Reschedule Nutrient Feed Item", "An error occurred while rescheduling supplement feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task SnoozeSupplementFeedItem(FeedItem feedItem, int waitTimeInMinutes)
        {
            try
            {
                foreach (SupplementEntry supplementEntry in feedItem.SupplementFeedItem.SupplementEntries)
                {
                    await SupplementApi.Net7.SupplementApi.SnoozeDose(supplementEntry.DoseId, waitTimeInMinutes);
                }

            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Reschedule Supplement Feed Item", "An error occurred while rescheduling supplement feed items.", "OK");
                ShowAlertBottomSheet("Reschedule Supplement Feed Item", "An error occurred while rescheduling supplement feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task SnoozeHabitFeedItem(FeedItem feedItem, int waitTimeInMinutes)
        {
            try
            {

            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Reschedule Medical Feed Item", "An error occurred while rescheduling medical feed items.", "OK");
                ShowAlertBottomSheet("Reschedule Medical Feed Item", "An error occurred while rescheduling medical feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task SnoozeMedicalFeedItem(FeedItem feedItem, int waitTimeInMinutes)
        {
            try
            {

            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Reschedule Medical Feed Item", "An error occurred while rescheduling medical feed items.", "OK");
                ShowAlertBottomSheet("Reschedule Medical Feed Item", "An error occurred while rescheduling medical feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task UndoSnoozeTrainingSessionFeedItem(FeedItem feedItem)
        {
            try
            {
                await ExerciseApi.Net7.ExerciseApi.UndoSkipTrainingSeseesion(feedItem.TrainingSessionFeedItem.TraningSession.Id);

            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Undo Snooze Training Session Feed Item", "An error occurred while trying to undo the snooze of training session feed items.", "OK");
                ShowAlertBottomSheet("Undo Snooze Training Session Feed Item", "An error occurred while trying to undo the snooze of training session feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task UndoSnoozeNutrientFeedItem(FeedItem feedItem)
        {
            try
            {
                await NutritionApi.SnoozeMealUndo(feedItem.NutrientsFeedItem.Meal.MealId);
            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Undo Snooze Nutrient Feed Item", "An error occurred while trying to undo the nutrient feed items.", "OK");
                ShowAlertBottomSheet("Undo Snooze Nutrient Feed Item", "An error occurred while trying to undo the nutrient feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task UndoSkipSupplementFeedItem(FeedItem feedItem)
        {
            try
            {
                foreach (SupplementEntry supplementEntry in feedItem.SupplementFeedItem.SupplementEntries)
                {
                    await SupplementApi.Net7.SupplementApi.UndoSkipSupplement(supplementEntry.DoseId);
                }
            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Undo Skip Supplement Feed Item", "An error occurred while trying to undo the skip of supplement feed items.", "OK");
                ShowAlertBottomSheet("Undo Skip Supplement Feed Item", "An error occurred while trying to undo the skip of supplement feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task UndoSnoozeHabitFeedItem(FeedItem feedItem)
        {
            try
            {
            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Undo Snooze Habit Feed Item", "An error occurred while trying to undo the habit feed items.", "OK");
                ShowAlertBottomSheet("Undo Snooze Habit Feed Item", "An error occurred while trying to undo the habit feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task UndoSnoozeMedicalFeedItem(FeedItem feedItem)
        {
            try
            {

            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Undo Snooze Habit Feed Item", "An error occurred while trying to undo the habit feed items.", "OK");
                ShowAlertBottomSheet("Undo Snooze Habit Feed Item", "An error occurred while trying to undo the habit feed items.", "OK");
            }
            finally
            {

            }
        }
        #endregion

        #region SKIPPING
        public async Task SkipCurrentFeedItem(SupplementPageViewModel supplementPageViewModel)
        {
            ShowLoadingActivityIndicator();

            switch (_feedItem.ItemType)
            {
                case FeedItemType.TrainingSessionFeedItem:
                    try
                    {
                        await ExerciseApi.Net7.ExerciseApi.SkipTrainingSession(_feedItem.TrainingSessionFeedItem.TraningSession.Id, "");
                    }
                    catch
                    {
                        ShowAlertBottomSheet("Error", "Error skipping training session.", "OK");
                    }
                    break;

                case FeedItemType.NutrientsFeedItem:
                    try
                    {
                        await ImageApi.Net7.NutritionApi.SkipMeal(_feedItem.NutrientsFeedItem.Meal.MealId);
                    }
                    catch
                    {
                        ShowAlertBottomSheet("Error", "Error skipping meal.", "OK");
                    }
                    break;

                case FeedItemType.SupplementItem:

                    //nothing at the moment
                    try
                    {
                        if(supplementPageViewModel == null)
                        {

                            foreach(SupplementEntry supplement in _feedItem.SupplementFeedItem.SupplementEntries)
                            {
                                await SupplementApi.Net7.SupplementApi.SkipSupplement(supplement.DoseId);
                            }

                        }
                        else if(supplementPageViewModel != null)
                        {
                            await SupplementApi.Net7.SupplementApi.SkipSupplement(supplementPageViewModel.SupplementDoseId);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        ShowAlertBottomSheet("Error", "Error skipping supplement.", "OK");
                    }                    
                    break;

                case FeedItemType.HabitsFeedItem:

                    //nothing at the moment
                    break;

                case FeedItemType.MedicationFeedItem:

                    //nothing at the moment
                    break;

                default:
                    break;

            }
            await RefreshPageWithoutClearingFeedItem();
        }
        public async Task OpenSkipPopup()
        {
            await CloseMenuPopup();
            DisplaySkipPopup = "inline";
            await StateHasChanged();
        }
        public async Task CloseSkipPopup(bool openMenuPopUp = true)
        {
            if(openMenuPopUp == true)
            {
                await OpenMenuPopup();
            }
            
            DisplaySkipPopup = "none";
            await StateHasChanged();
        }


        #endregion

        public void ShowErrorTemplatePopup(string header, string label)
        {
            if(HTMLBridge.TemplateErrorLabel!=null && HTMLBridge.TemplateErrorHeader!=null && HTMLBridge.TemplateErrorPopup != null)
            {
                HTMLBridge.TemplateErrorHeader.Text = header;
                HTMLBridge.TemplateErrorLabel.Text = label;
                HTMLBridge.TemplateErrorPopup.State = DevExpress.Maui.Controls.BottomSheetState.HalfExpanded;
            }
        }

        private async void HandleFeedItemUndoButtonClick(FeedItem feedItem)
        {
            is_user_interaction = true;
            _isFeedItemUndoClick = true;

            switch (feedItem.ItemType)
            {
                case FeedItemType.TrainingSessionFeedItem:

                    await UndoSnoozeTrainingSessionFeedItem(feedItem);
                    break;

                case FeedItemType.NutrientsFeedItem:

                    await UndoSnoozeNutrientFeedItem(feedItem);
                    break;

                case FeedItemType.SupplementItem:

                    await UndoSkipSupplementFeedItem(feedItem);
                    break;

                case FeedItemType.HabitsFeedItem:

                    await UndoSnoozeHabitFeedItem(feedItem);
                    break;

                case FeedItemType.MedicationFeedItem:

                    await UndoSnoozeMedicalFeedItem(feedItem);
                    break;

                default:
                    break;

            }
            await RefreshPage();
            is_user_interaction = false;
        }

        private async void HandleFeedItemStartButtonClick(FeedItem selectedFeedItem)
        {
            try
            {
                is_user_interaction = true;
                _feedItem = selectedFeedItem;
                _isFeedItemStartClick = true;
                switch (selectedFeedItem.ItemType)
                {
                    case FeedItemType.TrainingSessionFeedItem:

                        await ExerciseApi.Net7.ExerciseApi.StartTrainingSession(selectedFeedItem.TrainingSessionFeedItem.TraningSession.Id);
                        await ViewTrainingSessionDetail((selectedFeedItem.TrainingSessionFeedItem));
                        break;

                    case FeedItemType.NutrientsFeedItem:
                        break;

                    case FeedItemType.HabitsFeedItem:
                        break;

                    case FeedItemType.SupplementItem:

                        await ViewSupplementFeedItemDetails(selectedFeedItem);
                        break;

                    default:

                        //No calls yet
                        //await App.Current.MainPage.DisplayAlert("Retrieve Feed Item", "An error occurred while viewing feed items.", "OK");
                        ShowAlertBottomSheet("Retrieve Feed Item", "An error occurred while viewing feed items.", "OK");
                        break;
                }
                is_user_interaction = false;
            }
            catch
            {
                //await App.Current.MainPage.DisplayAlert("Start Feed Item", "An error occurred while starting feed items.", "OK");
                ShowAlertBottomSheet("Start Feed Item", "An error occurred while starting feed items.", "OK");
            }
            finally
            {
            }
        }

        private static bool isblocked = false;
        private async Task ViewTrainingSessionDetail(TrainingSessionFeedItem trainingSessionFeedItem)
        {
            if (!isblocked)
            {
                isblocked = true;
                ViewTrainingSessionContentPage.DoApiCalls = false;
                ShowLoadingActivityIndicator();

                ViewTrainingSessionContentPage.ExerciseViewModels = new ObservableCollection<ExercisePageViewModel>();
                ViewTrainingSessionContentPage TrainingPage = new ViewTrainingSessionContentPage(trainingSessionFeedItem.TraningSession);

                await App.Current.MainPage.Navigation.PushAsync(TrainingPage, true);

                await Task.Delay(10);
                foreach (var e in trainingSessionFeedItem.TraningSession.emExercises)
                {
                    await Task.Delay(1);
                    await TrainingPage.LoadExerciseInCurrentList(e);
                    
                }

                TrainingPage.RefreshProgressLabel();

                ViewTrainingSessionContentPage.DoApiCalls = true;
                isblocked = false;

                TrainingPage.HideLoadingActivity();
            }
        }

        private async void ViewTrainingSessionSummaryDetail(TrainingSessionFeedItem trainingSessionFeedItem)
        {
            is_user_interaction = true;
            _isFeedItemSummaryClick = true;

            ShowLoadingActivityIndicator();

            await App.Current.MainPage.Navigation.PushAsync(new ViewSummaryTrainingSessionContentPage(trainingSessionFeedItem.TraningSession.Id), true);
            is_user_interaction = false;

        }

        private async Task SelectDateOnChange(string selectedDate)
        {
            string formattedDate = string.Empty;
            string numberSuffix = string.Empty;
            string monthShort = string.Empty;
            DateTime dateTimeParsed;

            _isChangeDateButtonDisable = true;

            if (!DateTime.TryParseExact(selectedDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTimeParsed))
            {
                //await App.Current.MainPage.DisplayAlert("Date Parse Error", "An error occurred while parsing date picker.", "OK");
                ShowAlertBottomSheet("Date Parse Error", "An error occurred while parsing date picker.", "OK");
                _isChangeDateButtonDisable = false;
            }
            else
            {
                DateTime date = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                _dateSelected = date;

                monthShort = _dateSelected.ToString("MMM");

                numberSuffix = GetDayNumberSuffix(_dateSelected);

                formattedDate = string.Format("{0} {1}, {2}", _dateSelected.Day + numberSuffix, monthShort, _dateSelected.DayOfWeek);

                _selectedDate = formattedDate;

                _beforeFeedItems.Clear();
                _nowFeedItems.Clear();
                _laterFeedItems.Clear();

                await GetFeedItems();
                _isChangeDateButtonDisable = false;

                await StateHasChanged();
            }

        }

        protected new async Task StateHasChanged()
        {
            base.StateHasChanged();
        }

        private async Task LoadBrowserDimensions()
        {
            BrowserServices browserServices = new BrowserServices(JSRuntime);
            var dimension = await browserServices.GetDimensions();
            _InnerHeight = dimension.Height;
            _InnerWidth = dimension.Width;

            HTMLBridge.BrowserInnerHeight = _InnerHeight;
            HTMLBridge.BrowserInnerWidth = _InnerWidth;

            await InitializeFeedItemListSheetHeight();
        }

        private async Task InitializeFeedItemListSheetHeight()
        {
            _sheetFeedItemListInitialHeight = _InnerHeight - 260;
            _sheetFeedItemListMaxHeight = _InnerHeight - 32;
            this.SheetFeedItemListHeight = _sheetFeedItemListInitialHeight;
            await StateHasChanged();
        }

        public void LockScrollingToNow()
        {
            // LockScroll will prevent the first call to scrolltonow. It resets automatically
            LockScroll = true;
        }
        public async Task RefreshPage()
        {
            try
            {
                _beforeFeedItems.Clear();
                _nowFeedItems.Clear();
                _laterFeedItems.Clear();
                _feedItem = null;

                await CloseNutrientPopup();
                await CloseAddNewPopup();
                await CloseMenuPopup();
                await GetFeedItems();


                await JSRuntime.InvokeVoidAsync("HideExerciseDetail");
                _isBlackCoverDivHidden = true;
                _isFeedItemDetailsDivHidden = true;

                HideLoadingActivityIndicator();

                GetGreeting();

                await StateHasChanged();


                await Task.Delay(5);
                await ScrollDivToNowSection();
                await JSRuntime.InvokeVoidAsync("SetUnderlineOnInitialize");

                if (HTMLBridge.RefreshMenu != null)
                {
                    HTMLBridge.RefreshMenu.Invoke(this, null);
                }

            }
            catch
            {
                //Log Error
            }
            finally
            {

            }
        }
        public async Task RefreshPageWithoutClearingFeedItem()
        {
            //does not close popups or clear feed item
            try
            {
                _beforeFeedItems.Clear();
                _nowFeedItems.Clear();
                _laterFeedItems.Clear();

                await GetFeedItems();

                //_isBlackCoverDivHidden = true;
                //_isFeedItemDetailsDivHidden = true;

                HideLoadingActivityIndicator();

                //  GetGreeting();

                await StateHasChanged();

                await JSRuntime.InvokeVoidAsync("SetUnderlineOnInitialize");

                if (HTMLBridge.RefreshMenu != null)
                {
                    HTMLBridge.RefreshMenu.Invoke(this, null);
                }

            }
            catch
            {
                //Log Error
            }
            finally
            {

            }
        }

        private async void AddNewDailyExercise()
        {
            try
            {
                bool dailyExerciseCreatedSuccessfully = await ExerciseApi.Net7.ExerciseApi.CreateRandomDailyExercise(FeedApi.Net7.FeedApi.FkFederatedUser, DateTimeOffset.Now.Offset.Hours);

                if (dailyExerciseCreatedSuccessfully == true)
                {

                }
                else
                {
                    //await App.Current.MainPage.DisplayAlert("Add Daily Exercise", "The system failed to create a new daily exercise", "OK");
                    ShowAlertBottomSheet("Add Daily Exercise", "The system failed to create a new daily exercise", "OK");
                }
                await RefreshPage();
            }
            catch (Exception ex)
            {
                //await App.Current.MainPage.DisplayAlert("Add Daily Exercise", "An error occurred while adding daily exercise.", "OK");
                ShowAlertBottomSheet("Add Daily Exercise", "An error occurred while adding daily exercise.", "OK");
            }
            finally
            {
            }
        }

        private void GetGreeting()
        {
            DateTime dateNow = DateTime.Now;
            int hours = dateNow.Hour;

            string username = string.Empty;

            if (string.IsNullOrWhiteSpace(MiddleWare.UserName) == true)
            {
                username = "Guest";
            }
            else
            {
                username = MiddleWare.UserName;
            }

            if (hours > 5 && hours < 12)
            {
                _greeting = string.Format("Good Morning {0}", username);
            }
            else if (hours >= 12 && hours < 18)
            {
                _greeting = string.Format("Good Afternoon {0}", username);
            }
            else if (hours >= 18 && hours <= 23)
            {
                _greeting = string.Format("Good Evening {0}", username);
            }
            else
            {
                _greeting = string.Format("Good Night {0}", username);
            }
        }

        private void ShowLoadingActivityIndicator()
        {
            if (HTMLBridge.MainPageBlackStackLayout != null)
            {
                HTMLBridge.MainPageBlackStackLayout.IsVisible = true;
            }
            if (HTMLBridge.MainPageLoadingActivityIndicator != null)
            {
                HTMLBridge.MainPageLoadingActivityIndicator.IsVisible = true;
            }
        }

        private void HideLoadingActivityIndicator()
        {
            if (HTMLBridge.MainPageBlackStackLayout != null)
            {
                HTMLBridge.MainPageBlackStackLayout.IsVisible = false;
            }
            if (HTMLBridge.MainPageLoadingActivityIndicator != null)
            {
                HTMLBridge.MainPageLoadingActivityIndicator.IsVisible = false;
            }
        }

#endregion

        #region Photos

        public async Task TakePhoto()
        {
            bool IsCameraAllowed = await CheckCameraPermissions();
            if (IsCameraAllowed)
            {
                try
                {
                    string temp = await HandleImageAndSetNutrientImage(true);
                    if (temp != null && temp != "")
                    {
                        await OpenAddDishPopup(temp);
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                OpenRequestCameraPermissionsPopup();
            }
        }

        public async Task UploadPhoto()
        {
            bool IsFileAllowed = await CheckFilePermissions();
            if (IsFileAllowed)
            {
                try
                {
                    string temp = await Index.HandleImageAndSetNutrientImage(false);
                    if (temp != null && temp != "")
                    {
                        await OpenAddDishPopup(temp);
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                OpenRequestFilePermissionsPopup();
            }
        }

        public static async Task<string> HandleImageAndSetNutrientImage(bool isCamera)
        {
            try
            {
                // FileResult photo = null;
                IMediaFile photo = null;
                int orientation = -1;
                MediaPickResult PickedImage = null;
                if (isCamera)
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {
                        photo = await MediaGallery.CapturePhotoAsync();
                        // photo = await MediaPicker.Default.CapturePhotoAsync();

                    }
                    else
                    {
                        // Show messagebox that caputure is not supported
                    }
                }
                else
                {
                    PickedImage = await MediaGallery.PickAsync(1, MediaFileType.Image);
                }

                Microsoft.Maui.Graphics.IImage image = null;

                if (PickedImage != null && PickedImage.Files.Count() > 0)
                {
                    using (Stream file = await PickedImage.Files.First().OpenReadAsync()) //.FullPath, FileMode.Open);
                    {
                        image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(file);
                    }
                }

                if (photo != null)
                {
#if ANDROID
                    try
                    {
                        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(await photo.OpenReadAsync());
                        ExifIfd0Directory subIfd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                        orientation = subIfd0Directory.TryGetInt32(ExifDirectoryBase.TagOrientation, out int value) ? value : -1;
                        //metadata
                        Android.Graphics.Matrix rotationMatrix = new Android.Graphics.Matrix();
                        //System.Diagnostics.Debug.WriteLine(orientation);

                        switch (orientation)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                rotationMatrix.PreRotate(180);
                                break;
                            case 5:
                                rotationMatrix.PreRotate(90);
                                break;
                            case 6:
                                rotationMatrix.PreRotate(90);
                                break;
                            case 7:
                                rotationMatrix.PreRotate(-90);
                                break;
                            case 8:
                                rotationMatrix.PreRotate(-90);
                                break;
                        }

                        using (Stream file = await photo.OpenReadAsync()) //.FullPath, FileMode.Open);
                        {
                            Android.Graphics.Bitmap androidImage = await BitmapFactory.DecodeStreamAsync(file);
                            Android.Graphics.Bitmap normalisedBitmap = Android.Graphics.Bitmap.CreateBitmap(androidImage, 0, 0, androidImage.Width, androidImage.Height, rotationMatrix, true);

                            using (MemoryStream resultStream = new MemoryStream())
                            {
                                await normalisedBitmap.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, resultStream);
                                resultStream.Position = 0;
                                image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(resultStream);
                            }
                        }
                    }
                    catch(Exception e)
                    {

                    }
#endif
                    if (image == null)
                    {
                        using (Stream file = await photo.OpenReadAsync()) //.FullPath, FileMode.Open);
                        {
                            image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(file);
                        }
                    }
                }

                if (image == null)
                {
                    return null;
                }
                else
                {

                    int maxImageSize = 1600;
                    if (image.Width > maxImageSize || image.Height > maxImageSize)
                    {
                        image = image.Downsize(maxImageSize, true);
                    }
                    byte[] bytes;
#if IOS
                    UIImage iosImage = image.AsUIImage();
                    UIImage normalizedImage = iosImage.NormalizeOrientation();
                    NSData compressedNormalizedImageData = normalizedImage.AsJPEG(new nfloat(0.8));
                    bytes = compressedNormalizedImageData.ToArray();
#else
                    float precision = 0.8f;
                    bytes = await image.AsBytesAsync(Microsoft.Maui.Graphics.ImageFormat.Jpeg, precision);
#endif
                    Index.NutrientImageType = ".jpeg";


                    Index.NutrientImageData = Convert.ToBase64String(bytes);
                }
                return "isphoto";
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /*
        public byte[] ConvertToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }*/



#if WINDOWS
        public static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                ImageConverter imgCon = new();
                return (byte[])imgCon.ConvertTo(imageIn, typeof(byte[]));

                //  System.Drawing.Imaging.EncoderParameters param = new System.Drawing.Imaging.EncoderParameters() { Param}
                // imageIn.Save(ms, imageIn.RawFormat, System.Drawing.Imaging.EncoderParameters.);
                //  return ms.ToArray();
            }
        }
#endif
#endregion

        public static bool IsBackDisabled()
        {
            return DisplayNutrientPopup == "inline" ||
            DisplayAddDishPopup == "inline" ||
            DisplayMindfulnessPopup == "inline" ||
            DisplayAddNewPopup == "inline" ||
            DisplayFavoritePopup == "inline";
        }
        public static void ReEnableBack()
        {
            //does not work. why?
            DisplayNutrientPopup = "none";
            DisplayAddDishPopup = "none";
            DisplayMindfulnessPopup = "none";
            DisplayAddNewPopup = "none";
            DisplayFavoritePopup = "none";
        }
        #region permissions
        public static async Task<bool> CheckCameraPermissions()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown || status == PermissionStatus.Disabled)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void OpenRequestCameraPermissionsPopup()
        {
            if (HTMLBridge.CameraPermissionPopup != null)
            {
                HTMLBridge.CameraPermissionPopup.State = DevExpress.Maui.Controls.BottomSheetState.HalfExpanded;
            }
        }

        public async Task RequestCameraPermissions()
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.Camera>();
            if (HTMLBridge.CameraPermissionPopup != null)
            {
                HTMLBridge.CameraPermissionPopup.State = DevExpress.Maui.Controls.BottomSheetState.Hidden;
            }
            if (status != PermissionStatus.Denied)
            {
                await Task.Delay(1000);
                await TakePhoto();
            }
        }

        public async Task<bool> CheckFilePermissions()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown || status == PermissionStatus.Disabled)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void OpenRequestFilePermissionsPopup()
        {
            if (HTMLBridge.StoragePermissionPopup != null)
            {
                HTMLBridge.StoragePermissionPopup.State = DevExpress.Maui.Controls.BottomSheetState.HalfExpanded;
            }
        }

        public async Task RequestFilePermissions()
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.Photos>();
            if (HTMLBridge.StoragePermissionPopup != null)
            {
                HTMLBridge.StoragePermissionPopup.State = DevExpress.Maui.Controls.BottomSheetState.Hidden;
            }
            if (status != PermissionStatus.Denied)
            {
                await Task.Delay(1000);
                await UploadPhoto();
            }
        }

        public static async Task<bool> CheckNotificationPermissions()
        {

#if ANDROID
            PermissionStatus status = await Permissions.CheckStatusAsync<PushNotificationPermissionsAndroid>();
            if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown || status == PermissionStatus.Disabled)
            {
                return false;
            }
            else
            {
                return true;
            }
#elif IOS
            PushNotificationPermissionsIOS IOSPermissions = new PushNotificationPermissionsIOS();
            return await IOSPermissions.CheckAsync();
#endif
        }
        public void OpenRequestNotificationPermissionsPopup()
        {
            if (HTMLBridge.NotificationPermissionPopup != null)
            {
                HTMLBridge.NotificationPermissionPopup.State = DevExpress.Maui.Controls.BottomSheetState.HalfExpanded;
            }
        }

        public async Task RequestNotificationPermissions()
        {
#if ANDROID
            PermissionStatus status = await Permissions.RequestAsync<PushNotificationPermissionsAndroid>();
#elif IOS
            PushNotificationPermissionsIOS IOSPermissions = new PushNotificationPermissionsIOS();
            await IOSPermissions.RequestAsync();
#endif
            if (HTMLBridge.NotificationPermissionPopup != null)
            {
                HTMLBridge.NotificationPermissionPopup.State = DevExpress.Maui.Controls.BottomSheetState.Hidden;
            }
            //not async by design
            await UserMiddleware.RegisterDevice(await PushRegistration.CheckPermission(), PushRegistration.GetPlatform());
        }

        private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
            }
        }

        #endregion

    }
}