namespace Sws2Flashlight.Configuration;

/// <summary>
/// Sws2Flashlight plugin configuration (config.jsonc, bound from section "sws2-flashlight").
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
/// Light entity appearance settings (applies to the light_barn created per player).
/// </summary>
public sealed class FlashlightLight
{
    /// <summary>
    /// Light color as RGB hex string, e.g. "#FFFFFF" (white) or "#FFEECC" (warm white).
    /// </summary>
    public string Color { get; set; } = "#FFFFFF";

    /// <summary>
    /// Light color temperature in Kelvin (1000 - 12000). 6500K is daylight white.
    /// </summary>
    public float ColorTemperature { get; set; } = 6500f;

    /// <summary>
    /// Light brightness (0.0 - 1.0 or higher for stronger light; values >1 are clamped by the engine).
    /// </summary>
    public float Brightness { get; set; } = 1.0f;

    /// <summary>
    /// Maximum range of the light in game units (1 unit ≈ 1 cm, 8192 ≈ 82m).
    /// </summary>
    public float Range { get; set; } = 2048f;

    /// <summary>
    /// Whether the light casts shadows (more realistic but more expensive).
    /// </summary>
    public bool CastShadows { get; set; } = true;

    /// <summary>
    /// Light cookie texture path (used as a projected beam texture).
    /// </summary>
    public string LightCookie { get; set; } = "materials/effects/lightcookies/flashlight.vtex";

    /// <summary>
    /// Beam size in front of the light (X/Y are the beam cross size, Z is beam depth).
    /// </summary>
    public float SizeX { get; set; } = 45f;

    /// <summary>
    /// Beam size Y.
    /// </summary>
    public float SizeY { get; set; } = 45f;

    /// <summary>
    /// Beam size Z (depth of the beam, small values make a flat beam).
    /// </summary>
    public float SizeZ { get; set; } = 0.03f;

    /// <summary>
    /// Beam softness X (edge blur).
    /// </summary>
    public float SoftX { get; set; } = 1.0f;

    /// <summary>
    /// Beam softness Y (edge blur).
    /// </summary>
    public float SoftY { get; set; } = 1.0f;

    /// <summary>
    /// Skirt factor (beam shape trailing edge).
    /// </summary>
    public float Skirt { get; set; } = 0.5f;

    /// <summary>
    /// Skirt near factor.
    /// </summary>
    public float SkirtNear { get; set; } = 1.0f;

    /// <summary>
    /// Forward distance (in units) from the player's eye to place the light origin.
    /// </summary>
    public float ForwardDistance { get; set; } = 54f;

    /// <summary>
    /// Eye height offset while standing (in units).
    /// </summary>
    public float StandEyeOffsetZ { get; set; } = 64f;

    /// <summary>
    /// Eye height offset while crouching (in units).
    /// </summary>
    public float CrouchEyeOffsetZ { get; set; } = 46f;

    /// <summary>
    /// Whether the light should only be visible to the owner (via transmit state).
    /// </summary>
    public bool OwnerOnly { get; set; } = true;
}
