using System.Text.Json.Serialization;

namespace SWS2Flashlight.Configuration;

/// <summary>
/// SWS2Flashlight plugin configuration (config.jsonc, bound from section "SWS2Flashlight").
/// </summary>
public sealed class FlashlightConfig
{
    /// <summary>Enable debug logging.</summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Behavior settings.
    /// </summary>
    public FlashlightBehavior Behavior { get; set; } = new();

    /// <summary>
    /// Light entity appearance settings.
    /// </summary>
    public FlashlightLight Light { get; set; } = new();
}

/// <summary>
/// Behavior settings.
/// </summary>
public sealed class FlashlightBehavior
{
    /// <summary>
    /// Whether the flashlight updates its position every tick (64/s) or every N ticks to reduce overhead.
    /// Default: every tick for smooth tracking.
    /// </summary>
    public int UpdateIntervalTicks { get; set; } = 1;

    /// <summary>
    /// Whether to show a chat message when toggling the flashlight.
    /// </summary>
    public bool NotifyOnToggle { get; set; } = true;

    /// <summary>
    /// Whether the flashlight is automatically turned off when the player dies.
    /// </summary>
    public bool TurnOffOnDeath { get; set; } = true;

    /// <summary>
    /// Whether the flashlight is automatically turned off when the player switches teams.
    /// </summary>
    public bool TurnOffOnTeamSwitch { get; set; } = true;

    /// <summary>
    /// Whether to disable flashlight in spectator mode.
    /// </summary>
    public bool DisableInSpectator { get; set; } = true;
}

/// <summary>
/// Light entity appearance settings (applies to the light_spot created per player).
/// </summary>
public sealed class FlashlightLight
{
    /// <summary>
    /// Light color as RGB hex string, e.g. "#FFFFFF" (white) or "#FFEECC" (warm white).
    /// </summary>
    public string Color { get; set; } = "#FFFFFF";

    /// <summary>
    /// Light brightness (0.0 - 1.0 or higher for stronger light; values >1 are clamped by the engine).
    /// </summary>
    public float Brightness { get; set; } = 1.0f;

    /// <summary>
    /// Maximum range of the light in game units (1 unit ≈ 1 cm, 8192 ≈ 82m).
    /// </summary>
    public float Range { get; set; } = 2000f;

    /// <summary>
    /// Spot light cone inner angle in degrees (light cone half-angle; smaller = tighter beam).
    /// </summary>
    public float Theta { get; set; } = 15f;

    /// <summary>
    /// Spot light cone outer angle in degrees.
    /// </summary>
    public float Phi { get; set; } = 25f;

    /// <summary>
    /// Light falloff. Higher values = light fades faster.
    /// </summary>
    public float Falloff { get; set; } = 1.0f;

    /// <summary>
    /// Whether the light casts shadows (more realistic but more expensive).
    /// </summary>
    public bool CastShadows { get; set; } = false;

    /// <summary>
    /// Whether the light should only be visible to the owner (via transmit state).
    /// </summary>
    public bool OwnerOnly { get; set; } = true;
}
