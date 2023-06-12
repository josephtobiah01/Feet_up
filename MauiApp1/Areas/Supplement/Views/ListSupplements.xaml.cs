using MauiApp1.Areas.Exercise.Resources;
using MauiApp1.Areas.Supplement.ViewModels;
using ParentMiddleWare.Models;
using System.Collections.ObjectModel;

namespace MauiApp1.Areas.Supplement.Views;

public partial class ListSupplements : ContentPage
{

    #region [Fields]

    private ObservableCollection<SupplementPageViewModel> _supplementPageViewModels;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ListSupplements()
	{
		InitializeComponent();
        _supplementPageViewModels = new ObservableCollection<SupplementPageViewModel>();

    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeData();
    }

    private void InitializeData()
    {
        LoadSupplements();
        //this.SupplementsCollectionView.ItemsSource = _supplementPageViewModels;
        this.DataSourceVerticalLayout.BindingContext = _supplementPageViewModels[0];
        //BindableLayout.SetItemsSource(this.DataSourceVerticalLayout, _supplementPageViewModels[0]);
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void TextPairVerticalStackLayout_Loaded(object sender, EventArgs e)
    {

    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        CloseDialog();
    }

    #endregion

    #region [Methods :: Tasks]

    private void LoadSupplements()
    {

        //SupplementPageViewModel supplementPageViewModel = null;

        //supplementPageViewModel = new SupplementPageViewModel();
        //supplementPageViewModel.SupplementName = "Supplement name 1";
        //supplementPageViewModel.FromDate = DateTime.Now;
        //supplementPageViewModel.ToDate = DateTime.Now.AddMinutes(15);
        //supplementPageViewModel.TextDictionary = new Dictionary<Areas.Supplement.ViewModels.TextCategory, string>();
        //supplementPageViewModel.TextDictionary.Add(Areas.Supplement.ViewModels.TextCategory.Before_Meal, "1 Tablet");

        //_supplementPageViewModels.Add(supplementPageViewModel);


        //supplementPageViewModel = new SupplementPageViewModel();
        //supplementPageViewModel.SupplementName = "Supplement name 2";
        //supplementPageViewModel.FromDate = DateTime.Now.AddMinutes(16);
        //supplementPageViewModel.ToDate = DateTime.Now.AddMinutes(30);
        //supplementPageViewModel.TextDictionary = new Dictionary<Areas.Supplement.ViewModels.TextCategory, string>();
        //supplementPageViewModel.TextDictionary.Add(Areas.Supplement.ViewModels.TextCategory.Before_Meal, "1 Tablet");
        //supplementPageViewModel.TextDictionary.Add(Areas.Supplement.ViewModels.TextCategory.Description, "Avoid eating fat based food with this");


        //_supplementPageViewModels.Add(supplementPageViewModel);

        //supplementPageViewModel = new SupplementPageViewModel();
        //supplementPageViewModel.SupplementName = "Supplement name 3";
        //supplementPageViewModel.FromDate = DateTime.Now.AddMinutes(16);
        //supplementPageViewModel.ToDate = DateTime.Now.AddMinutes(30);
        //supplementPageViewModel.TextDictionary = new Dictionary<Areas.Supplement.ViewModels.TextCategory, string>();
        //supplementPageViewModel.TextDictionary.Add(Areas.Supplement.ViewModels.TextCategory.Before_Meal, "1 Tablet");
        //supplementPageViewModel.TextDictionary.Add(Areas.Supplement.ViewModels.TextCategory.Description, "Avoid eating fat based food with this");


        //_supplementPageViewModels.Add(supplementPageViewModel);
    }

    private async void CloseDialog()
    {
        await Navigation.PopAsync();
    }

    #endregion
 
}