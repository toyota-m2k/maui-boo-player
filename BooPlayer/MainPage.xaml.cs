using BooPlayer.ViewModel;

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

		Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), () => {
			ViewModel.CurrentHostEntry.Value = new Models.HostEntry() {
				Id = 0,
				Address = "192.168.0.152:6001",
				Name = "MakibaO",
			};
		});

		//Task.Run(async () => {
		//	await Task.Delay(3000);
		//	await Dispatcher.DispatchAsync(async () => {
  //              await DisplayAlert("HOGE", "FUGA", "OK");
  //          });
		//});

    }
}

