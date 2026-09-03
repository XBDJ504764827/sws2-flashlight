using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;
using Sws2Flashlight.Configuration;

namespace Sws2Flashlight.Services;

/// <summary>
/// Wraps a dynamically created <c>light_spot</c> entity that acts as a player-owned flashlight.
/// The entity follows the owner's eye position and view angles.
/// </summary>
public sealed class FlashlightEntity : IDisposable
{
    /// <summary>True while the light entity is valid.</summary>
    public bool IsActive => _light is { IsValid: true } && !_disposed;

    private readonly ISwiftlyCore _core;
    private readonly FlashlightConfig _config;
    private readonly int _playerId;

    private CLightSpotEntity? _light;
    private bool _disposed;

    public FlashlightEntity(ISwiftlyCore core, FlashlightConfig config, int playerId)
    {
        _core = core;
        _config = config;
        _playerId = playerId;
    }

    /// <summary>
    /// Creates the light entity with the configured appearance and spawns it.
    /// </summary>
    public void Create()
    {
        if (_disposed)
        {
            return;
        }

        // Light owner (player) for placement
        var player = _core.PlayerManager.GetPlayer(_playerId);
        if (player == null || player.Pawn == null)
        {
            return;
        }

        try
        {
            _light = _core.EntitySystem.CreateEntity<CLightSpotEntity>();
            if (_light == null || !_light.IsValid)
            {
                _core.Logger.LogWarning("[sws2-flashlight] Failed to create light_spot entity for player {PlayerId}", _playerId);
                return;
            }

            ApplyAppearance();
            _light.DispatchSpawn();

            // Position light at the eye at spawn
            UpdateTransform();

            // Restrict visibility to owner if configured
            if (_config.Light.OwnerOnly)
            {
                _light.SetTransmitState(false);
                _light.SetTransmitState(true, _playerId);
            }
        }
        catch (Exception ex)
        {
            _core.Logger.LogError(ex, "[sws2-flashlight] Error creating flashlight entity for player {PlayerId}", _playerId);
            Dispose();
        }
    }

    /// <summary>
    /// Applies configured color / brightness / range / cone angles to the light component.
    /// </summary>
    private void ApplyAppearance()
    {
        if (_light == null)
        {
            return;
        }

        var component = _light.CLightComponent;
        if (component == null)
        {
            return;
        }

        // Parse color from hex string (#RRGGBB or #RRGGBBAA)
        var color = ParseColor(_config.Light.Color);

        component.Color = color;
        component.Brightness = Math.Clamp(_config.Light.Brightness, 0f, 32f);
        component.Range = Math.Clamp(_config.Light.Range, 0f, 32768f);
        component.Theta = Degree(_config.Light.Theta);
        component.Phi = Degree(_config.Light.Phi);
        component.Falloff = _config.Light.Falloff;
        component.CastShadows = _config.Light.CastShadows ? 1 : 0;

        // Notify network of changes
        component.ColorUpdated();
        component.BrightnessUpdated();
        component.RangeUpdated();
        component.ThetaUpdated();
        component.PhiUpdated();
        component.FalloffUpdated();
        component.CastShadowsUpdated();
    }

    /// <summary>
    /// Updates the light's position and direction to follow the owner's eye.
    /// </summary>
    public void UpdateTransform()
    {
        if (!IsActive)
        {
            return;
        }

        var player = _core.PlayerManager.GetPlayer(_playerId);
        var pawn = player?.PlayerPawn;
        if (pawn == null || !pawn.IsValid || player is { IsAlive: false })
        {
            return;
        }

        var eyePos = pawn.EyePosition;
        var eyeAngles = pawn.EyeAngles;

        if (eyePos.HasValue == false)
        {
            return;
        }

        // Place the light a bit in front of the eye to avoid the light being fully inside the player model.
        eyeAngles.ToDirectionVectors(out var forward, out _, out _);
        var origin = eyePos.Value + forward * 10f;

        _light!.Teleport(origin, eyeAngles, null);
    }

    /// <summary>
    /// Destroys the light entity.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            if (_light is { IsValid: true })
            {
                _light.Despawn();
            }
        }
        catch (Exception ex)
        {
            _core.Logger.LogDebug(ex, "[sws2-flashlight] Error despawn light entity for player {PlayerId}", _playerId);
        }
        finally
        {
            _light = null;
        }
    }

    private static Color ParseColor(string hex)
    {
        try
        {
            var value = hex.TrimStart('#');
            if (value.Length >= 6)
            {
                var r = byte.Parse(value.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = byte.Parse(value.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = byte.Parse(value.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                var a = value.Length >= 8 ? byte.Parse(value.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) : (byte)255;
                return new Color(r, g, b, a);
            }
        }
        catch
        {
            // ignore and fallback
        }

        return new Color(255, 255, 255, 255);
    }

    private static float Degree(float value) => value * MathF.PI / 180f;
}
