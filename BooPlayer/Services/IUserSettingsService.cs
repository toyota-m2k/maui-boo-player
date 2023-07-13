using BooPlayer.Models;

namespace BooPlayer.Services;


internal interface IUserSettingsService {
    UserSettings UserSettings { get; }
    void Save();
}
