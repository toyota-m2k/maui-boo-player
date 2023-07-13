using Microsoft.Extensions.Configuration;

namespace BooPlayer.Services.Impl;
internal class FileService : IFileService {
    private readonly IConfiguration _configuration;

    public string DataDirectory => FileSystem.AppDataDirectory;
    public string CacheDirectory => FileSystem.CacheDirectory;

    public string SQLiteFilePath => Path.Combine(DataDirectory, _configuration["SQLiteFileName"]??"boo.db");
    public string UserSettingsFilePath => Path.Combine(DataDirectory, _configuration["UserSettingsFileName"]?? "user.json");

    public FileService(IConfiguration configuration) {
        _configuration = configuration;
    }
}
