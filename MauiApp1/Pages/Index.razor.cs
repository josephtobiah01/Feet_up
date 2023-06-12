using FeedApi.Net7.Models;
using ImageApi.Net7;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Areas.Exercise.ViewModels;
using MauiApp1.Areas.Exercise.Views;
using MauiApp1.Areas.Nutrient.Views;
using MauiApp1.Areas.Overview.Views;
using MauiApp1.Business.BrowserServices;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Pages.Nutrient;
using MauiApp1.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Microsoft.Maui.Animations;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using UserApi.Net7.Models;
//using Microsoft.Maui.Graphics.Platform;
//using Microsoft.Maui.Graphics.Platform;


namespace MauiApp1.Pages
{
    public partial class Index
    //in order to be referenced by the parent .razor, class has to be PARTIAL
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        #region [Fields]
        //navigation
        private IDisposable registration;
        //For Ui Fields
        public static string DisplayFooter = "none";
        public string DisplayNutrientPopup = "none";
        public string DisplayUserPopup = "none";
        public string DisplayAddDishPopup = "none";
        public string DisplayMindfulnessPopup = "none";
        public string DisplayAddNewPopup = "none";
        public string DisplayFavoritePopup = "none";

        private bool _isBlackCoverDivHidden = true;
        private bool _isFeedItemDetailsDivHidden = true;
        private bool _isExerciseWhatsNewTabDiv = true;
        private bool _isChangeDateButtonDisable = false;

        private bool _isFeedItemStartClick = false;
        private bool _isFeedItemSnoozeClick = false;
        private bool _isFeedItemUndoClick = false;
        private bool _isFeedItemSummaryClick = false;

        private bool _isRefreshFeedItemNeeded = false;
        private bool _ReloadFeedItemDetailsPage = true;

        private DateTime _dateSelected;
        private string _selectedDate;

        private List<FeedItem> _beforeFeedItems;
        private List<FeedItem> _nowFeedItems;
        private List<FeedItem> _laterFeedItems;

        private FeedItem _feedItem;
        //private EdsTrainingSession _edsTrainingSession;

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
      //  public static FileResult tmpPhoto = null;


        private int _sheetHeight = 492;
        private double _sheetFeedItemListInitialHeight = 0;
        private double _sheetFeedItemListMaxHeight = 0;
        private double _sheetFeedItemListHeight = 295;
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
        private bool _feedItemListDragging = false;
        private bool _feedItemListSnoozing = false;

        private bool _doScrollToNow = true;
        private double _InnerHeight { get; set; }
        private double _InnerWidth { get; set; }

        private string _greeting { get; set; }

        //for TestMode Set in MainPage.xml.cs
        bool _TestMode = true;

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

        /* this method is run after page is rendered */
            /* more detail on this and onafterrenderasync at https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-7.0#after-component-render-onafterrenderasync */
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("renderJqueryComponentsinIndex");
                await JSRuntime.InvokeVoidAsync("JavaScriptInteropDatepicker");
                await JSRuntime.InvokeVoidAsync("blazorjs.dragable");
                await JSRuntime.InvokeVoidAsync("blazorjsFeedItemContentGroupDrag.dragable");
                await LoadBrowserDimensions();
                await IntializeData();
#if DEBUG
                registration = NavigationManager.RegisterLocationChangingHandler(LocationChangingHandler);
#endif
            }

            //if (_feedItemListDragging == false && _feedItemListSnoozing == false)
            //{
            //    await ScrollDivToNowSection();
            //    await SetFeedItemListLastScrollValue();
            //}

        }
        //protected override void OnAfterRender(bool firstRender)
        //{
        //base.OnAfterRender(firstRender);
        //    if (firstRender)
        //    {
        //    }

        //    if (_feedItemListDragging == false && _feedItemListSnoozing == false)
        //    {
        //            ScrollDivToNowSection();
        //            SetFeedItemListLastScrollValue();
        //    }
        //}
        #endregion

        #region [Methods :: EventHandlers :: Class]
        // backbutton override
        private ValueTask LocationChangingHandler(LocationChangingContext arg)
        {
#if DEBUG
            if(DisplayFooter == "inline"||
            DisplayNutrientPopup == "inline"||
            DisplayUserPopup == "inline"||
            DisplayAddDishPopup == "inline"||
            DisplayMindfulnessPopup == "inline"||
            DisplayAddNewPopup == "inline"||
            DisplayFavoritePopup == "inline"
                )
            { 
                arg.PreventNavigation();
                DisplayFooter = "none";
                DisplayNutrientPopup = "none";
                DisplayUserPopup = "none";
                DisplayAddDishPopup = "none";
                DisplayMindfulnessPopup = "none";
                DisplayAddNewPopup = "none";
                DisplayFavoritePopup = "none";
            }
#endif
            return ValueTask.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            //  await IntializeData();
           // await Task.Delay(1);
        }

        private async Task IntializeData()
        {
            try
            {
              //  _doScrollToNow = true;
                _feedItemListDragging = false;
                _nowFeedItems = new List<FeedItem>();
                _beforeFeedItems = new List<FeedItem>();
                _laterFeedItems = new List<FeedItem>();

                _dateSelected = DateTime.Now;

                InitializeSelectDate();

                await GetFeedItems();

                GetGreeting();

                await StateHasChanged();

                if (HTMLBridge.RefreshData != null)
                {
                    HTMLBridge.RefreshData -= RefreshData_OnRefresh;
                }

                HTMLBridge.RefreshData += RefreshData_OnRefresh;

                //    ScrollDivToNowSection();

                InitializeControl();

               // _TestMode = Preferences.Default.Get("TestMode", false);
              //  _doScrollToNow = false;
            }
            catch (Exception ex)
            {
                //Log error
            }
            finally
            {

            }

        }

        private void InitializeControl()
        {
           
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
        private void NewButton_Click()
        {
            OpenAddNewPopup();
            //AddNewDailyExercise();
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

        private void FeedItem_Click(FeedItem feedItem)
        {
            HandleFeedItemClick(feedItem);
        }


        private void TabItemExerciseWhatsInsideInput_Click()
        {
            _isExerciseWhatsNewTabDiv = false;
        }

        private void TabItemExerciseHistoryInput_Click()
        {
            _isExerciseWhatsNewTabDiv = true;

        }

        private void BlackCoverDiv_Click()
        {
            BlackCoverDivToggle();
            //RefreshPage();
        }

        private void FeedItemSnooze_Click(FeedItem feedItem)
        {
            HandleFeedItemSnoozeButtonClick(feedItem);
        }

        private void FeedItemDetailsStart_Click(FeedItem feedItem)
        {
            HandleFeedItemStartButtonClick(feedItem);
        }

        private async void FeedItemListSheet_OnTouchMove(Microsoft.AspNetCore.Components.Web.TouchEventArgs args)
        {
            _feedItemListDragging = true;

            await JSRuntime.InvokeVoidAsync("ResetScrollCounter");

            //--> I guess this is where I should update my dragitem position
            //--> Normal drag via mouse automatically updates position, but not touchMove
            this.SheetFeedItemListHeight = Convert.ToInt32(_InnerHeight - args.ChangedTouches[0].ClientY);

            if (this.SheetFeedItemListHeight <= _sheetFeedItemListInitialHeight)
            {
                this.SheetFeedItemListHeight = _sheetFeedItemListInitialHeight;
            }

            if (this.SheetFeedItemListHeight >= _sheetFeedItemListMaxHeight)
            {
                this.SheetFeedItemListHeight = _sheetFeedItemListMaxHeight;
            }
        }

        private async void FeedItemListSheet_OnTouchLeave(Microsoft.AspNetCore.Components.Web.TouchEventArgs args)
        {
            _feedItemListDragging = false;
            await SetFeedItemListLastScrollValue();
        }

        private void FeedItemDetailsBottomSheet_OnTouchMove(Microsoft.AspNetCore.Components.Web.TouchEventArgs args)
        {
            int maxSheetDragHeight = (int)((_InnerHeight / 100) * 86);
            //double x = args.ChangedTouches[0].ClientX;
            //double y = args.ChangedTouches[0].ClientY;
            //--> I guess this is where I should update my dragitem position
            //--> Normal drag via mouse automatically updates position, but not touchMove
            _sheetHeight = Convert.ToInt32(_InnerHeight - args.ChangedTouches[0].ClientY);

            if (_sheetHeight <= 492)
            {
                _sheetHeight = 492;
            }

            if (_sheetHeight >= maxSheetDragHeight)
            {
                _sheetHeight = maxSheetDragHeight;
            }
        }

        private void CheckFeedItem_OnScroll()
        {
             JSRuntime.InvokeVoidAsync("CheckScroll");
        }

        private async void FeedButton_Click()
        {
            GoToCurrentDate();
        }

        private async void DashboardButton_Click()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HomeContentPage());
        }

        private async void BiodataButton_Click()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HomeContentPage());
        }

        private async void Dashboard_Click()
        {
            GoToCurrentDate();
        }
        private async void ChatButton_Click()
        {

#if IOS
            await Application.Current.MainPage.Navigation.PushAsync(new ViewIOSChatContentPage());
#else
           await Application.Current.MainPage.Navigation.PushAsync(new ViewChatContentPage());
#endif
            //await Application.Current.MainPage.Navigation.PushAsync(new ViewChatContentPage());
        }
        public async void GoToChatPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage());
            CloseAddNewPopup();
        }
        private async void GotoScanPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new BarcodeScannerContentPage());
        }
        public async void GoToSearchRecipesPage(FeedItem feeditem, long status)
        {
            //if status==1, display search;
            //if status ==2, display favorites;
            //if status ==2, display history;
            await App.Current.MainPage.Navigation.PushAsync(new SearchRecipesPage(feeditem, status));
        }
        public async void GoToOverviewPage(FeedItem feeditem, bool IsSubmitted = false)
        {
            //NutritionUploadModel = new List<NutritionUploadModel>();
            //tmpPhoto = null;
            var Page = new OverviewPage(feeditem, IsSubmitted);
            //Page.CLosing += Page_CLosing;
            //Page.Unloaded += Page_Unloaded;
            //Page.Disappearing += Page_Disappearing;
            //Page.Unfocused += Page_Unfocused;
            await App.Current.MainPage.Navigation.PushAsync(Page);
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
        public void OpenUserPopup()
        {
            DisplayUserPopup = "inline";
        }
        public void CloseUserPopup()
        {
            DisplayUserPopup = "none";
        }
        public async Task OpenNutrientPopup(FeedItem currentfeeditem,bool IsCustomDish=false)
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
                NutrientPopupCurrentFeedItem = temporaryFeedItem;
            }
            else
            {
                NutrientPopupCurrentFeedItem = currentfeeditem;
            }
            Index.NutritionUploadModel = new List<NutritionUploadModel>();

            NutrientPopupRecipesDisplayed = await ImageApi.Net7.NutritionApi.GetFavoritesAndHistory();
            //for(int i=0;i< NutrientPopupRecipesDisplayed.History.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine(NutrientPopupRecipesDisplayed.History[i].RecipeName);
            //}
            DisplayNutrientPopup = "inline";
        }
        public void CloseNutrientPopup()
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
                //NutrientCustomDishImageUrl =  NutrientImageData;
            }
        }
       
        public async Task CloseAddDishPopup()
        {
            NutrientPopupRecipesDisplayed = await ImageApi.Net7.NutritionApi.GetFavoritesAndHistory();
            DisplayAddDishPopup = "none";
        }
        public void OpenFavoritePopup()
        {
            DisplayFavoritePopup = "inline";
        }
        public void CloseFavoritePopup(){

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
                    await App.Current.MainPage.DisplayAlert("Error", "Please enter a name for the dish.", "OK");
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
            if (NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.DishesEaten == null && NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.IsCustom!=true)
            {
                NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.DishesEaten = await ImageApi.Net7.NutritionApi.GetMealDishes(NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId);
            }
            NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.DishesEaten.Add(dishAdded);
            //go to overview
            GoToOverviewPage(NutrientPopupCurrentFeedItem);
            await CloseAddDishPopup();
            CloseNutrientPopup();
            CloseAddNewPopup();
        }
        public async Task FavoriteDish()
        {
            NutrientIsFavorite = !NutrientIsFavorite;
            if (NutrientIsFavorite)
            {
                if (ParentMiddleWare.MiddleWare.ShowFavoriteMsg)
                {
                    OpenFavoritePopup();
                }
            }
            if (NutrientIsFavorite && NutrientRecipe!=null)
            {
                bool IsSuccessful=await ImageApi.Net7.NutritionApi.FavoriteDish(NutrientRecipe.RecipeID);
                await StateHasChanged();
            }
            else
            {
                await ImageApi.Net7.NutritionApi.UnFavoriteDish(NutrientRecipe.RecipeID);
                await StateHasChanged();
            }
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

        private void OpenAddNewPopup()
        {
            DisplayAddNewPopup = "inline";
        }
        private void CloseAddNewPopup()
        {
            DisplayAddNewPopup = "none";
        }
        public void Dispose()
        {
            registration?.Dispose();
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

        private async Task GetFeedItems()
        {
            await PushRegistration.CheckPermission();


            isblocked = false;
            if (!MiddleWare.IsInit)
            {
              //  Trace.TraceError("Starting Middleware.init");
                //  MiddleWare.BaseUrl = "https://fitapp-mainapi-test.azurewebsites.net";
                //// FeedApi.Net7.FeedApi.BaseUrl = "https://localhost:7174";
                MiddleWare.UserID = -100;
                await SetupUser();
                
                //var config = await FeedApi.Net7.FeedApi.GetConfig();
                //MiddleWare.NowLaterTime = config[0];
                //MiddleWare.AutoSkippedTimeout = config[1];
                //MiddleWare.OverDueTime = config[2];
                //MiddleWare.TestKitStatus = config[3];
                MiddleWare.IsInit = true;
                // Trace.TraceError("Finish Middleware.init");              
            }

            _beforeFeedItems = new List<FeedItem>();
            _nowFeedItems = new List<FeedItem>();
            _laterFeedItems = new List<FeedItem>();

            try
            {
             //   Trace.TraceError("About to get feeditems");
                List<FeedItem> feedItems = await FeedApi.Net7.FeedApi.GetDailyFeedAsync(_dateSelected);
                

                //feedItems = InejectSupplementsAndNutrients(feedItems);
                var nowIsFilled = false;
                if (_dateSelected.Date != DateTime.Now.Date) nowIsFilled = true;
                foreach (FeedItem feedItem in feedItems)
                {
                    //feedItem.Status = FeedItemStatus.Scheduled;
                    // if (feedItem.Date - DateTime.Now <= TimeSpan.FromMinutes(120) && nowIsFilled==false)
                    //if (feedItem.Date - DateTime.Now <= TimeSpan.FromMinutes(16))
                    if (feedItem.Date <= DateTime.Now.AddMinutes(-1))
                    {
                        _beforeFeedItems.Add(feedItem);
                    }
                    else if (feedItem.Date < DateTime.Now.AddMinutes(15)
                        && feedItem.Status != FeedItemStatus.Completed && feedItem.Status != FeedItemStatus.Skipped)
                    {
                        _nowFeedItems.Add(feedItem);
                        nowIsFilled = true;
                    }
                    else
                    {
                        if (!nowIsFilled)
                        {
                            _nowFeedItems.Add(feedItem);
                            nowIsFilled = true;
                        }
                        else
                        {
                            _laterFeedItems.Add(feedItem);
                        }
                    }
                }
               // await ScrollDivToNowSection();
                await SetFeedItemListLastScrollValue();
            }
            catch (Exception ex)
            {
                // Log Error
                //await App.Current.MainPage.DisplayAlert("Retrieve Feed Item", ex.Message + ex.StackTrace, "OK");
                await App.Current.MainPage.DisplayAlert("Retrieve Feed Item", "An error occurred while retrieving feed items. Please check internet connection and try again", "OK");

            }
            finally
            {
            }
        }

        private string GetStatuswithFormattedTimeElapsed(FeedItemType feedItemType, DateTime dateTime, FeedItemStatus status)
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

            //ScrollDivToNowSection();
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

            //ScrollDivToNowSection();

        }

        private async void GoToCurrentDate()
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

            //ScrollDivToNowSection();

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

        private void BlackCoverDivToggle()
        {
            if (_isBlackCoverDivHidden == true)
            {
                _isBlackCoverDivHidden = false;
                _isFeedItemDetailsDivHidden = false;
            }
            else
            {
                _isBlackCoverDivHidden = true;
                _isFeedItemDetailsDivHidden = true;
                _ReloadFeedItemDetailsPage = true;
            }

            if(_isRefreshFeedItemNeeded== true)
            {
                RefreshPage();
                _isRefreshFeedItemNeeded = false;
            }
            else
            {

            }
        }

        private async Task ScrollDivToNowSection()
        {
            if (_doScrollToNow)
            {
                
                await JSRuntime.InvokeVoidAsync("setupDebounce");
                await JSRuntime.InvokeVoidAsync("ScrollToNow");
            }
        }

        private async Task SetFeedItemListLastScrollValue()
        {
            await JSRuntime.InvokeVoidAsync("SetFeedItemListLastScroll");
        }

        private bool is_user_interaction = false;
        private void HandleFeedItemClick(FeedItem feedItem)
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
            else
            {
                ViewFeedItemDetail(feedItem);
            }
            is_user_interaction = false;
        }

        private void ViewFeedItemDetail(FeedItem feedItem)
        {
            is_user_interaction = true;
            switch (feedItem.ItemType)
            {
                case FeedItemType.TrainingSessionFeedItem:

                    //_sheetHeight = 495;
                    ViewTrainingSessionFeedItemDetails(feedItem);
                    break;

                case FeedItemType.NutrientsFeedItem:
                    break;

                case FeedItemType.SupplementItem:
                    //_sheetHeight = 600;
                    //_sheetHeight = ((int)_InnerHeight / 100) * 86;
                    break;

                default:
                    break;
            }
            is_user_interaction = false;
            //_isBlackCoverDivHidden = false;
            //_isFeedItemDetailsDivHidden = false;
            //_isExerciseWhatsNewTabDiv = false;

            //_feedItem = feedItem;

        }

        private void ViewTrainingSessionFeedItemDetails(FeedItem feedItem)
        {
            _feedItem = feedItem;
            _sheetHeight = 495;
            _isBlackCoverDivHidden = false;
            _isFeedItemDetailsDivHidden = false;
            _isExerciseWhatsNewTabDiv = false;
        }

        private void ViewSupplementFeedItemDetails(FeedItem feedItem)
        {
            _feedItem = feedItem;
            _sheetHeight = ((int)_InnerHeight / 100) * 86;
            _isBlackCoverDivHidden = false;
            _isFeedItemDetailsDivHidden = false;
            _isExerciseWhatsNewTabDiv = false;
        }

        private async void HandleFeedItemSnoozeButtonClick(FeedItem selectedFeedItem)
        {
            try
            {
                _isFeedItemSnoozeClick = true;
                _feedItemListSnoozing = true;

                string action = await App.Current.MainPage.DisplayActionSheet("Snooze Notification", "Cancel", null,
                  "2 hours", "1 hour", "30 minutes", "15 minutes");
                //FeedItem nowFeedItem = null;

                //nowFeedItem = _nowFeedItems.Where(feedItem => feedItem == selectedFeedItem);

                //  ExerciseApi.Net7.ExerciseApi.RescheduleTrainingSession(selectedFeedItem.TrainingSessionFeedItem.TraningSession.Id, )
                //    RescheduleTrainingSession

                int wait = 0;
               // for (int index = 0; index < _nowFeedItems.Count(); index++)  !!!!!!!!!!!!!!!!!!!!!!!!
                {
                 //   if (_nowFeedItems[index] == selectedFeedItem)
                    {
                        switch (action)
                        {
                            case "15 minutes":
                                wait = 15;
                                selectedFeedItem.Date.AddMinutes(15);
                                //    _nowFeedItems[index].Date = _nowFeedItems[index].Date.AddMinutes(15);  // !!!!!!!
                                break;

                            case "30 minutes":
                                wait = 30;
                                selectedFeedItem.Date.AddMinutes(30);
                                //    _nowFeedItems[index].Date = _nowFeedItems[index].Date.AddMinutes(30);
                                break;

                            case "1 hour":
                                wait = 60;
                                selectedFeedItem.Date.AddMinutes(60);
                                //  _nowFeedItems[index].Date = _nowFeedItems[index].Date.AddMinutes(60);
                                break;

                            case "2 hours":
                                wait = 120;
                                selectedFeedItem.Date.AddMinutes(120);
                                //  _nowFeedItems[index].Date = _nowFeedItems[index].Date.AddMinutes(120);
                                break;

                            default:
                                break;
                        }
                    }
                 //   else
                    {
                        // Do nothing continue the loop
                    }
                }

                if (wait > 0)
                {
                    await ShowLoadingActivityIndicator();

                    switch (selectedFeedItem.ItemType)
                    {
                        case FeedItemType.TrainingSessionFeedItem:

                           await SnoozeTrainingSessionFeedItem(selectedFeedItem, wait);
                            break;

                        case FeedItemType.NutrientsFeedItem:

                            await SnoozeNutrientFeedItem(selectedFeedItem, wait);
                            break;

                        case FeedItemType.SupplementItem:

                            await SnoozeSupplementFeedItem(selectedFeedItem, wait);
                            break;

                        case FeedItemType.HabitsFeedItem:

                            await SnoozeHabitFeedItem(selectedFeedItem, wait);
                            break;

                        case FeedItemType.MedicationFeedItem:

                            await SnoozeMedicalFeedItem(selectedFeedItem, wait);
                            break;

                        default:
                            break;

                    }

                    _feedItemListSnoozing = false;
                    RefreshPage();
                }

            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Reschedule Feed Item", "An error occurred while rescheduling feed items.", "OK");
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
                        ViewTrainingSessionDetail((selectedFeedItem.TrainingSessionFeedItem));
                        break;

                    case FeedItemType.NutrientsFeedItem:
                        break;

                    case FeedItemType.HabitsFeedItem:
                        break;

                    case FeedItemType.SupplementItem:

                        ViewSupplementFeedItemDetails(selectedFeedItem);
                        break;

                    default:

                        //No calls yet
                        App.Current.MainPage.DisplayAlert("Retrieve Feed Item", "An error occurred while viewing feed items.", "OK");
                        break;
                }
                is_user_interaction = false;
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Start Feed Item", "An error occurred while starting feed items.", "OK");
            }
            finally
            {
            }
        }

        private static bool isblocked = false;
        private async void ViewTrainingSessionDetail(TrainingSessionFeedItem trainingSessionFeedItem)
        {
            if (!isblocked)
            {
                isblocked = true;

                await ShowLoadingActivityIndicator();

                ViewExerciseContentPage._exerciseViewModels = new ObservableCollection<ExercisePageViewModel>();
                var TraningPage = new ViewExerciseContentPage(trainingSessionFeedItem.TraningSession);
           
                //   await ViewExerciseContentPage.LoadExerciseViewModel(trainingSessionFeedItem.TraningSession);
                await App.Current.MainPage.Navigation.PushAsync(TraningPage, true);

                // this.Loaded += (s, e) =>
                await Task.Delay(10);
                foreach (var e in trainingSessionFeedItem.TraningSession.emExercises)
                {
                    await Task.Delay(1);
                    await ViewExerciseContentPage.LoadExerciseInCurrentList(e);
                }

                isblocked = false;

            }
        }

        private async void ViewTrainingSessionSummaryDetail(TrainingSessionFeedItem trainingSessionFeedItem)
        {
            is_user_interaction = true;
            _isFeedItemSummaryClick = true;

            await ShowLoadingActivityIndicator();

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
                //_selectedDate = DateTime.Now.ToString("dd/MM/yyyy");
                //_dateSelected = DateTime.Now;

                await App.Current.MainPage.DisplayAlert("Date Parse Error", "An error occurred while parsing date picker.", "OK");

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

            await Task.Delay(10);
            if (_feedItemListDragging == false && _feedItemListSnoozing == false)
            {
                await ScrollDivToNowSection();
                await SetFeedItemListLastScrollValue();
            }
        }

        private async Task LoadBrowserDimensions()
        {
            BrowserServices browserServices = new BrowserServices(JSRuntime);
            var dimension = await browserServices.GetDimensions();
            _InnerHeight = dimension.Height;
            _InnerWidth = dimension.Width;

            HTMLBridge.BrowserInnerHeight = _InnerHeight;
            HTMLBridge.BrowserInnerWidth = _InnerWidth;

            InitializeFeedItemListSheetHeight();
        }

        private async Task InitializeFeedItemListSheetHeight()
        {
            _sheetFeedItemListInitialHeight = _InnerHeight - 260;
            _sheetFeedItemListMaxHeight = _InnerHeight -32;
            this.SheetFeedItemListHeight = _sheetFeedItemListInitialHeight;

           await StateHasChanged();
        }

        public async Task RefreshPage()
        {
            try
            {
                _beforeFeedItems.Clear();
                _nowFeedItems.Clear();
                _laterFeedItems.Clear();
                _feedItem = null;

                await GetFeedItems();


                _isBlackCoverDivHidden = true;
                _isFeedItemDetailsDivHidden = true;

                HideLoadingActivityIndicator();

                GetGreeting();

                await StateHasChanged();

                //ScrollDivToNowSection();

                await JSRuntime.InvokeVoidAsync("SetUnderlineOnInitialize");

                if (HTMLBridge.RefreshMenu != null)
                {
                    HTMLBridge.RefreshMenu.Invoke(this, null);
                }

            }
            catch(Exception ex)
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
                bool dailyExerciseCreatedSuccessfully = await ExerciseApi.Net7.ExerciseApi.CreateRandomDailyExercise(FeedApi.Net7.FeedApi.UserID, DateTimeOffset.Now.Offset.Hours);

                if (dailyExerciseCreatedSuccessfully == true)
                {

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Add Daily Exercise", "The system failed to create a new daily exercise", "OK");
                }

                RefreshPage();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Add Daily Exercise", "An error occurred while adding daily exercise.", "OK");
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

            if(string.IsNullOrWhiteSpace(MiddleWare.UserName) == true)
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


        private async Task SnoozeTrainingSessionFeedItem(FeedItem feedItem, int waitTimeInMinutes)
        {
            try
            {
                await ExerciseApi.Net7.ExerciseApi.RescheduleTrainingSession(feedItem.TrainingSessionFeedItem.TraningSession.Id, waitTimeInMinutes);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Reschedule Training Session Feed Item", "An error occurred while rescheduling training session feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Reschedule Nutrient Feed Item", "An error occurred while rescheduling supplement feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Reschedule Supplement Feed Item", "An error occurred while rescheduling supplement feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Reschedule Medical Feed Item", "An error occurred while rescheduling medical feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Reschedule Medical Feed Item", "An error occurred while rescheduling medical feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Undo Snooze Training Session Feed Item", "An error occurred while trying to undo the snooze of training session feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Undo Snooze Nutrient Feed Item", "An error occurred while trying to undo the nutrient feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Undo Skip Supplement Feed Item", "An error occurred while trying to undo the skip of supplement feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Undo Snooze Habit Feed Item", "An error occurred while trying to undo the habit feed items.", "OK");
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
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Undo Snooze Habit Feed Item", "An error occurred while trying to undo the habit feed items.", "OK");
            }
            finally
            {

            }
        }

        private async Task ShowLoadingActivityIndicator()
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

        #region USERS
        public static async Task<bool> SetupUser()
        {
            //var config = await FeedApi.Net7.FeedApi.GetConfig();
            //MiddleWare.NowLaterTime = config[0];
            //MiddleWare.AutoSkippedTimeout = config[1];
            //MiddleWare.OverDueTime = config[2];

            MiddleWare.UserID = -1000;
            var user = await GetLoggedInUser();
            if (user != null && user.isSuccess)
            {
                
                bool IsSuccessful = await LoginUser(user);
                if (IsSuccessful)
                {
                    EnableFooter();
                    NavMenu.isLoggedIn = true;
                }
            }

            return true;
        }

        public static async Task<UserOpResult> LoginUserManually(string UserName, string Password)
        {
            return await UserApi.Net7.UserMiddleware.LoginUser(UserName, Password);
        }

        // call this immedeatly after login
        public static async Task<bool> SaveUser(UserOpResult user)
        {
            try
            {
                //  await SecureStorage.Default.SetAsync("user_token", JsonConvert.SerializeObject(user));
                Preferences.Default.Set("user_token", EncryptDecrypt(JsonConvert.SerializeObject(user), 200));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // call this when the app starts -- if it returnsother than null, then call "LoginUser" function
        public static async Task<UserOpResult> GetLoggedInUser()
        {
            try
            {

                //  var Token = await SecureStorage.Default.GetAsync("user_token");
                var Token = EncryptDecrypt(Preferences.Default.Get("user_token", "Unknown"), 200);


                if (Token == null)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<UserOpResult>(Token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Set the icon on top right from "login" to "Logoff"
        // Navigate to mainpage after login
        // Set the name on the ui to the " user.UserName"
        public static async Task<bool> LoginUser(UserOpResult user)
        {
            MiddleWare.UserID = user.UserId;
            MiddleWare.UserName = user.UserName;

            await FeedApi.Net7.FeedApi.GetDailyPlanId(MiddleWare.UserID, DateTime.Now);

            DateTime date = DateTime.Now;
            string dString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);

            long PlanId = -1;
            if (MiddleWare.DailyPlanId.Keys.Contains(DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)))
            {
                PlanId = MiddleWare.DailyPlanId[DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)];
            }

            MiddleWare.UserName = user.UserName + " U: " + user.UserId + " P: " + PlanId;



            EnableFooter();
            NavMenu.isLoggedIn = true;
            return true;
        }

        // call this when the user clicks log off (after model are you sure..)
        public static bool LogOffuser()
        {
          
            // set no user in the api
            MiddleWare.UserID = -100;
            MiddleWare.DailyPlanId = new Dictionary<DateTime, long>();
            MiddleWare.UserName = "Guest";
            DisableFooter();
            NavMenu.isLoggedIn = false;


            Preferences.Default.Clear();
            return true;
            //return SecureStorage.Default.Remove("user_token");

            // TODO: RELOAD FEEDPAGE
        }


        public static string EncryptDecrypt(string szPlainText, int szEncryptionKey)
        {
            StringBuilder szInputStringBuild = new StringBuilder(szPlainText);
            StringBuilder szOutStringBuild = new StringBuilder(szPlainText.Length);
            char Textch;
            for (int iCount = 0; iCount < szPlainText.Length; iCount++)
            {
                Textch = szInputStringBuild[iCount];
                Textch = (char)(Textch ^ szEncryptionKey);
                szOutStringBuild.Append(Textch);
            }
            return szOutStringBuild.ToString();
        }

        #endregion


        #region Photos

        public async Task TakePhoto()
        {
            try
            {
                string temp = await HandleImageAndSetNutrientImage(true);
                if (temp != null  && temp != "")
                {
                    await OpenAddDishPopup(temp);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task UploadPhoto()
        {
            try
            {
                string temp = await Index.HandleImageAndSetNutrientImage(false);
                if (temp != null && temp != "")
                {
                    await OpenAddDishPopup(temp);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task<string> HandleImageAndSetNutrientImage(bool isCamera)
        {
            FileResult photo = null;
            if (isCamera)
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    photo = await MediaPicker.Default.CapturePhotoAsync();

                }
                else
                {
                    // Show messagebox that caputure is not supported
                }
            }
            else
            {
                photo = await MediaPicker.Default.PickPhotoAsync();
            }


            if (photo != null)
            {
                using (Stream file = await photo.OpenReadAsync()) //.FullPath, FileMode.Open);
                {
                    //  System.Drawing.Image img = System.Drawing.Image.FromStream(file);
                    int maxImageSize = 1600;
                    float precision = 0.8f;
                    byte[] bytes;
#if ANDROID
                    var  image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(file);
                    if (image.Width > maxImageSize || image.Height > maxImageSize)
                    {
                        image = image.Downsize(maxImageSize, true);
                    }
                    bytes = await image.AsBytesAsync(ImageFormat.Jpeg, precision);
                    Index.NutrientImageType = ".jpeg";
#elif IOS
                    var image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(file);
                    if (image.Width > maxImageSize || image.Height > maxImageSize)
                    {
                        image = image.Downsize(maxImageSize, true);
                    }
                    bytes = await image.AsBytesAsync(ImageFormat.Jpeg, precision);
                    Index.NutrientImageType = ".jpeg";

#else
                    System.Drawing.Image img = System.Drawing.Image.FromStream(file);
                    if (img.Size.Height > 1920)
                    {
                        img = resizeImage(img, new System.Drawing.Size(800, 600));
                        
                    }
                    bytes = ImageToByteArray(img);
                    Index.NutrientImageType = Path.GetExtension(photo.FileName);
#endif

                   Index.NutrientImageData = Convert.ToBase64String(bytes);
                }

                return "isphoto";
            }
            else
            {
                return null;
            }
        }


        public byte[] ConvertToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }



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
            Bitmap b = new Bitmap(destWidth, destHeight);
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
                ImageConverter imgCon = new ImageConverter();
                return (byte[])imgCon.ConvertTo(imageIn, typeof(byte[]));

                //  System.Drawing.Imaging.EncoderParameters param = new System.Drawing.Imaging.EncoderParameters() { Param}
                // imageIn.Save(ms, imageIn.RawFormat, System.Drawing.Imaging.EncoderParameters.);
                //  return ms.ToArray();
            }
        }
    }
    #endregion
}