namespace BooPlayer.Services;
internal interface IPageService {
    Task<string?> ShowPasswordDialog();
    Task ShowConfirmationMessage(string title, string message);
    Task<bool> ShowOkCancelMessage(string title, string message);
    Task<bool> ShowYesNoMessage(string title, string message);

    Task ShowModalDialog(Page dialogPage);
    Task RunOnUIThread(Action fn);
    Task<T> RunOnUIThread<T>(Func<T> fn);
}
