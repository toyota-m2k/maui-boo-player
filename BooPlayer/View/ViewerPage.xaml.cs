using BooPlayer.ViewModel;

namespace BooPlayer.View;

public partial class ViewerPage : ContentPage {
    private ItemListViewModel ViewModel {
        set => BindingContext = value;
        get => (BindingContext as ItemListViewModel)!;
    }

    public ViewerPage() {
        ViewModel = MauiProgram.App.Services.GetRequiredService<ItemListViewModel>();
        InitializeComponent();
        ViewModel.PlayerModel.AttachPlayer(MediaPlayer,PositionSlider);

        Button x = new Button() {
            Text = "HOGE",
        };
    }
}