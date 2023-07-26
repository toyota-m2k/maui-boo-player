using BooPlayer.Utils;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace BooPlayer.ViewModel;

internal class MediaPlayerModel {
    public IReadOnlyReactiveProperty<MediaElementState> ROCurrentState { get; } = new ReactiveProperty<MediaElementState>(MediaElementState.None);
    public IReadOnlyReactiveProperty<TimeSpan> RODuration { get; } = new ReactiveProperty<TimeSpan>(TimeSpan.Zero);
    public IReadOnlyReactiveProperty<TimeSpan> ROPosition { get; } = new ReactiveProperty<TimeSpan>(TimeSpan.Zero);
    public IReadOnlyReactiveProperty<int> ROMediaWidth { get; } = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> ROMediaHeight { get; } = new ReactiveProperty<int>(0);

    public ReactiveProperty<double> Volume { get; } = new ReactiveProperty<double>(0.5);
    public ReactiveProperty<double> Speed { get; } = new ReactiveProperty<double>(1);
    public ReadOnlyReactiveProperty<bool> IsPlaying { get; }
    public ReadOnlyReactiveProperty<bool> IsReady { get; }

    public ReactiveCommand PlayCommand { get; } = new();
    public ReactiveCommand PauseCommand { get; } = new();
    public ReactiveCommand StopCommand { get; } = new();

    class SeekMediator : IDisposable {
        private WeakReference<MediaElement>? mPlayer;
        private MediaElement? Player {
            get => mPlayer.GetValue();
            set => mPlayer = (value == null) ? null : new WeakReference<MediaElement>(value);
        }

        private WeakReference<Slider>? mSlider;
        private Slider? Slider {
            get => mSlider.GetValue();
            set => mSlider = (value == null) ? null : new WeakReference<Slider>(value);
        }


        private readonly MediaPlayerModel Model;

        public SeekMediator(MediaPlayerModel model, MediaElement player, Slider slider) {
            Model = model;
            Player = player;
            Slider = slider;

            player.PositionChanged += OnPlayerPositionChanged;
            slider.ValueChanged += OnSliderValueChanged;
            slider.DragStarted += OnSliderDragStarted;
            slider.DragCompleted += OnSliderDragCompleted;

            //model.ROCurrentState.Subscribe(state => {
            //    if(state==MediaElementState.Playing) {
            //        startPositionWatcher();
            //    } else {
            //        stopPositionWatcher();
            //    }
            //});

            model.PlayCommand.Subscribe(_ => {
                Player?.Play();
            });
            model.PauseCommand.Subscribe(_ => {
                Player?.Pause();
            });
            model.StopCommand.Subscribe(_ => {
                Player?.Stop();
            });

        }

        private bool mDragging = false;
        private bool mPlayingBeforeDragging = false;

        private void OnSliderDragCompleted(object? sender, EventArgs e) {
            if(mPlayingBeforeDragging) {
                Player?.Play();
            }
            mDragging = false;
        }

        private void OnSliderDragStarted(object? sender, EventArgs e) {
            mPlayingBeforeDragging = Model.IsPlaying.Value;
            Player?.Pause();
            if(sender is Slider slider) {
                SeekTo(TimeSpan.FromSeconds(slider.Value));
            }
            mDragging = true;
        }

        private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e) {
            if (mDragging) {
                SeekTo(TimeSpan.FromSeconds(e.NewValue));
            }
        }

        private void OnPlayerPositionChanged(object? sender, MediaPositionChangedEventArgs e) {
            var slider = Slider;
            if(slider!=null && !mDragging) {
                slider.Value = e.Position.TotalSeconds;
            }
        }

        public void SeekTo(TimeSpan position) {
            Player?.SeekTo(position);
        }

        public void Dispose() {
            Player?.Stop();
            Player = null;
            Slider = null;
        }
    }

    private SeekMediator mSeekMediator = null!;

    public void AttachPlayer(MediaElement player, Slider slider) {
        mSeekMediator = new SeekMediator(this, player, slider);
    }

    public void DetachPlayer() {
        mSeekMediator?.Dispose();
        mSeekMediator = null!;
    }

    public MediaPlayerModel() {
        IsPlaying = ROCurrentState.Select(state => state == MediaElementState.Playing).ToReadOnlyReactiveProperty();
        IsReady = ROCurrentState.Select(state => state == MediaElementState.Playing || state == MediaElementState.Paused || state== MediaElementState.Stopped).ToReadOnlyReactiveProperty();
    }
}
