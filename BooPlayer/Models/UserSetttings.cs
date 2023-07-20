using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.Models;
internal class UserSettings {
    public HostEntry? CurrentHost { get; set; } = null;
    public string CurrentItemId { get; set; } = string.Empty;
    public long CurrentPosition { get; set; } = 0;
    public List<HostEntry> Hosts { get; set; } = new List<HostEntry>();

    [JsonIgnore]
    public HostEntryList EditablHostList {
        get => new HostEntryList(Hosts);
        set => Hosts = value.Hosts.Select(HostEntry.CloneEntry).ToList();
    }
}

