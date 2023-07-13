using BooPlayer.ViewModel;

namespace BooPlayer;

public partial class AppShell : Shell
{
	ItemListViewModel ViewModel {
        set => BindingContext = value;
        get => (BindingContext as ItemListViewModel)!;
    }

	public AppShell()
	{
		ViewModel = MauiProgram.App.Services.GetRequiredService<ItemListViewModel>();
		InitializeComponent();
	}
}
