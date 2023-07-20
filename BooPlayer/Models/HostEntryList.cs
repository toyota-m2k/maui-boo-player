using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.Models; 
internal class HostEntryList {
    public ObservableCollection<IHostEntry> Hosts { get; } 

    public HostEntryList(IList<HostEntry> entries) {
        Hosts = new ObservableCollection<IHostEntry>(entries);
    }

    public IHostEntry? GetHostEntry(string address) {
        return Hosts.FirstOrDefault(it => it.Address == address);
    }
    public int IndexOfHostEntry(string address) {
        var entry = GetHostEntry(address);
        if(entry!=null) {
            return Hosts.IndexOf(entry);
        } else {
            return -1;
        }
    }

    public IHostEntry? AddHost(string address, string name) {
        var org = GetHostEntry(address);
        if (org!=null) {
            return null;
        }
        else {
            var host = HostEntry.NewEntry(address, name);
            Hosts.Add(host);
            return host;
        }
    }

    public IHostEntry AddOrUpdateHost(string address, string name) {
        IHostEntry? host;
        host = GetHostEntry(address);
        if (host != null) {
            if(host.Name != name) {
                var index = Hosts.IndexOf(host);
                host = HostEntry.UpdateEntry(host, address, name);
                Hosts[index] = host;        
            }
        }
        else {
            host = HostEntry.NewEntry(address, name);
            Hosts.Add(host);
        }
        return host;
    }

    public void RemoveHost(string address) {
        var org = GetHostEntry(address);
        if (org!=null) {
            Hosts.Remove(org);
        }
    }
}
