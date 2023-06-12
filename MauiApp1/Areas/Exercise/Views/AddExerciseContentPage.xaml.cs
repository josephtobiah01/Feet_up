
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView.Maui;
using ParentMiddleWare.Models;


namespace MauiApp1.Areas.Exercise.Views;


public class EmExerciseSelectedHandler : EventArgs
{
    public long ExerciseId { get; set; }
}



public partial class AddExerciseContentPage : ContentPage
{

    #region[Fields || Variables]
    public EmTrainingSession _emTrainingSession;
    #endregion
    #region [Methods :: EventHandlers :: Class]

    public AddExerciseContentPage()
    {
        _emTrainingSession = new EmTrainingSession();
        InitializeComponent();
        rootComponent.Parameters =
          new Dictionary<string, object>
          {
                { "TrainingSession", _emTrainingSession}
          };
    }

  
    
    protected virtual void OnExerciseSelected(EmExerciseSelectedHandler exercise)
    {
        EventHandler<EmExerciseSelectedHandler> handler = ExerciseAdded;
        if (handler != null)
        {
            handler(this, exercise);
        }
    }

    public event EventHandler<EmExerciseSelectedHandler> ExerciseAdded;




public AddExerciseContentPage(EmTrainingSession emTrainingSession)
    {
        _emTrainingSession = emTrainingSession;
        InitializeComponent();
        rootComponent.Parameters =
          new Dictionary<string, object>
          {
                { "TrainingSession", _emTrainingSession},
                { "Callback", new EventCallback<long>(null, DoAddExercise) }
          };
    }

    public void DoAddExercise(long ExerciseClassID)
    {
        EmExerciseSelectedHandler x = new EmExerciseSelectedHandler();
        x.ExerciseId = ExerciseClassID;
        OnExerciseSelected(x);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        //Initialize Data Here
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BlazorWebView != null)
        {
            BlazorWebView.RootComponents.Clear();
            BlazorWebView.Handler.DisconnectHandler();
            BlazorWebView = null;
        }
    }
    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        GC.Collect();
    }

    private async void CloseModal(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PopModalAsync();
    }
    #endregion

}