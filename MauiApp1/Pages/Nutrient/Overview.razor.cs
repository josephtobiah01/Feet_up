using MauiApp1.Areas.Nutrient.Views;
using ParentMiddleWare.Models;
using Microsoft.AspNetCore.Components;
using ImageApi.Net7;
using FeedApi.Net7.Models;
using Microsoft.JSInterop;
using CommunityToolkit.Maui.Alerts;

namespace MauiApp1.Pages.Nutrient
{

    public partial class Overview 
    {
        [Parameter]
        public FeedItem feedItem { get; set; }
        [Parameter]
        public bool IsSubmitted { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        public string DisplayNutrientPopup = "none";
        public string DisplayAddDishPopup = "none";
        public string DisplayLoaderPopup = "none";
        public string DisplayFavoritePopup = "none";

        //nutrient fields
        public NutrientrecipesForMeal NutrientPopupRecipesDisplayed;
        public List<NutrientDish> DishesDisplayed = new List<NutrientDish>();
        public List<int> DishesDisplayedIndices = new List<int>();
        public NutrientDish TotalNutrients;
        public bool NutrientIsCustomAddedDish = false;
        public int NutrientServings = 1;
        public double NutrientPortion = 1;
        public string NutrientNotes = "";
        public string NutrientDishName = "";
     //   public string NutrientImageData = "";
        public string NutrientCustomDishImageUrl = "";
        public bool NutrientIsFavorite = false;
        public bool HideOverviewContents = false;

        public bool ShowAgainCheckedValue = true;

        public NutrientRecipeModel NutrientRecipe = null;
        private bool LockSubmission = false;

        protected override async Task OnInitializedAsync()
        {
            if (!IsSubmitted)
            {
                if(feedItem.Status == FeedItemStatus.Ongoing || feedItem.Status == FeedItemStatus.Completed)
                {
                    HideOverviewContents = true;
                    await BuildDishesDisplayedList();
                }
                else
                {
                    HideOverviewContents = false;
                }
            }
            else
            {
                HideOverviewContents = true;
                await BuildDishesDisplayedList();
            }


            if (HideOverviewContents)
            {
                GetTotals();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
            }
        }

        public async void GoToSearchRecipesPage(long status)
        {
            //if status==1, display search;
            //if status ==2, display favorites;
            //if status ==2, display history;
            await App.Current.MainPage.Navigation.PushAsync(new SearchRecipesPage(feedItem, status));
        }
        public void ClosePage()
        {
            App.Current.MainPage.Navigation.PopAsync();
        }

        public async Task GoToHome()
        {
            await App.Current.MainPage.Navigation.PopToRootAsync();
        }
        #region popups
        public async Task OpenNutrientPopup()
        {
            try
            {
                NutrientPopupRecipesDisplayed = await ImageApi.Net7.NutritionApi.GetFavoritesAndHistory();
            }
            catch
            {
                App.alertBottomSheetManager.ShowAlertMessage("Error", "Error getting favorites and history.", "OK");
            }
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
                NutrientCustomDishImageUrl = String.Format("data:image/png;base64,{0}", Index.NutrientImageData);
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
                        App.alertBottomSheetManager.ShowAlertMessage("Error", "Error getting favorites and history.", "OK");
                    }
                }
            }
            catch
            {

            }
            DisplayAddDishPopup = "none";
        }
        public void AddServing()
        {
            NutrientServings += 1;
        }
        public async void OpenLoaderPopup()
        {
            DisplayLoaderPopup = "inline";
        }
        public void CloseLoaderPopup()
        {
            DisplayLoaderPopup = "none";
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
                    uploadModel.MealId = Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId;
                    uploadModel.RecipeId = recipe.RecipeID;
                    uploadModel.NumberOfServings = NutrientServings;
                    uploadModel.NutrientPortion = NutrientPortion;
                    uploadModel.UploadType = NutritionUploadModel_Type.ByRecipe;
                    Index.NutritionUploadModel.Add(uploadModel);
                }
                else
                {
                    NutrientRecipeModel newrecipe = new NutrientRecipeModel();
                    uploadModel.UploadType = NutritionUploadModel_Type.PhotoUpload;
                    uploadModel.NumberOfServings = NutrientServings;
                    uploadModel.NutrientPortion = NutrientPortion;
                    uploadModel.FoodImage64String = Index.NutrientImageData;
                    uploadModel.FoodImageType = Index.NutrientImageType;
                    newrecipe.DisplayImageUrl = NutrientCustomDishImageUrl;
                    uploadModel.NutrientNotes = NutrientNotes;
                    newrecipe.IsFavorite = NutrientIsFavorite;
                    uploadModel.IsFavorite = NutrientIsFavorite;
                    if (NutrientDishName == null || NutrientDishName == "")
                    {
                        ShowAlertBottomSheet("Error", "Please enter a name for the dish.", "OK");
                        return;
                    }
                    newrecipe.RecipeName = NutrientDishName;
                    uploadModel.NutrientDishName = NutrientDishName;
                    uploadModel.MealId = Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId;
                    dishAdded.Recipe = newrecipe;
                    Index.NutritionUploadModel.Add(uploadModel);

                }

                if (feedItem.NutrientsFeedItem.Meal.DishesEaten == null)
                {
                    feedItem.NutrientsFeedItem.Meal.DishesEaten = new List<NutrientDish>();
                }
                feedItem.NutrientsFeedItem.Meal.DishesEaten.Add(dishAdded);
                await CloseAddDishPopup();
                CloseNutrientPopup();
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
                    bool IsSuccessful = await ImageApi.Net7.NutritionApi.FavoriteDish(NutrientRecipe.RecipeID);
                }
                catch
                {
                    App.alertBottomSheetManager.ShowAlertMessage("Error", "Error favoriting dish.", "OK");
                }
                StateHasChanged();
            }
            else if (NutrientRecipe != null)
            {
                try
                {
                    await ImageApi.Net7.NutritionApi.UnFavoriteDish(NutrientRecipe.RecipeID);
                }
                catch
                {
                    App.alertBottomSheetManager.ShowAlertMessage("Error", "Error unfavoriting dish.", "OK");
                }
                StateHasChanged();
            }
            else
            {
                if (NutrientIsFavorite && ParentMiddleWare.MiddleWare.ShowFavoriteMsg)
                {
                    OpenFavoritePopup();
                }
                StateHasChanged();
            }
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
        #endregion
        public async Task SubmitMeal()
        {
            await Task.Run(() => OpenLoaderPopup());
            bool IsSuccessful = false;
            if (LockSubmission != true)
            {
                LockSubmission = true;
                try
                {
                    foreach (var x in Index.NutritionUploadModel)
                    {
                        if (x.UploadType == NutritionUploadModel_Type.PhotoUpload)
                        {
                            if (feedItem.NutrientsFeedItem.Meal.IsCustom)
                            {
                                long temp = await NutritionApi.AddDishByPhotoCustomMeal(x.NutrientDishName, x.NutrientNotes, x.NutrientPortion, x.NumberOfServings, x.FoodImageType, x.FoodImage64String, x.IsFavorite);
                                if (temp > 0)
                                {
                                    IsSuccessful = true;
                                    Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId = temp;
                                    feedItem.NutrientsFeedItem.Meal.MealId = temp;
                                    Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.IsCustom = false;
                                    feedItem.NutrientsFeedItem.Meal.IsCustom = false;
                                }
                            }
                            else
                            {
                                IsSuccessful = await NutritionApi.AddDishByPhoto(x.NutrientDishName, x.NutrientNotes, feedItem.NutrientsFeedItem.Meal.MealId, x.NutrientPortion, x.NumberOfServings, x.FoodImageType, x.FoodImage64String, x.IsFavorite);
                            }
                            if (!IsSuccessful)
                            {
                                break;
                            }
                        }
                        else if (x.UploadType == NutritionUploadModel_Type.ByRecipe)
                        {
                            if (x.RecipeId == 0)
                            {
                                IsSuccessful = false;
                                break;
                            }
                            if (x.IsFavorite)
                            {
                                if (feedItem.NutrientsFeedItem.Meal.IsCustom)
                                {
                                    long temp = await NutritionApi.AddDishByReferenceCustomMeal(x.RecipeId, x.NutrientNotes, x.NutrientPortion, x.NumberOfServings);
                                    if (temp > 0)
                                    {
                                        IsSuccessful = true;
                                        Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId = temp;
                                        feedItem.NutrientsFeedItem.Meal.MealId = temp;
                                        Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.IsCustom = false;
                                        feedItem.NutrientsFeedItem.Meal.IsCustom = false;
                                    }
                                }
                                else
                                {
                                    IsSuccessful = await NutritionApi.AddDishByReference(x.RecipeId, x.NutrientNotes, feedItem.NutrientsFeedItem.Meal.MealId, x.NutrientPortion, x.NumberOfServings);
                                }
                            }
                            else
                            {
                                if (feedItem.NutrientsFeedItem.Meal.IsCustom)
                                {
                                    long temp = await NutritionApi.AddDishByReferenceCustomMeal(x.RecipeId, x.NutrientNotes, x.NutrientPortion, x.NumberOfServings, x.IsFavorite);
                                    if (temp > 0)
                                    {
                                        IsSuccessful = true;
                                        Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId = temp;
                                        feedItem.NutrientsFeedItem.Meal.MealId = temp;
                                        Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.IsCustom = false;
                                        feedItem.NutrientsFeedItem.Meal.IsCustom = false;
                                    }
                                }
                                else
                                {
                                    IsSuccessful = await NutritionApi.AddDishByReference(x.RecipeId, x.NutrientNotes, feedItem.NutrientsFeedItem.Meal.MealId, x.NutrientPortion, x.NumberOfServings, x.IsFavorite);
                                }
                            }
                            if (!IsSuccessful)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    App.alertBottomSheetManager.ShowAlertMessage("Error", "Error submitting dishes.", "OK");
                }
                LockSubmission = false;
            }

            await Task.Run(() => CloseLoaderPopup());
            if (IsSuccessful)
            {
                Index.NutritionUploadModel = new List<NutritionUploadModel>();
                await BuildDishesDisplayedList();
                HideOverviewContents = true;
                GetTotals();
                StateHasChanged();
                //await App.Current.MainPage.DisplayAlert("Success!", "Your images were uploaded.", "OK");
                var toast = Toast.Make("Success! Your images were uploaded!", CommunityToolkit.Maui.Core.ToastDuration.Long);
                await toast.Show();
                StateHasChanged();
                //todo
                //GoToHome();
            }
            else
            {
                StateHasChanged();
                ShowAlertBottomSheet("Error", "There was an error uploading images. Please try again later.", "OK");
            }
            //todo
        }
        public void GetTotals()
        {
            TotalNutrients = new NutrientDish();
            TotalNutrients.NumberOfServings = 0;
            TotalNutrients.Recipe = new NutrientRecipeModel();
            TotalNutrients.Recipe.NutrientInformation = new NutrientRecipeModel.RecipeNutrientInformation();
            TotalNutrients.Recipe.NutrientInformation.Carbohydrates = 0;
            TotalNutrients.Recipe.NutrientInformation.Fiber = 0;
            TotalNutrients.Recipe.NutrientInformation.Protein = 0;
            TotalNutrients.Recipe.NutrientInformation.Fat = 0;
            TotalNutrients.Recipe.NutrientInformation.Calories = 0;
            for (int i = 0; i < DishesDisplayed.Count; i++)
            {
                TotalNutrients.NumberOfServings += DishesDisplayed[i].NumberOfServings;
                if (DishesDisplayed[i].Recipe != null && DishesDisplayed[i].Recipe.NutrientInformation != null && DishesDisplayed[i].Recipe.NutrientInformation.Calories != null)
                {
                    TotalNutrients.Recipe.NutrientInformation.Carbohydrates += DishesDisplayed[i].Recipe.NutrientInformation.Carbohydrates;
                    TotalNutrients.Recipe.NutrientInformation.Fiber += DishesDisplayed[i].Recipe.NutrientInformation.Fiber;
                    TotalNutrients.Recipe.NutrientInformation.Protein += DishesDisplayed[i].Recipe.NutrientInformation.Protein;
                    TotalNutrients.Recipe.NutrientInformation.Fat += DishesDisplayed[i].Recipe.NutrientInformation.Fat;
                    TotalNutrients.Recipe.NutrientInformation.Calories += DishesDisplayed[i].Recipe.NutrientInformation.Calories;
                }
            }
        }
        public async Task BuildDishesDisplayedList()
        {
            try
            {
                DishesDisplayed = await ImageApi.Net7.NutritionApi.GetMealDishes(feedItem.NutrientsFeedItem.Meal.MealId);
                for (int i = 0; i < DishesDisplayed.Count(); i++)
                {
                    DishesDisplayedIndices.Add(i);
                }
            }
            catch
            {
                App.alertBottomSheetManager.ShowAlertMessage("Error", "Error getting dishes.", "OK");
            }
        }
        public async Task ToggleCollapsibleByIndex(int index)
        {
            await JSRuntime.InvokeAsync<string>("ToggleCollapsible", index.ToString());
        }

        #region photos
        public async Task TakePhoto()
        {
            try
            {
                string temp = await Index.HandleImageAndSetNutrientImage(true);
                if (temp != null && temp != "")
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

        public byte[] ConvertToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        #endregion

        #region [Methods :: Tasks]

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