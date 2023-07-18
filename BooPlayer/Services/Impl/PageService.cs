namespace BooPlayer.Services.Impl;
internal class PageService : IPageService {
    IMainThreadService _mainThreadService;
    public PageService(IMainThreadService mainThreadService) { 
        _mainThreadService = mainThreadService;
    }

    public async Task ShowConfirmationMessage(string title, string message) {
        await _mainThreadService.Run(async () => {
            var page = Application.Current?.MainPage;
            if (page == null) {
                return;
            }
            await page.DisplayAlert(title,message,"OK");
            return;
        });
    }

    public async Task<bool> ShowOkCancelMessage(string title, string message) {
        return await _mainThreadService.Run(async () => {
            var page = Application.Current?.MainPage;
            if (page == null) {
                return false;
            }
            return await page.DisplayAlert(title,message,"OK","Cancel");
        });
    }
    public async Task<bool> ShowYesNoMessage(string title, string message) {
        return await _mainThreadService.Run(async () => {
            var page = Application.Current?.MainPage;
            if (page == null) {
                return false;
            }
            return await page.DisplayAlert(title, message, "Yes", "No");
        });
    }

    public async Task<string?> ShowPasswordDialog() {
        var page = Application.Current?.MainPage;
        if (page == null) return null;
        return await page.Dispatcher.DispatchAsync(async () => {
            //await Task.Delay(1000);
            //await page.DisplayAlert("HOGE", "FUGA", "OK");
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

}
