using FeedApi.Net7.Models;
using MauiApp1.Areas.Supplement.ViewModels;
namespace MauiApp1.Areas.Supplement.Views;

public partial class AddSupplementFormContentView : ContentView
{

    #region [Methods :: EventHandlers :: Class]

    public AddSupplementFormContentView()
    {
        InitializeComponent();
        InitializeData();
        InitializeControls();
    }

    private void InitializeData()
    {

    }

    private void InitializeControls()
    {
        this.SupplementTypePicker.SelectedIndex = 0;

        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)this.BindingContext;
        addSupplementPageViewModel.SupplementTimes = new System.Collections.ObjectModel.ObservableCollection<TimeOnly>();

        BindableLayout.SetItemsSource(this.TimeListStackLayout, addSupplementPageViewModel.SupplementTimes);

        //#if WINDOWS || ANDROID || IOS
        ////this.SupplementTypePickerButton.IsVisible = false;
        //this.SupplementQuantityEntry.WidthRequest = 210;
        //this.TimePickerBorder.WidthRequest = -1;
        //#endif
    }


    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void DiscardButton_Clicked(object sender, EventArgs e)
    {
        CloseDialog();
    }

    private void SubmitButton_Clicked(object sender, EventArgs e)
    {
        AddSupplement();
    }

    private void SupplementTypePickerButton_Clicked(object sender, EventArgs e)
    {
        SelectSupplementTypePicker();
    }

    private void BeforeMealButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectBeforeMeal(button);
    }

    private void AfterMealButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectAfterMeal(button);
    }

    private void EmptyStomachButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectEmptyStomach(button);
    }

    private void MondayButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectMonday(button);
    }

    private void TuesdayButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectTuesday(button);
    }

    private void WednesdayButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectWednesday(button);
    }

    private void ThursdayButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectThursday(button);
    }

    private void FridaydayButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectFriday(button);
    }

    private void SaturdayButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectSaturday(button);
    }

    private void SundayButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        SelectSunday(button);
    }

    private void AddSupplementTimeButton_Clicked(object sender, EventArgs e)
    {
        AddSupplementTime();
    }

    private void RemoveSuplplementTimeButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button button = (Button)sender;
            TimeOnly timeOnly = (TimeOnly)button.BindingContext;
            RemoveSupplementTime(timeOnly);
        }
        catch(Exception ex)
        {

        }
        finally
        {

        }
    }

    #endregion

    #region [Methods :: Tasks]

    private void SelectBeforeMeal(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsBeforeMeal == true)
        {
            addSupplementPageViewModel.IsBeforeMeal = false;
            addSupplementPageViewModel.IsAfterMeal = false;
            addSupplementPageViewModel.IsEmptyStomach = false;
        }
        else
        {
            addSupplementPageViewModel.IsBeforeMeal = true;
            addSupplementPageViewModel.IsAfterMeal = false;
            addSupplementPageViewModel.IsEmptyStomach = false;
        }
    }

    private void SelectAfterMeal(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsAfterMeal == true)
        {
            addSupplementPageViewModel.IsBeforeMeal = false;
            addSupplementPageViewModel.IsAfterMeal = false;
            addSupplementPageViewModel.IsEmptyStomach = false;
        }
        else
        {
            addSupplementPageViewModel.IsBeforeMeal = false;
            addSupplementPageViewModel.IsAfterMeal = true;
            addSupplementPageViewModel.IsEmptyStomach = false;
        }
    }

    private void SelectEmptyStomach(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsEmptyStomach == true)
        {
            addSupplementPageViewModel.IsBeforeMeal = false;
            addSupplementPageViewModel.IsAfterMeal = false;
            addSupplementPageViewModel.IsEmptyStomach = false;
        }
        else
        {
            addSupplementPageViewModel.IsBeforeMeal = false;
            addSupplementPageViewModel.IsAfterMeal = false;
            addSupplementPageViewModel.IsEmptyStomach = true;
        }
    }

    private async void AddSupplement()
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)this.BindingContext;

        await App.Current.MainPage.Navigation.PushAsync(new ViewAddSupplementSuccessContentPage(), true);
    }

    private async void CloseDialog()
    {
        await Navigation.PopAsync();
    }

    private void SelectMonday(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsMonday == true)
        {
            addSupplementPageViewModel.IsMonday = false;
        }
        else
        {
            addSupplementPageViewModel.IsMonday = true;
        }
    }

    private void SelectTuesday(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsTuesday == true)
        {
            addSupplementPageViewModel.IsTuesday = false;
        }
        else
        {
            addSupplementPageViewModel.IsTuesday = true;
        }
    }

    private void SelectWednesday(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsWednesday == true)
        {
            addSupplementPageViewModel.IsWednesday = false;
        }
        else
        {
            addSupplementPageViewModel.IsWednesday = true;
        }
    }

    private void SelectThursday(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsThursday == true)
        {
            addSupplementPageViewModel.IsThursday = false;
        }
        else
        {
            addSupplementPageViewModel.IsThursday = true;
        }
    }

    private void SelectFriday(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsFriday == true)
        {
            addSupplementPageViewModel.IsFriday = false;
        }
        else
        {
            addSupplementPageViewModel.IsFriday = true;
        }
    }

    private void SelectSaturday(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsSaturday == true)
        {
            addSupplementPageViewModel.IsSaturday = false;
        }
        else
        {
            addSupplementPageViewModel.IsSaturday = true;
        }
    }

    private void SelectSunday(Button button)
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)button.BindingContext;
        if (addSupplementPageViewModel.IsSunday == true)
        {
            addSupplementPageViewModel.IsSunday = false;
        }
        else
        {
            addSupplementPageViewModel.IsSunday = true;
        }
    }

    private void SelectSupplementTypePicker()
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)this.BindingContext;
        this.SupplementTypePicker.Focus();
    }

    private void AddSupplementTime()
    {
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)this.BindingContext;

        if (addSupplementPageViewModel.SupplementTimes == null)
        {
            addSupplementPageViewModel.SupplementTimes = new System.Collections.ObjectModel.ObservableCollection<TimeOnly>();
        }

        TimeSpan timeSpan = this.SupplementTimePicker.Time;
        TimeOnly timeOnly = TimeOnly.FromTimeSpan(timeSpan);

        addSupplementPageViewModel.SupplementTimes.Add(timeOnly);
    }

    private void RemoveSupplementTime(TimeOnly timeOnly)
    {
     
        AddSupplementPageViewModel addSupplementPageViewModel = (AddSupplementPageViewModel)this.BindingContext;

        addSupplementPageViewModel.SupplementTimes?.Remove(timeOnly); 
    }

    #endregion

  
}