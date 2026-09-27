using MacroDeck.Sdk.MusicPlayer;
using Windows.Media;
using Windows.Media.Control;
using Windows.Storage.Streams;


namespace CryptedFantom.SystemMedia;

internal sealed class SmtcMusicPlayer(GlobalSystemMediaTransportControlsSession session) : IMusicPlayer
{
        public async Task<MusicPlayerArtwork?> GetArtworkAsync(string artworkId, CancellationToken cancellationToken = default)
        {
                var props = await session.TryGetMediaPropertiesAsync();
                if (props.Thumbnail is null)
                {
                        return null;
                }

                using var stream = await props.Thumbnail.OpenReadAsync();
                var bytes = new byte[stream.Size];
                using var reader = new DataReader(stream);
                await reader.LoadAsync((uint)stream.Size).AsTask(cancellationToken);
                reader.ReadBytes(bytes);
                return new MusicPlayerArtwork(bytes, stream.ContentType);
        }

        public async Task<MusicPlayerState> GetStateAsync(CancellationToken cancellationToken = default)
        {
                var props = await session.TryGetMediaPropertiesAsync();
                var playback = session.GetPlaybackInfo();
                var timeline = session.GetTimelineProperties();

                return new MusicPlayerState
                {
                        IsConnected = true,
                        PlaybackState = playback.PlaybackStatus switch
                        {
                                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing => PlaybackState.Playing,
                                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused => PlaybackState.Paused,
                                _ => PlaybackState.Stopped
                        },
                        TrackName = props.Title,
                        Artists = string.IsNullOrEmpty(props.Artist) ? [] : [props.Artist],
                        AlbumName = props.AlbumTitle,
                        Position = timeline.Position,
                        Duration = timeline.EndTime - timeline.StartTime,
                        ShuffleEnabled = playback.IsShuffleActive ?? false,
                        RepeatMode = playback.AutoRepeatMode switch
                        {
                                MediaPlaybackAutoRepeatMode.Track => RepeatMode.Track,
                                MediaPlaybackAutoRepeatMode.List => RepeatMode.Context,
                                _ => RepeatMode.Off
                        }
                };
        }

        public Task PlayAsync(CancellationToken cancellationToken = default)
                => session.TryPlayAsync().AsTask(cancellationToken);

        public Task PauseAsync(CancellationToken cancellationToken = default)
                => session.TryPauseAsync().AsTask(cancellationToken);

        public Task TogglePlayPauseAsync(CancellationToken cancellationToken = default)
                => session.TryTogglePlayPauseAsync().AsTask(cancellationToken);

        public Task NextAsync(CancellationToken cancellationToken = default)
                => session.TrySkipNextAsync().AsTask(cancellationToken);

        public Task PreviousAsync(CancellationToken cancellationToken = default)
                => session.TrySkipPreviousAsync().AsTask(cancellationToken);

        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
        {
                if (!session.GetPlaybackInfo().Controls.IsPlaybackPositionEnabled)
                {
                        throw new NotSupportedException("This app doesn't expose seeking.");
                }
                return session.TryChangePlaybackPositionAsync(position.Ticks).AsTask(cancellationToken);
        }

        public Task SetShuffleAsync(bool enabled, CancellationToken cancellationToken = default)
                => throw new NotSupportedException("Shuffle isn't supported by this plugin.");

        public Task SetRepeatModeAsync(RepeatMode mode, CancellationToken cancellationToken = default)
                => throw new NotSupportedException("Repeat isn't supported by this plugin.");
        
        public Task SetVolumeAsync(int volumePercent, CancellationToken cancellationToken = default)
                => throw new NotSupportedException(
                "System media sessions don't expose volume; use the app or Windows volume mixer instead.");
}