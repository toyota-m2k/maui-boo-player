using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.Models;
public class HostEntry {
    public int Id { get; set; } = 0;
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Challenge { get; set; } = null;
    public string? AccessToken { get; set; } = null;
}

