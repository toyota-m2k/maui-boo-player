using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.Models;

internal class ItemList {
    [JsonProperty("list")]
    public IList<Item> Items { get; } = new List<Item>();
    [JsonProperty("date")]
    public long Date { get; } = 0;
}


