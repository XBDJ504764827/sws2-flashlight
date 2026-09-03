using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.EntitySystem;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;
using Sws2Flashlight.Configuration;

namespace Sws2Flashlight.Services;

/// <summary>
/// Wraps a dynamically created <c>light_barn</c> entity that acts as a player-owned flashlight.
/// The entity follows the owner's eye position and view angles.
/// </summary>
public sealed class FlashlightEntity : IDisposable
{
    /// <summary>True while the light entity is valid.</summary>
    public bool IsActive => _light is { IsValid: true } && !_disposed;

    private readonly ISwiftlyCore _core;
    private readonly FlashlightConfig _config;
    private readonly int _playerId;

    private CBarnLight? _light;
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
            _light = _core.EntitySystem.CreateEntity<CBarnLight>();
            if (_light == null || !_light.IsValid)
            {
                _core.Logger.LogWarning("[sws2-flashlight] Failed to create light_barn entity for player {PlayerId}", _playerId);
                return;
            }

            ApplyAppearance();
            UpdateTransform();

            // Spawn with the light cookie keyvalue (beam texture)
            using var kv = new CEntityKeyValues();
            kv.SetString("lightcookie", _config.Light.LightCookie);
            _light.DispatchSpawn(kv);

            // Position again after spawn (transform is applied pre-spawn for the beam)
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
    /// Applies configured color / brightness / range / beam shape to the light entity.
    /// </summary>
    private void ApplyAppearance()
    {
        if (_light == null)
        {
            return;
        }

        // Enable the light - critical! Without this the light stays dormant.
        _light.Enabled = true;

        // Beam shape (X/Y are the beam cross size, Z is beam depth; small Z makes a flat beam)
        _light.SizeParams = new Vector(_config.Light.SizeX, _config.Light.SizeY, _config.Light.SizeZ);
        _light.SoftX = _config.Light.SoftX;
        _light.SoftY = _config.Light.SoftY;
        _light.Skirt = _config.Light.Skirt;
        _light.SkirtNear = _config.Light.SkirtNear;

        // Light properties
        _light.Color = ParseColor(_config.Light.Color);
        _light.ColorTemperature = Math.Clamp(_config.Light.ColorTemperature, 1000f, 12000f);
        _light.Brightness = Math.Clamp(_config.Light.Brightness, 0f, 32f);
        _light.Range = Math.Clamp(_config.Light.Range, 1f, 32768f);
        _light.CastShadows = _config.Light.CastShadows ? 1 : 0;

        // Direct light mode (3 = full dynamic light contribution)
        _light.DirectLight = 3;
        _light.BounceLight = 0;

        // Notify network of changes
        _light.EnabledUpdated();
        _light.SizeParamsUpdated();
        _light.SoftXUpdated();
        _light.SoftYUpdated();
        _light.SkirtUpdated();
        _light.SkirtNearUpdated();
        _light.ColorUpdated();
        _light.ColorTemperatureUpdated();
        _light.BrightnessUpdated();
        _light.RangeUpdated();
        _light.CastShadowsUpdated();
        _light.DirectLightUpdated();
        _light.BounceLightUpdated();
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

        var absOrigin = pawn.AbsOrigin;
        var vAngle = pawn.V_angle;
        if (absOrigin == null)
        {
            return;
        }

        var originVec = absOrigin.Value;

        // Eye height offset (crouch detection not available here; use standing offset)
        var eyeOffsetZ = _config.Light.StandEyeOffsetZ;

        // Compute forward vector on horizontal plane (yaw only) for origin offset,
        // then use the FULL pitch for the light rotation so the beam follows the view.
        var yawRad = vAngle.Y * (MathF.PI / 180f);
        var forward = new Vector(MathF.Cos(yawRad), MathF.Sin(yawRad), 0f);

        var origin = new Vector(
            originVec.X + forward.X * _config.Light.ForwardDistance,
            originVec.Y + forward.Y * _config.Light.ForwardDistance,
            originVec.Z + eyeOffsetZ);

        _light!.Teleport(origin, vAngle, null);
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
                return new Color(r, g, b);
            }
        }
        catch
        {
            // ignore and fallback
        }

        return new Color(255, 255, 255);
    }
}
