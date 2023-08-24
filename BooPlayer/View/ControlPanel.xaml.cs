using BooPlayer.ViewModel;

namespace BooPlayer.View;

public partial class ControlPanel : ContentView
{
    private ItemListViewModel ViewModel {
        set => BindingContext = value;
        get => (BindingContext as ItemListViewModel)!;
    }
    
    public ControlPanel()
	{
        ViewModel = MauiProgram.App.Services.GetRequiredService<ItemListViewModel>();
        InitializeComponent();
	}
}