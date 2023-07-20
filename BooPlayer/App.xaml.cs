using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BooPlayer;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();

        //MainPage = new AppShell();
        MainPage = new MainPage();
	}

    

    //protected override Window CreateWindow(IActivationState? activationState) {
    //    var window = base.CreateWindow(activationState);
    //    window.Activated += OnActivated;
    //    //MauiProgram.WindowSubject.OnNext(window);
    //    return window;
    //}

    //private void OnActivated(object? sender, EventArgs e) {
    //    if(sender is Window window) {
    //        window.Activated -= OnActivated;
    //        Task.Run(async () => {
    //            //await Task.Delay(1000);
    //            MauiProgram.WindowSubject.OnNext(window);
    //            //await Dispatcher.DispatchAsync(() => {
    //            //    MauiProgram.WindowSubject.OnNext(window);
    //            //});
    //        });
    //    }
    //}
}
