using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.Models;

public interface IHostEntry {
    public string Address { get; }
    public string Name { get; }
    [JsonIgnore]
    public string? Challenge { get; set; }
    [JsonIgnore]
    public string? AccessToken { get; set; }

}

public class HostEntry : IHostEntry {
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public string? Challenge { get; set; } = null;
    [JsonIgnore]
    public string? AccessToken { get; set; } = null;

    public static IHostEntry NewEntry(string address, string name) {
        return new HostEntry() {
            Address = address,
            Name = name,
        };
    }
    public static IHostEntry UpdateEntry(IHostEntry orgEntry, string address, string name) {
        return new HostEntry() {
            Address = address,
            Name = name,
            Challenge = orgEntry.Challenge,
            AccessToken = orgEntry.AccessToken,
        };
    }
    public static HostEntry CloneEntry(IHostEntry orgEntry) {
        return new HostEntry() {
            Address = orgEntry.Address,
            Name = orgEntry.Name,
            Challenge = orgEntry.Challenge,
            AccessToken = orgEntry.AccessToken,
        };
    }
}

