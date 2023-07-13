using BooPlayer.Models;
using BooPlayer.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace BooPlayer.Services.Impl;
internal class UserSettingsService : IUserSettingsService {
    private readonly IFileService _fileService;
    private readonly ILogger _logger;

    public UserSettings UserSettings { get; private set; }

    UserSettings IUserSettingsService.UserSettings => throw new NotImplementedException();

    public UserSettingsService(IFileService fileService, ILoggerFactory loggerFactory) {
        _fileService = fileService;
        _logger = loggerFactory.CreateLogger("Settings");
        UserSettings = new UserSettings();
        Load();
    }

    private void Load() {
        try {
            var jsonText = File.ReadAllText(_fileService.UserSettingsFilePath, Encoding.UTF8);
            UserSettings = JsonConvert.DeserializeObject<UserSettings>(jsonText) ?? new UserSettings();
        } catch(Exception ex) {
            _logger.Error(ex);
            UserSettings = new UserSettings();
        }
    }

    public void Save() {
        try {
            var jsonText = JsonConvert.SerializeObject(UserSettings);
            File.WriteAllText(_fileService.UserSettingsFilePath, jsonText, Encoding.UTF8);
        } catch(Exception ex) {
            _logger.Error(ex);
        }
    }
}
