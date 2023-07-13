namespace BooPlayer.Services;
internal interface IMainThreadService {
    void Run(Action f);
    T Run<T>(Func<T> f);
    public void Run(Task t);
    public bool IsMainThread { get; }
}
