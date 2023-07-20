using BooPlayer.ViewModel;

namespace BooPlayer.View;

public partial class HostListPage : ContentPage
{
    private HostListViewModel ViewModel {
        set => BindingContext = value;
        get => (BindingContext as HostListViewModel)!;
    }
    
    public HostListPage()
	{
        ViewModel = MauiProgram.App.Services.GetRequiredService<HostListViewModel>();
        InitializeComponent();

        ViewModel.CompleteCommand.Subscribe(async (ok) => {
            await Navigation.PopModalAsync();
        });

        var button = new Button();
        button.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 10);
    }
}