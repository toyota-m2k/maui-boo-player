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
    //public ReadOnlyReactiveProperty<bool> IsPhoto { get; }
    //public ReadOnlyReactiveProperty<bool> IsVideo { get; }
    //public ReadOnlyReactiveProperty<string?> CurrentVideoUrl { get; }
    //public ReadOnlyReactiveProperty<string?> CurrentPhotoUrl { get; }

    public ReactiveProperty<bool> IsPhoto { get; } = new ();
    public ReactiveProperty<bool> IsVideo { get; } = new();
    public ReactiveProperty<string?> CurrentVideoUrl { get; } = new();
    public ReactiveProperty<string?> CurrentPhotoUrl { get; } = new();

    public ReadOnlyReactiveProperty<string> CurrentName { get; }


    public MediaPlayerModel PlayerModel { get; } = new ();

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


        CurrentItem.Subscribe(item => {
            if (item == null) {
                IsPhoto.Value = false;
                IsVideo.Value = false;
                CurrentPhotoUrl.Value = null;
                CurrentVideoUrl.Value = null;
            }
            else {
                if (IsVideo.Value) {
                    // 動画再生中のときは、一旦、ソースをnullにしてから、0.5秒後に再設定する。
                    // ソースの変更より先に（または、ほぼ同時に）MediaElement が非表示になってしまうと、（おそらく）Sourceへのバインドが無視されて
                    // それまでの動画ファイルのダウンロードがキャンセルされず、無駄な通信が発生する。
                    // SecureArchive サーバーの場合は、同時に１つのファイルしかダウンロードできないので、動画ファイルの受信が終わるまで、次に選択された動画や写真が表示されなくなる。
                    CurrentVideoUrl.Value = null;
                    App.Current?.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(0.5), () => {
                        IsPhoto.Value = item.IsPhoto;
                        IsVideo.Value = item.IsVideo;
                        CurrentPhotoUrl.Value = item.IsPhoto ? item.GetUrl(CurrentHostEntry.Value) : null;
                        CurrentVideoUrl.Value = item.IsVideo ? item.GetUrl(CurrentHostEntry.Value) : null;
                    });
                } else {
                    IsPhoto.Value = item.IsPhoto;
                    IsVideo.Value = item.IsVideo;
                    CurrentPhotoUrl.Value = item.IsPhoto ? item.GetUrl(CurrentHostEntry.Value) : null;
                    CurrentVideoUrl.Value = item.IsVideo ? item.GetUrl(CurrentHostEntry.Value) : null;
                }
            }
        });

        // 通常なら、次の実装でよいのだが、動画ダウンロードがキャンセルされない問題があるので、上記の実装に変更した。

        //CurrentPhotoUrl = CurrentItem.CombineLatest(CurrentHostEntry, (item, host) => {
        //    if (item != null && host != null && item.IsPhoto) {
        //        return item.GetUrl(host);
        //    }
        //    return null;
        //}).ToReadOnlyReactiveProperty(null);

        //CurrentVideoUrl = CurrentItem.CombineLatest(CurrentHostEntry, (item, host) => {
        //    if (item != null && host != null && item.IsVideo) {
        //        return item.GetUrl(host);
        //    }
        //    return null;
        //}).ToReadOnlyReactiveProperty(null);

        //IsPhoto = CurrentItem.Select(item => {
        //    if (item == null) {
        //        return false;
        //    }
        //    return item.IsVideo;
        //}).ToReadOnlyReactiveProperty(false);
        
        //IsVideo = CurrentItem.Select(item => {
        //    if (item == null) {
        //        return false;
        //    }
        //    return item.IsVideo;
        //}).ToReadOnlyReactiveProperty(false);

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
