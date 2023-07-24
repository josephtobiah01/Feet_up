using MauiApp1.Interfaces;
using System.ComponentModel;
using System.Drawing.Text;
using System.Windows.Input;

namespace MauiApp1.Areas.Dashboard.Views;

public class TestCSharpOnly : ContentView, INotifyPropertyChanged
{

    #region Fields

    private DateTime _selectedDate = DateTime.Today;
    private int _targetCalories;
    private double _currentCalories;
    private INutrientsIntakeService _nutrientsIntakeService;
    private ICommand _navigateTo2ndPage;

    #endregion Fields



    #region Properties


    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand NavigateTo2ndPage
    {
        get { return _navigateTo2ndPage; }
        set
        {
            if (_navigateTo2ndPage != value)
            {
                _navigateTo2ndPage = value;
                OnPropertyChanged(nameof(NavigateTo2ndPage));
            }
        }
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }
    }

    public int TargetCalories
    {
        get => _targetCalories;
        set
        {
            if (_targetCalories != value)
            {
                _targetCalories = value;
                OnPropertyChanged(nameof(TargetCalories));
            }
        }
    }

    public double CurrentCalories
    {
        get => _currentCalories;
        set
        {
            if (_currentCalories != value)
            {
                _currentCalories = value;
                OnPropertyChanged(nameof(CurrentCalories));

            }
        }
    }


    #endregion Properties

    #region Methods
    #region Constructors

    public TestCSharpOnly(INutrientsIntakeService nutrientsIntakeService)
    {
        nutrientsIntakeService = _nutrientsIntakeService;
    }


    #endregion Constructors

    protected override void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void InitializeComponent()
    {
        var VerticalStackLayoutParent = new VerticalStackLayout()
        {
            Margin = new Thickness(0),
            Padding = new Thickness(0)
        };

        var labelOfVerticalStackLayoutParent = new Label()
        {

        };
    }
    #endregion Methods
    
}