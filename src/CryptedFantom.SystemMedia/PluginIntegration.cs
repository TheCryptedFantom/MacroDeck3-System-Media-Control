using MacroDeck.Sdk;
using MacroDeck.Sdk.Actions;
using MacroDeck.Sdk.MusicPlayer;
using MacroDeck.Sdk.MusicPlayer.Actions;
using Serilog;
using Windows.Media.Control;

namespace CryptedFantom.SystemMedia;

public sealed class PluginIntegration : IPluginIntegration, IMusicPlayerProvider
{
        private readonly ILogger _logger;
        private GlobalSystemMediaTransportControlsSessionManager? _manager;

        public PluginIntegration(ILogger logger)
        {
                _logger = logger.ForContext<PluginIntegration>();
                Actions = MusicPlayerActions.Common(ResolvePlayer, GetInstances)
						.Where(a => !a.Id.Contains("shuffle", StringComparison.OrdinalIgnoreCase)
								&& !a.Id.Contains("repeat", StringComparison.OrdinalIgnoreCase)
								&& !a.Id.Contains("volume", StringComparison.OrdinalIgnoreCase))
						.ToList();
        }

        public IReadOnlyList<IActionDefinition> Actions { get; }

        public async Task InitializeAsync(IIntegrationContext context)
        {
                _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        }

        public Task ShutdownAsync() => Task.CompletedTask;

        public IReadOnlyList<MusicPlayerInstance> GetInstances()
                => _manager?.GetSessions()
                        .Select(s => new MusicPlayerInstance(s.SourceAppUserModelId, s.SourceAppUserModelId))
                        .ToList()
                        ?? [];

        public IMusicPlayer? GetPlayer(string instanceId)
        {
                var session = _manager?.GetSessions()
                        .FirstOrDefault(s => s.SourceAppUserModelId == instanceId);
                return session is null ? null : new SmtcMusicPlayer(session);
        }

        private IMusicPlayer? ResolvePlayer(string? instanceId)
                => string.IsNullOrEmpty(instanceId)
                        ? GetPlayer(_manager?.GetSessions() is [var first, ..] ? first.SourceAppUserModelId : "")
                        : GetPlayer(instanceId);
}