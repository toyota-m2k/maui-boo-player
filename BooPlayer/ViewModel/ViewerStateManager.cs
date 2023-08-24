using BooPlayer.Utils;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.ViewModel; 

/**
 * 初期状態  Flyout表示、コントロールパネル表示
 * 画面タップ
 *      動画モード
 *          停止中（パネル表示） --> 再生開始（パネル表示）
 *          再生中（パネル表示） --> 再生継続（パネル非表示）
 *          再生中（パネル非表示）
 *      
 * １回目 --> プレイボタンで、再生開始（３秒後にコントロールパネルを隠す）
 *          ２回目 --> コントロールパネル表示中なら、コントロールパネルを消す
 *          
 */
//internal class ViewerStateManager {
//    public ReactiveProperty<bool> SlideShow = new();
//    public ReadOnlyReactiveProperty<bool> ShowPanel;

//    private enum PanelState {
//        Shown,    // 表示中
//        Hiding,   // 表示中（３秒後に隠す）
//        Hidden,    // 非表示
//    }

//    //private enum PlayerState {
//    //    Photo,          // 静止画
//    //    VideoStopped,
//    //    VideoPlaying,
//    //}

//    private WeakReference<ItemListViewModel> mListViewModelRef;
//    private ItemListViewModel ItemListViewModel => mListViewModelRef.GetValue()!;

//    private ReactiveProperty<PanelState> State { get; } = new ReactiveProperty<PanelState>(PanelState.Shown);
//    private ReactiveProperty<bool> LockPanel { get; } = new ReactiveProperty<bool>(false);

//    private long autoHideTick = 0;
//    private CancellationTokenSource? autoHideCancellation = null;

//    public ViewerStateManager(ItemListViewModel listViewModel) {
//        mListViewModelRef = new WeakReference<ItemListViewModel>(listViewModel);
//        listViewModel.PlayerModel.IsPlaying.Subscribe(playing => {
//            if(playing) {
//                SetState(PanelState.Hiding);
//            } else {
//                SetState(PanelState.Shown);
//            }
//        });
//    }



//    public void Postpone() {
//        lock(this) {
//            if(State.Value == PanelState.Hiding) { 
//                autoHideTick = DateTime.Now.Ticks;
//            }
//        }
//    }

//    public void NextStateOnClick() {
//        lock (this) {
//            switch (State.Value) {
//                default:
//                case PanelState.Hiding:
//                    ItemListViewModel.PlayerModel.PauseCommand.Execute();
//                    break;
//                case PanelState.Shown:
//                    ItemListViewModel.PlayerModel.PlayCommand.Execute();
//                    break;
//                case PanelState.Hidden:
//                    SetState(PanelState.Hiding);
//                    break;
//            }
//        }
//    }

//    public void SetState(PanelState state) {
//        lock(this) {
//            if (State.Value == state) {
//                // 変更なし
//                if (state == PanelState.Hiding) {
//                    // --> PlayingWithPanelの場合だけ Touch
//                    Postpone();
//                }
//            } else {
//                if (State.Value == PanelState.Hiding) {
//                    StopWatching();
//                } else if( state == PanelState.Hiding) {
//                    StartWatching();
//                }
//                State.Value = state;
//            }
//        }
//    }

//    void StartWatching() {

//    }

//    void StopWatching() {

//    }
//}
