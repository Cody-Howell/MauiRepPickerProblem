using PickerProblem.MAUI.ViewModels;

namespace PickerProblem.MAUI;

public partial class MainPage : ContentPage {
    public MainPage() {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }
}

