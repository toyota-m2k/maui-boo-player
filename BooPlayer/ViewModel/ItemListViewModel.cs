using BooPlayer.Models;
using BooPlayer.Services;
using BooPlayer.Utils;
using Microsoft.Extensions.Logging;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace BooPlayer.ViewModel;
internal class ItemListViewModel {
    private readonly IItemListService _itemListService;
    private readonly IPageService _pageService;
    private readonly ILogger _logger;

    private static List<Item> _emptyList = new List<Item>();

    public ReactiveProperty<HostEntry?> CurrentHostEntry { get; } = new ();
    public ReactiveCommand ReloadListCommand { get; } = new ();
    public ReactiveProperty<List<Item>> ItemList { get; } = new ( _emptyList );
    //public ReactiveProperty<int> CurrentIndex { get; } = new ( -1 );
    //public ReadOnlyReactiveProperty<Item?> CurrentItem { get; }
    public ReactiveProperty<Item?> CurrentItem { get; } = new ();
    public ReadOnlyReactiveProperty<bool> IsPhoto { get; }
    public ReadOnlyReactiveProperty<bool> IsVideo { get; }
    //public ReadOnlyReactiveProperty<string?> CurrentUrl { get; }
    public ReadOnlyReactiveProperty<string?> CurrentVideoUrl { get; }
    public ReadOnlyReactiveProperty<string?> CurrentPhotoUrl { get; }

    public ReadOnlyReactiveProperty<string> CurrentName { get; }


    public ItemListViewModel(IItemListService itemListService, IPageService pageService, ILoggerFactory loggerFactory) {
        _itemListService = itemListService;
        _pageService = pageService;
        _logger = loggerFactory.CreateLogger("ItemList");

        CurrentHostEntry.Subscribe(LoadItemList);
        ReloadListCommand.Subscribe(ReloadItemList);


        //CurrentItem = CurrentIndex.Select(index => {
        //    if (index < 0 || index >= ItemList.Value.Count) {
        //        return null;
        //    }
        //    var hostEntry = CurrentHostEntry.Value;
        //    if(hostEntry == null) {
        //        return null;
        //    }
        //    return ItemList.Value[index];
        //}).ToReadOnlyReactiveProperty(null);

        IsPhoto = CurrentItem.Select(item => {
            if (item == null) {
                return false;
            }
            return item.Type == "jpg" || item.Type == "png";
        }).ToReadOnlyReactiveProperty(false);
        
        IsVideo = CurrentItem.Select(item => {
            if (item == null) {
                return false;
            }
            return item.Type == "mp4" || item.Type == "mp3";
        }).ToReadOnlyReactiveProperty(false);

        CurrentPhotoUrl = IsPhoto.CombineLatest(CurrentItem, CurrentHostEntry, (isPhoto, item, host) => { 
            if (isPhoto && item != null && host!=null) {
                return item.GetUrl(host);
            }
            return null;
        }).ToReadOnlyReactiveProperty(null);
        CurrentVideoUrl = IsVideo.CombineLatest(CurrentItem, CurrentHostEntry, (isVideo, item, host) => {
            if (isVideo && item != null && host != null) {
                return item.GetUrl(host);
            }
            return null;
        }).ToReadOnlyReactiveProperty(null);

        CurrentName = CurrentItem.Select(item => {
            if (item == null) {
                return "None";
            }
            return item.Name;
        }).ToReadOnlyReactiveProperty("");
    }


    private async void LoadItemList(HostEntry? entry) {
        if (entry == null) {
            ItemList.Value = _emptyList;
            return;
        }
        try {
            ItemList.Value = (await _itemListService.GetItemListAsync(entry)).Items.ToList();
        } catch(Exception e) {
            ItemList.Value = _emptyList;
            _logger.Error(e);
            await _pageService.ShowConfirmationMessage("Error", $"Failed to load item list from {entry.Name}.");
        }
    }

    private void ReloadItemList() {
        LoadItemList(CurrentHostEntry.Value);
    }
}
