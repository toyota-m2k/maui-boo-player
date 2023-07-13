namespace BooPlayer.Services;

internal interface IFileService {
    string DataDirectory { get; }
    string CacheDirectory { get; }

    public string SQLiteFilePath { get; }
    public string UserSettingsFilePath { get; }
}
