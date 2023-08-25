using BooPlayer.Models;
using BooPlayer.Services;
using BooPlayer.Utils;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.ViewModel;

internal class HostListViewModel {
    private readonly IUserSettingsService _userSettingsService;
    private readonly HostEntryList _hostEntryList;
    private readonly ItemListViewModel _itemListViewModel;

    public ObservableCollection<IHostEntry> Hosts => _hostEntryList.Hosts;
    public ReactiveProperty<IHostEntry?> SelectedHost { get; } = new((IHostEntry?)null);
    public ReadOnlyReactiveProperty<bool> IsReady { get; }
    public bool Cancellable { get; }

    public ReactiveProperty<bool> IsEditing { get; } = new(false);
    public ReactiveProperty<string> EditingAddress { get; } = new(string.Empty);
    public ReactiveProperty<string> EditingName { get; } = new(string.Empty);
    public ReadOnlyReactiveProperty<bool> CanCompleteEdit { get; }

    public ReactiveCommand AddHostCommand { get; } = new();
    public ReactiveCommand<IHostEntry?> BeginEditCommand { get; } = new();
    public ReactiveCommand<string> EndEditCommand { get; } = new();
    public ReactiveCommand<IHostEntry> RemoveHostCommand { get; } = new();
    public ReactiveCommand<string> CompleteCommand { get; } = new();

    public HostListViewModel(IUserSettingsService userSettingsService, ItemListViewModel itemListViewModel) {
        _userSettingsService = userSettingsService;
        _hostEntryList = _userSettingsService.UserSettings.EditablHostList;
        _itemListViewModel = itemListViewModel;

        IsReady = IsEditing.CombineLatest(SelectedHost, (editing, host) => !editing && host != null).ToReadOnlyReactiveProperty();
        CanCompleteEdit = EditingAddress.CombineLatest(EditingName, (address,name) => address.IsNotEmpty() && name.IsNotEmpty()).ToReadOnlyReactiveProperty();
        //SelectionMode = IsEditing.Select(editing => editing ? Microsoft.Maui.Controls.SelectionMode.Multiple : Microsoft.Maui.Controls.SelectionMode.Single).ToReadOnlyReactiveProperty();
        Cancellable = _itemListViewModel.CurrentHostEntry.Value != null;

        AddHostCommand.Subscribe(() => {
            BeginEditCommand.Execute(null);
        });

        BeginEditCommand.Subscribe(host => {
            //host = host ?? SelectedHost.Value;
            if (host != null) {
                EditingAddress.Value = host.Address;
                EditingName.Value = host.Name;
            }
            IsEditing.Value = true;
        });
        EndEditCommand.Subscribe(ok => {
            if(ok=="True" && EditingAddress.Value.IsNotEmpty() && EditingName.Value.IsNotEmpty()) {
                _hostEntryList.AddOrUpdateHost(EditingAddress.Value, EditingName.Value);
            }
            IsEditing.Value = false;
        });
        RemoveHostCommand.Subscribe(host => {
            if (host != null) {
                _hostEntryList.RemoveHost(host.Address);
            }
        });
        CompleteCommand.Subscribe(ok => {
            if (ok == "True") {
                var host = SelectedHost.Value;
                _userSettingsService.UserSettings.CurrentHost = host as HostEntry;
                _userSettingsService.UserSettings.EditablHostList = _hostEntryList;
                _userSettingsService.Save();
                if (host != null) {
                    _itemListViewModel.CurrentHostEntry.Value = host;
                }
            }
        });
    }
}
