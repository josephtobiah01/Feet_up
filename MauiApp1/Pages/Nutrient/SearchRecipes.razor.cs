using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MauiApp1.Areas.Nutrient.Views;
using ExerciseApi.Net7;
using ParentMiddleWare.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FeedApi.Net7.Models;
using ImageApi.Net7;

namespace MauiApp1.Pages.Nutrient
{

    public partial class SearchRecipes 
    {
        [Parameter]
        public FeedItem feedItem { get; set; }
        [Parameter]
        public long status  { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        //if status==1, display search;
        //if status ==2, display favorites;
        //if status ==2, display history;
        public string DisplayPopup = "none";
        public string DisplaySearchBar = "none";
        public string DisplayFavoritePopup = "none";
        public string HeaderText = "";
        public List<NutrientRecipeModel> NutrientPopupRecipesDisplayed = new List<NutrientRecipeModel>();
        public List<NutrientRecipeModel> NutrientPopupRecipesDisplayedStaticCopy = new List<NutrientRecipeModel>();

        public bool NutrientIsCustomAddedDish = false;
        public int NutrientServings = 1;
        public double NutrientPortion = 1;
        public string NutrientNotes = "";
        public string NutrientDishName = "";
        public string NutrientCustomDishImageUrl = "";
        public bool NutrientIsFavorite = false;
        public NutrientRecipeModel NutrientRecipe = null;
        public bool ShowAgainCheckedValue = true;

        private string _searchTerm = "";
        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                //every time the searchterm is set, call filter search
                _searchTerm = value;
                FilterSearch();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await RefreshList();
        }
        public async Task RefreshList()
        {
            NutrientrecipesForMeal NutrientPopupRecipesObject = await ImageApi.Net7.NutritionApi.GetFavoritesAndHistory();
            if (status == 1)
            {
                HeaderText = "Search Recipes";
                DisplaySearchBar = "inline";
                NutrientPopupRecipesDisplayed = NutrientPopupRecipesObject.History;
                NutrientPopupRecipesDisplayedStaticCopy = NutrientPopupRecipesObject.History;
            }
            else if (status == 2)
            {
                HeaderText = "Favourites";
                DisplaySearchBar = "inline";
                NutrientPopupRecipesDisplayed = NutrientPopupRecipesObject.Favorite;
                NutrientPopupRecipesDisplayedStaticCopy = NutrientPopupRecipesObject.Favorite;
            }
            else if (status == 3)
            {
                HeaderText = "History";
                DisplaySearchBar = "inline";
                NutrientPopupRecipesDisplayed = NutrientPopupRecipesObject.History;
                NutrientPopupRecipesDisplayedStaticCopy = NutrientPopupRecipesObject.History;
            }
        }
        public async void GoToOverviewPage()
        {
           await App.Current.MainPage.Navigation.PushAsync(new OverviewPage(feedItem));
        }
        public void ClosePage()
        {
            App.Current.MainPage.Navigation.PopAsync();
        }

        public void FilterSearch()
        {
            NutrientPopupRecipesDisplayed = new List<NutrientRecipeModel>();
            for (int i=0;i< NutrientPopupRecipesDisplayedStaticCopy.Count(); i++)
            {
                if (NutrientPopupRecipesDisplayedStaticCopy[i].RecipeName.ToLower().Contains(SearchTerm.ToLower()))
                {
                    NutrientPopupRecipesDisplayed.Add(NutrientPopupRecipesDisplayedStaticCopy[i]);
                }
            }
        }

        public async Task OpenAddDishPopup(string imageurl = "", NutrientRecipeModel recipe = null)
        {
            //only one or the other is null, not both.
            DisplayPopup = "inline";
            NutrientServings = 1;
            await OneHundredPercentPortion();
            NutrientDishName = "";
            NutrientIsFavorite = false;
            NutrientNotes = "";

            NutrientIsCustomAddedDish = false;
            NutrientRecipe = recipe;
            NutrientDishName = recipe.RecipeName;
            NutrientCustomDishImageUrl = recipe.DisplayImageUrl;
        }
        public async Task CloseAddDishPopup()
        {
            await RefreshList();
            DisplayPopup = "none";
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
                uploadModel.IsFavorite = NutrientIsFavorite;
                dishAdded.Recipe = recipe;
                uploadModel.MealId = Index.NutrientPopupCurrentFeedItem.NutrientsFeedItem.Meal.MealId;
                uploadModel.RecipeId = recipe.RecipeID;
                uploadModel.NumberOfServings = NutrientServings;
                uploadModel.NutrientPortion = NutrientPortion;
                uploadModel.UploadType = NutritionUploadModel_Type.ByRecipe;
                Index.NutritionUploadModel.Add(uploadModel);
            }
            else
            {
                //imageupload should not happen in this page
            }
            if (feedItem.NutrientsFeedItem.Meal.DishesEaten == null)
            {
                feedItem.NutrientsFeedItem.Meal.DishesEaten = new List<NutrientDish>();
            }
            feedItem.NutrientsFeedItem.Meal.DishesEaten.Add(dishAdded);
            //go to overview
            GoToOverviewPage();
            CloseAddDishPopup();
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
            if (NutrientIsFavorite && NutrientRecipe != null)
            {
                bool IsSuccessful = await ImageApi.Net7.NutritionApi.FavoriteDish(NutrientRecipe.RecipeID);
                StateHasChanged();
            }
            else
            {
                await ImageApi.Net7.NutritionApi.UnFavoriteDish(NutrientRecipe.RecipeID);
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
    }
}