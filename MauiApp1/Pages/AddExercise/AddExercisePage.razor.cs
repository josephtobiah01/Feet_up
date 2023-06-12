using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MauiApp1.Areas.Exercise.Views;
using ExerciseApi.Net7;
using ParentMiddleWare.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MauiApp1.Pages.AddExercise
{

    public partial class AddExercisePage : IDisposable
    {

        [Parameter]
        public EmTrainingSession TrainingSession { get; set; }

        

        #region[Variables and fields]
        private List<EmExerciseType> FullExerciseList;
        //static unchanging copy of the entire list of exercises to refresh the exercise list every time we filter
        //and avoid having to call GetExerciseTypes each time we need to refresh FullExerciseList
        private List<EmExerciseType> FullExerciseListStaticCopy;
        private List<EmExerciseType> SuggestedExerciseList;
        private List<EmEquipment> FullEquipmentList;
        private List<EmMainMuscleWorked> FullMainMuscleWorkedList;
        private EmEquipment EquipmentFilterTerm = null;
        private EmMainMuscleWorked MainMuscleFilterTerm = null;
        /*
         * filterdisplaycategory==1 if equipment
         * filterdisplaycategory==2 if mainmuscle
         * */
        public int filterdisplaycategory=0;
        //displaypopup = "none" to make popup disappear
        //displaypopup = "inline" to make popup appear
        //we can probably change it from display to position and move it up/down and animate it if we want it to slide up/down instead of just appearing in the future.
        //though this will take some time
        public string DisplayPopup = "none";
        private string _searchTerm="";
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
        #endregion
        #region[Initialization]
        protected override async Task OnInitializedAsync()
        {
            await InitializeData();
            selfReferenceAsDotNetObject = DotNetObjectReference.Create(this);
        }
        private async Task InitializeData()
        {
            SuggestedExerciseList = new List<EmExerciseType>();
            FullExerciseList = new List<EmExerciseType>();
            FullEquipmentList = new List<EmEquipment>();
            FullMainMuscleWorkedList = new List<EmMainMuscleWorked>();
            FullExerciseList = await ExerciseApi.Net7.ExerciseApi.GetExerciseTypes();
            FullExerciseListStaticCopy = new List<EmExerciseType>();
           /* for(int i = 0; i < FullExerciseList.Count; i++)
            {
                if (!FullExerciseList[i].IsDeleted)
                {
                    FullExerciseListStaticCopy.Add(FullExerciseList[i]);
                }
            }
            FullExerciseList = new List<EmExerciseType>(FullExerciseListStaticCopy);*/
            FullEquipmentList = await ExerciseApi.Net7.ExerciseApi.GetEquipments();
            FullMainMuscleWorkedList = await ExerciseApi.Net7.ExerciseApi.GetMainMuscleWorked();
            //should probably make a static copy of the suggested list as well.

            #region[TEMPORARY]
            for (int i=0;i< FullExerciseList.Count&&i<3;i++)
            {
                SuggestedExerciseList.Add(FullExerciseList[i]);
            }
            #endregion
        }
        #endregion
        #region[Methods]
        public void ShowAddExerciseEquipmentFilterPopup()
        {
            /*
               * filterdisplaycategory==1 if equipment
               * filterdisplaycategory==2 if mainmuscle
               * */
            filterdisplaycategory = 1;
            OpenAddExerciseFilterPopup();
        }
        public void ShowAddExerciseMuscleFilterPopup()
        {
            /*
               * filterdisplaycategory==1 if equipment
               * filterdisplaycategory==2 if mainmuscle
               * */
            filterdisplaycategory = 2;
            OpenAddExerciseFilterPopup();
        }
        //when a equipment button is pressed, filter list by chosen equipment and close popup
        public void FilterByEquipment(EmEquipment EquipmentToFilterTheListBy)
        {
            EquipmentFilterTerm = EquipmentToFilterTheListBy;
            FilterSearch();
            CloseAddExerciseFilterPopup();
        }
        //when a muscle button is pressed, filter list by chosen muscle and close popup
        public void FilterByMainMuscleWorked(EmMainMuscleWorked MuscleToFilterTheListBy)
        {
            MainMuscleFilterTerm = MuscleToFilterTheListBy;
            FilterSearch();
            CloseAddExerciseFilterPopup();
        }
        private void OpenAddExerciseFilterPopup()
        {
            DisplayPopup = "inline";
        }
        private void CloseAddExerciseFilterPopup()
        {
            DisplayPopup = "none";
        }
        /* filter exercises according to search term*/
        public void FilterSearch()
        {
            ResetLists();

            var TempFullExerciseList = new List<EmExerciseType>();
            var TempSuggestedExerciseList = new List<EmExerciseType>();
            for (int i = 0; i < FullExerciseList.Count(); i++)
            {
                if (EquipmentFilterTerm != null)
                {
                    //if it does not fit the equipment/muscletype conditions, skip to the next exercise item in the list
                    if (FullExerciseList[i].EmEquipment.Id != EquipmentFilterTerm.Id)
                    {
                        continue;
                    }
                }
                if (MainMuscleFilterTerm != null)
                {
                    //if it does not fit the equipment/muscletype conditions, skip to the next exercise item in the list
                    if (FullExerciseList[i].EmMainMuscleWorked.Id != MainMuscleFilterTerm.Id)
                    {
                        continue;
                    }
                }
                if (FullExerciseList[i].Name.ToLower().Contains(SearchTerm.ToLower()))
                {
                    TempFullExerciseList.Add(FullExerciseList[i]);
                }
            }
            for (int i = 0; i < SuggestedExerciseList.Count(); i++)
            {
                if (EquipmentFilterTerm != null)
                {
                    //if it does not fit the equipment/muscletype conditions, skip to the next exercise item in the list
                    if (SuggestedExerciseList[i].EmEquipment.Id != EquipmentFilterTerm.Id)
                    {
                        continue;
                    }
                }
                if (MainMuscleFilterTerm != null)
                {
                    //if it does not fit the equipment/muscletype conditions, skip to the next exercise item in the list
                    if (SuggestedExerciseList[i].EmMainMuscleWorked.Id != MainMuscleFilterTerm.Id)
                    {
                        continue;
                    }
                }
                if (SuggestedExerciseList[i].Name.ToLower().Contains(SearchTerm.ToLower()))
                {
                    TempSuggestedExerciseList.Add(SuggestedExerciseList[i]);
                }
            }
            //set to null so that we dont filter again next time
            EquipmentFilterTerm = null;
            MainMuscleFilterTerm = null;

            //set list contents to the filtered templist contents
            FullExerciseList = TempFullExerciseList;
            SuggestedExerciseList = TempSuggestedExerciseList;
        }
        //resets list to initial state
        public void ResetLists()
        {
            FullExerciseList = new List<EmExerciseType>(FullExerciseListStaticCopy);
            #region[TEMPORARY]
            for (int i = 0; i < FullExerciseList.Count && i < 3; i++)
            {
                SuggestedExerciseList.Add(FullExerciseList[i]);
            }
            #endregion
        }
        public void CloseAddExercisePage()
        {
            App.Current.MainPage.Navigation.PopModalAsync();
        }

        #endregion
        #region[Disposal]
        private DotNetObjectReference<AddExercisePage>? selfReferenceAsDotNetObject;
        public void Dispose()
        {
            selfReferenceAsDotNetObject?.Dispose();
        }
        #endregion
        public async void AddExercisetoTrainingSession(long ExerciseId)
        {
            DoCallBack(ExerciseId);
            await App.Current.MainPage.Navigation.PopModalAsync();
        }
        private async void DoCallBack(long ExerciseId)
        {
            await Callback.InvokeAsync(ExerciseId);
        }

        [Parameter]
        public EventCallback<long> Callback { get; set; }



    }
}