using BooPlayer.Services;
using BooPlayer.Utils;
using BooPlayer.View;
using BooPlayer.ViewModel;
using Microsoft.Extensions.Logging;

namespace BooPlayer;

public partial class MainPage : FlyoutPage
{
	private ItemListViewModel ViewModel { 
		set => BindingContext = value;
		get => (BindingContext as ItemListViewModel)!;
	}

	public MainPage()
	{
		ViewModel = MauiProgram.App.Services.GetRequiredService<ItemListViewModel>();
		InitializeComponent();

        //ViewModel.CurrentHostEntry.Value = new Models.HostEntry() {
        //    Address = "192.168.0.151:6001",
        //    Name = "MakibaO",
        //};

        //Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), () => {
        //	ViewModel.CurrentHostEntry.Value = new Models.HostEntry() {
        //		Id = 0,
        //		Address = "192.168.0.151:6001",
        //		Name = "MakibaO",
        //	};
        //});

        //Task.Run(async () => {
        //	await Task.Delay(3000);
        //	await Dispatcher.DispatchAsync(async () => {
        //              await DisplayAlert("HOGE", "FUGA", "OK");
        //          });
        //});
        Loaded += OnLoaded;

    }

    private void OnLoaded(object? sender, EventArgs e) {
        MauiProgram.MainViewLoaded();
        MauiProgram.App.Services.GetRequiredService<IPageService>().ShowModalDialog(new HostListPage());
        Loaded -= OnLoaded;
    }
}

