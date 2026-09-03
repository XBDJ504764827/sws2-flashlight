using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Scheduler;
using SWS2Flashlight.Configuration;

namespace SWS2Flashlight.Services;

/// <summary>
/// Manages per-player flashlight entities: creation, per-tick following, teardown on death/leave/map change.
/// </summary>
public sealed class FlashlightManager : IDisposable
{
    private readonly ISwiftlyCore _core;
    private readonly FlashlightConfig _config;

    private readonly Dictionary<int, FlashlightEntity> _flashlights = new();
    private readonly object _lock = new();

    private CancellationTokenSource? _tickCts;

    public FlashlightManager(ISwiftlyCore core, FlashlightConfig config)
    {
        _core = core;
        _config = config;
    }

    public void Start()
    {
        Stop();
        _tickCts = _core.Scheduler.RepeatBySeconds(1f / 64f * Math.Max(1, _config.Behavior.UpdateIntervalTicks), UpdateAll);
    }

    public void Stop()
    {
        _tickCts?.Cancel();
        _tickCts?.Dispose();
        _tickCts = null;
    }

    /// <summary>
    /// Applies a new config to the manager and refreshes existing light entities.
    /// </summary>
    public void ApplyConfig(FlashlightConfig config)
    {
        lock (_lock)
        {
            foreach (var (playerId, flashlight) in _flashlights)
            {
                flashlight.Dispose();
                CreateFor(playerId);
            }
        }
    }

    /// <summary>
    /// Turns on the flashlight for the given player (idempotent).
    /// </summary>
    public bool TurnOn(int playerId, bool notify = false)
    {
        lock (_lock)
        {
            if (_flashlights.ContainsKey(playerId))
            {
                return false; // already on
            }

            return CreateFor(playerId, notify);
        }
    }

    /// <summary>
    /// Turns off the flashlight for the given player (idempotent).
    /// </summary>
    public void TurnOff(int playerId, string reason = "")
    {
        lock (_lock)
        {
            if (_flashlights.TryGetValue(playerId, out var flashlight))
            {
                flashlight.Dispose();
                _flashlights.Remove(playerId);
                _core.Logger.LogDebug("[SWS2Flashlight] Flashlight off for player {PlayerId} ({Reason})", playerId, reason);
            }
        }
    }

    public Task TurnOffAsync(int playerId, string reason = "")
    {
        return _core.Scheduler.NextTickAsync(() => TurnOff(playerId, reason));
    }

    /// <summary>
    /// Returns whether the given player's flashlight is on.
    /// </summary>
    public bool IsOn(int playerId)
    {
        lock (_lock)
        {
            return _flashlights.ContainsKey(playerId);
        }
    }

    /// <summary>
    /// Toggles the flashlight for the given player. Returns the new state (true = on).
    /// </summary>
    public bool Toggle(int playerId, bool notify)
    {
        lock (_lock)
        {
            if (_flashlights.ContainsKey(playerId))
            {
                TurnOff(playerId, "toggle");
                return false;
            }

            return CreateFor(playerId, notify);
        }
    }

    /// <summary>
    /// Cleans up a player's flashlight (e.g., on disconnect, death, or map unload).
    /// </summary>
    public void CleanupPlayer(int playerId)
    {
        TurnOff(playerId, "cleanup");
    }

    /// <summary>
    /// Cleans up all flashlights (e.g., on map unload or plugin unload).
    /// </summary>
    public void CleanupAll()
    {
        lock (_lock)
        {
            foreach (var flashlight in _flashlights.Values)
            {
                flashlight.Dispose();
            }
            _flashlights.Clear();
        }
    }

    /// <summary>
    /// Creates a flashlight for the given player.
    /// </summary>
    private bool CreateFor(int playerId, bool notify = false)
    {
        var player = _core.PlayerManager.GetPlayer(playerId);
        if (player == null || !player.IsValid || player.IsFakeClient)
        {
            return false;
        }

        if (!player.IsAlive)
        {
            return false;
        }

        if (_config.Behavior.DisableInSpectator && player.PlayerPawn is { Team: Team.Spectator })
        {
            return false;
        }

        var flashlight = new FlashlightEntity(_core, _config, playerId);
        flashlight.Create();

        if (!flashlight.IsActive)
        {
            flashlight.Dispose();
            return false;
        }

        _flashlights[playerId] = flashlight;

        if (notify && _config.Behavior.NotifyOnToggle)
        {
            var localizer = _core.Translation.GetPlayerLocalizer(player);
            player.SendChat($"[SWS2Flashlight] {localizer["flashlight.on"]}");
        }

        _core.Logger.LogDebug("[SWS2Flashlight] Flashlight on for player {PlayerId}", playerId);
        return true;
    }

    /// <summary>
    /// Updates all active flashlight positions (called on the tick loop).
    /// </summary>
    private void UpdateAll()
    {
        if (_flashlights.Count == 0)
        {
            return;
        }

        List<KeyValuePair<int, FlashlightEntity>> snapshot;
        lock (_lock)
        {
            snapshot = _flashlights.ToList();
        }

        foreach (var (playerId, flashlight) in snapshot)
        {
            flashlight.UpdateTransform();
        }
    }

    public void Dispose()
    {
        Stop();
        CleanupAll();
    }
}
