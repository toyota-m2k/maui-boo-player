using BooPlayer.Services;
using BooPlayer.ViewModel;
using Microsoft.Extensions.Logging.Abstractions;

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
        //IDisposable windowCreated = null!;
        //windowCreated = MauiProgram.Window.Subscribe(window => {
        //    if (window != null) {
        //        window.Destroying += OnWindowDestroying;
        //        windowCreated.Dispose();
        //    }
        //});

        //Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object? sender, EventArgs e) {
        ViewModel.PlayerModel.DetachPlayer();
    }

    private void OnMediaEnd(object sender, EventArgs e) {
        ViewModel.PlayerModel.EndOfMovie.Execute();
    }

    //private void OnLoaded(object? sender, EventArgs e) {
    //    Loaded -= OnLoaded;
    //}

    //private void OnWindowDestroying(object? sender, EventArgs e) {
    //    ViewModel.PlayerModel.DetachPlayer();
    //    if (Window != null) {
    //        Window.Destroying -= OnWindowDestroying;
    //    }
    //}
}