using BooPlayer.Utils;
using BooPlayer.View;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace BooPlayer.Services.Impl;
internal class PageService : IPageService {
    private readonly ILogger _logger;
    public PageService(ILoggerFactory loggerFactory) { 
        _logger = loggerFactory.CreateLogger("PageService");
    }

    private async Task WaitForPrepared() {
        await MauiProgram.IsReady.FirstAsync(it => it);
        return;
    }

    public async Task ShowConfirmationMessage(string title, string message) {
        await WaitForPrepared();
        var page = Application.Current?.MainPage;
        if (page == null) return;
        try {
            await page.Dispatcher.DispatchAsync(async () => {
                await page.DisplayAlert(title, message, "OK");
                return;
            });
        } catch(Exception ex) {
            _logger.Error(ex);
        }
    }

    public async Task<bool> ShowOkCancelMessage(string title, string message) {
        await WaitForPrepared();
        var page = Application.Current?.MainPage;
        if (page == null) return false;
        try {
            return await page.Dispatcher.DispatchAsync(async () => {
                return await page.DisplayAlert(title, message, "OK", "Cancel");
            });
        } catch(Exception ex) {
            _logger.Error(ex);
            return false;
        }
    }
    public async Task<bool> ShowYesNoMessage(string title, string message) {
        await WaitForPrepared();
        var page = Application.Current?.MainPage;
        if (page == null) return false;
        try {
            return await page.Dispatcher.DispatchAsync(async () => {
                return await page.DisplayAlert(title, message, "Yes", "No");
            });
        } catch(Exception ex) {
            _logger.Error(ex);
            return false;
        }
    }

    public async Task<string?> ShowPasswordDialog() {
        await WaitForPrepared();
        var page = Application.Current?.MainPage;
        if (page == null) return null;
        return await page.Dispatcher.DispatchAsync(async () => {
            var result = await page.DisplayPromptAsync("Password", "Enter password", "OK", "Cancel");
            return result;
        });

        //return await _mainThreadService.Run(async () => { 
        //    var page = Application.Current?.MainPage;
        //    if (page == null) {
        //        return null;
        //    }
        //    var result = await page.DisplayPromptAsync("Password", "Enter password", "OK", "Cancel", "password", -1, Keyboard.Default, "");
        //    return result;
        //});
    }

    public async Task ShowModalDialog(Page dialogPage) {
        await WaitForPrepared();
        var page = Application.Current?.MainPage;
        if (page == null) return;
        
        await page.Navigation.PushModalAsync(dialogPage);
    }

    public async Task RunOnUIThread(Action fn) {
        await WaitForPrepared();
        var page = Application.Current?.MainPage;
        if (page == null) throw new InvalidOperationException("No MainPage!!");
        await page.Dispatcher.DispatchAsync(fn);
    }
    public async Task<T> RunOnUIThread<T>(Func<T> fn) {
        await WaitForPrepared();
        var page = Application.Current?.MainPage;
        if (page == null) throw new InvalidOperationException("No MainPage!!");
        return await page.Dispatcher.DispatchAsync(fn);
    }
}
