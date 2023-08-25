using BooPlayer.Models;
using BooPlayer.Services;
using BooPlayer.Utils;
using Microsoft.Extensions.Logging;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;

namespace BooPlayer.ViewModel;
internal class ItemListViewModel {
    private readonly IItemListService _itemListService;
    private readonly IPageService _pageService;
    private readonly ILogger _logger;

    private static List<Item> _emptyList = new List<Item>();

    public ReactiveProperty<IHostEntry?> CurrentHostEntry { get; } = new();
    public ReactiveCommand ReloadListCommand { get; } = new();
    public ReactiveCommand SelectHostCommand { get; } = new();

    public ReactiveProperty<List<Item>> RawItemList { get; } = new(_emptyList);
    public ReadOnlyReactiveProperty<List<Item>> ItemList { get; }
    //public ReactiveProperty<int> CurrentIndex { get; } = new ( -1 );
    //public ReadOnlyReactiveProperty<Item?> CurrentItem { get; }
    public ReactiveProperty<Item?> CurrentItem { get; } = new();
    //public ReadOnlyReactiveProperty<bool> IsPhoto { get; }
    //public ReadOnlyReactiveProperty<bool> IsVideo { get; }
    //public ReadOnlyReactiveProperty<string?> CurrentVideoUrl { get; }
    //public ReadOnlyReactiveProperty<string?> CurrentPhotoUrl { get; }
    public ReactiveProperty<object?> Dummy { get; } = new();

    public ReactiveProperty<bool> IsPhoto { get; } = new();
    public ReactiveProperty<bool> IsVideo { get; } = new();
    public ReactiveProperty<string?> CurrentVideoUrl { get; } = new();
    public ReactiveProperty<string?> CurrentPhotoUrl { get; } = new();

    public ReadOnlyReactiveProperty<string> CurrentName { get; }

    public MediaPlayerModel PlayerModel { get; } = new();

    public ReactiveCommand PrevCommand { get; } = new();
    public ReactiveCommand NextCommand { get; } = new();
    public ReactiveCommand SlideCommand { get; } = new();

    enum ListFilter {
        ALL = 0,
        VIDEO = 1,
        PHOTO = 2,
        AUDIO = 3,
    }

    public ReactiveProperty<int> Filter { get; } = new(0);
    public ReadOnlyReactiveProperty<bool> PhotoFiltered { get; }

    private LifeKeeper _lifeKeeper;

    //ViewerStateManager ViewerStateManager { get; }


    public ItemListViewModel(IItemListService itemListService, IPageService pageService, ILoggerFactory loggerFactory) {
        _itemListService = itemListService;
        _pageService = pageService;
        _logger = loggerFactory.CreateLogger("ItemList");
        _lifeKeeper = new LifeKeeper(itemListService,_logger);

        CurrentHostEntry.Subscribe(LoadItemList);
        ReloadListCommand.Subscribe(ReloadItemList);

        ItemList = RawItemList.CombineLatest(Filter, (list, filter) => {
            switch (filter) {
                default:
                case (int)ListFilter.ALL:   return list;
                case (int)ListFilter.VIDEO: return list.Where(it => it.IsVideo).ToList();
                case (int)ListFilter.PHOTO: return list.Where(it => it.IsPhoto).ToList();
            }
        }).ToReadOnlyReactiveProperty(_emptyList);

        //ShowListCommand.Subscribe((it) => {
        //    ShowList.Value = it == "True";
        //});

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

        PhotoFiltered = Filter.Select(it=> it == (int)ListFilter.PHOTO).ToReadOnlyReactiveProperty();

        CurrentName = CurrentItem.Select(item => {
            if (item == null) {
                return "None";
            }
            return item.Name;
        }).ToReadOnlyReactiveProperty("");

        NextCommand.Subscribe(() => {
            NextItem(true);
        });
        PrevCommand.Subscribe(() => {
            NextItem(false);
        });
        SlideCommand.Subscribe(() => {
            ToggleSlideShow();
        });

    }

    private bool NextItem(bool next) {
        var current = CurrentItem.Value;
        if (current == null) return false;

        var index = ItemList.Value.IndexOf(current);
        if (index == -1) return false;
        if(next) {
            index++;
        } else {
            index--;
        }
        if( 0<=index && index < ItemList.Value.Count ) {
            CurrentItem.Value = ItemList.Value[index];
            return true;
        }
        return false;
    }

    CancellationTokenSource? SlideShowCancelToken = null;
    private void ToggleSlideShow() {
        lock(this) {
            if (SlideShowCancelToken == null) {
                StartSlideShow();
            } else {
                StopSlideShow();
            }
        }
    }
    private void StartSlideShow() {
        if(SlideShowCancelToken!= null) {
            return;
        }
        SlideShowCancelToken = new();
        var token = SlideShowCancelToken.Token;
        Task.Run(async () => {
            try {
                while (!token.IsCancellationRequested) {
                    await Task.Delay(1500, token);
                    if (!NextItem(true)) {
                        StopSlideShow();
                    }
                }
            } catch(Exception) {
            } finally {
                lock (this) {
                    SlideShowCancelToken = null;
                }
            }
        });

    }
    private void StopSlideShow() {
        lock (this) {
            SlideShowCancelToken?.Cancel();
        }
    }
    
    class LifeKeeper {
        private IHostEntry? _host;
        private IItemListService _itemListService;
        private ILogger _logger;

        public LifeKeeper(IItemListService itemListService, ILogger logger) {
            _itemListService = itemListService;
            _logger = logger;
        }
        public IHostEntry? Host {
            get => _host; 
            set {
                lock(this) { 
                    _host = value;
                    KeepAlive(value);
                }
            }
        }
        CancellationTokenSource? _cancellationTokenSouce = null;
        private void KeepAlive(IHostEntry? host) {
            _cancellationTokenSouce?.Cancel();
            if (host == null) {
                _cancellationTokenSouce = null;
            }
            else {
                var cts = new CancellationTokenSource();
                _cancellationTokenSouce = cts;
                Task.Run(async () => {
                    while (!cts.IsCancellationRequested) {
                        await Task.Delay(30 * 1000, cts.Token); // 30秒に１回くらいでどうだろう。
                        await _itemListService.KeepAlive(host, cts.Token);
                    }
                }, cts.Token);
            }
        }
    }


    private async void LoadItemList(IHostEntry? entry) {
        if (entry == null) {
            RawItemList.Value = _emptyList;
            _lifeKeeper.Host = null;
            return;
        }
        try {
            RawItemList.Value = (await _itemListService.GetItemListAsync(entry)).Items.ToList();
            _lifeKeeper.Host = entry;
        } catch(Exception e) {
            RawItemList.Value = _emptyList;
            _lifeKeeper.Host = null;
            _logger.Error(e);
            await _pageService.ShowConfirmationMessage("Error", $"Failed to load item list from {entry.Name}.");
        }
        var page = (Application.Current?.MainPage as FlyoutPage);
        if(page!=null) {
            page.IsPresented = true;
        }
    }

    private void ReloadItemList() {
        LoadItemList(CurrentHostEntry.Value);
    }
}
