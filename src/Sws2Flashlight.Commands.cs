using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.Translation;

namespace Sws2Flashlight;

/// <summary>
/// Player-facing commands: /fl toggles the flashlight, /fl on|off sets the state explicitly.
/// </summary>
public partial class Sws2Flashlight
{
    [Command("fl", registerRaw: true, permission: "", helpText: "Toggle your flashlight")]
    public void FlashlightCommand(ICommandContext context)
    {
        if (!context.IsSentByPlayer || context.Sender == null)
        {
            context.Reply(Localized(context, "flashlight.only_players"));
            return;
        }

        var player = context.Sender;
        var state = _manager.Toggle(player.PlayerID, notify: true);
        context.Reply(Localized(context, state ? "flashlight.on" : "flashlight.off"));
    }

    [Command("fl2", registerRaw: true, permission: "", helpText: "Toggle your flashlight (alias)")]
    public void FlashlightAliasCommand(ICommandContext context)
    {
        FlashlightCommand(context);
    }

    [Command("flashlight", registerRaw: true, permission: "", helpText: "Toggle your flashlight (alias)")]
    public void FlashlightFullCommand(ICommandContext context)
    {
        FlashlightCommand(context);
    }

    private string Localized(ICommandContext context, string key)
    {
        // Command is only usable by players; PlayerLocalizer falls back
        // to the server language for the console path.
        var localizer = context.Sender != null
            ? Core.Translation.GetPlayerLocalizer(context.Sender)
            : Core.Localizer;
        return localizer[key];
    }
}

