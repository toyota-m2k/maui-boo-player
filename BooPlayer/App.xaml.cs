using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BooPlayer;

public partial class App : Application
{
    //public IHost Host {
    //    get;
    //}

    //public static T GetService<T>()
    //    where T : class {
    //    if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service) {
    //        throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
    //    }

    //    return service;
    //}
    
    public App()
	{
		InitializeComponent();

        //Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
        //        .UseContentRoot(AppContext.BaseDirectory)
        //        .ConfigureServices((context, service) => {
        //            service
        //            .AddHttpClient()
        //            .AddLogging(builder => {
        //                builder.AddFilter(level => true);
        //                builder.AddConsole();
        //            });

        //        })
        //        .Build();

        //MainPage = new AppShell();
        MainPage = new MainPage();
        
	}
}
