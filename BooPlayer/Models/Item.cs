using Newtonsoft.Json;
using BooPlayer.Utils;

namespace BooPlayer.Models;

public class Item {
    [JsonProperty("id")]
    public string Id { get; set; } = "";
    [JsonProperty("name")]
    public string Name { get; set; } = "";
    [JsonProperty("size")]
    public long Size { get; set; }
    [JsonProperty("date")]
    public long Date { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; } = "";
    [JsonProperty("duration")]
    public long Duration { get; set; }

    public string? GetUrl(HostEntry host) {
        string type;
        string auth = "";
        if (Type == "mp4" || Type == "mp3") {
            type = "video";
        }
        else if (Type == "jpg" || Type == "png") {
            type = "photo";
        }
        else {
            return null;    // unsupported type
        }
        if (host.AccessToken.IsNotEmpty()) {
            auth = $"&auth={host.AccessToken}";
        }
        return $"http://{host.Address}/{type}?id={Id}{auth}";
    }
}
