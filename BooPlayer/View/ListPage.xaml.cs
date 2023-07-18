using BooPlayer.ViewModel;

namespace BooPlayer.View;

public partial class ListPage : ContentPage
{
    private ItemListViewModel ViewModel {
        set => BindingContext = value;
        get => (BindingContext as ItemListViewModel)!;
    }

    public ListPage()
	{
        ViewModel = MauiProgram.App.Services.GetRequiredService<ItemListViewModel>();
        InitializeComponent();
	}
}