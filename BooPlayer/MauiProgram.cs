using BooPlayer.Services.Impl;
using BooPlayer.Services;
using BooPlayer.Utils;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using BooPlayer.ViewModel;
using Newtonsoft.Json;
using BooPlayer.Models;
using System.Reactive.Subjects;

namespace BooPlayer;

public static class MauiProgram
{
	public static MauiApp App = null!;
	public static ILogger Logger = null!;

	private static BehaviorSubject<bool> _isReady = new(false);
	public static IObservable<bool> IsReady => _isReady;

	public static void MainViewLoaded() {
		_isReady.OnNext(true);
	}


	public static T GetService<T>() where T : class {
        if(App.Services.GetService(typeof(T)) is not T service) {
			throw new ArgumentException($"{typeof(T)} needs to be registered in CreateMauiApp() within MauiProgram.cs.");
		}
		return service;
	}
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			;
        
		// Configurations
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BooPlayer.appsettings.json");
        var config = new ConfigurationBuilder().AddJsonStream(stream!).Build();
		builder.Configuration.AddConfiguration(config);

        // Services
        builder.Services
			// System Services
			.AddHttpClient()

            // My Services
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IUserSettingsService, UserSettingsService>()
            .AddSingleton<IMainThreadService, MainThradService>()
			.AddSingleton<IItemListService, ItemListService>()
            .AddSingleton<IPageService, PageService>()

			// View Models
			.AddSingleton<ItemListViewModel>()
            .AddTransient<HostListViewModel>()
            ;

#if DEBUG
        builder.Logging.AddDebug();
#endif
        App = builder.Build();

        Logger = GetService<ILoggerFactory>().CreateLogger("Boo");
        UtLog.SetGlobalLogger(Logger);


        return App;
	}
}
