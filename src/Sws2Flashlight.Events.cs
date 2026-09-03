using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Misc;

namespace Sws2Flashlight;

/// <summary>
/// Event wiring: F-key toggle, disconnect cleanup, map unload cleanup, death cleanup.
/// </summary>
public partial class Sws2Flashlight
{
    /// <summary>
    /// Handles client key state changes. F key press toggles the flashlight.
    /// </summary>
    private void OnClientKeyStateChanged(IOnClientKeyStateChangedEvent @event)
    {
        if (!@event.Pressed || @event.Key != KeyKind.F)
        {
            return;
        }

        var player = Core.PlayerManager.GetPlayer(@event.PlayerId);
        if (player == null || !player.IsValid)
        {
            return;
        }

        _manager.Toggle(player.PlayerID, notify: _config.Behavior.NotifyOnToggle);
    }

    /// <summary>
    /// Cleans up a player's flashlight when they disconnect.
    /// </summary>
    private void OnClientDisconnected(IOnClientDisconnectedEvent @event)
    {
        _manager.CleanupPlayer(@event.PlayerId);
    }

    /// <summary>
    /// Cleans up all flashlights when the map unloads.
    /// </summary>
    private void OnMapUnload(IOnMapUnloadEvent @event)
    {
        _manager.CleanupAll();
    }

    /// <summary>
    /// Turns off the flashlight when a player dies (if configured).
    /// </summary>
    private HookResult OnPlayerDeath(EventPlayerDeath @event)
    {
        if (!_config.Behavior.TurnOffOnDeath)
        {
            return HookResult.Continue;
        }

        var victim = @event.UserIdPlayer;
        if (victim != null)
        {
            _manager.TurnOff(victim.PlayerID, "player_death");
        }

        return HookResult.Continue;
    }

    /// <summary>
    /// Turns off the flashlight when a player switches teams (if configured).
    /// </summary>
    private HookResult OnPlayerTeam(EventPlayerTeam @event)
    {
        if (!_config.Behavior.TurnOffOnTeamSwitch)
        {
            return HookResult.Continue;
        }

        var player = @event.UserIdPlayer;
        if (player != null)
        {
            _manager.TurnOff(player.PlayerID, "team_switch");
        }

        return HookResult.Continue;
    }
}