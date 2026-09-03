using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Plugins;
using Sws2Flashlight.Configuration;
using Sws2Flashlight.Services;

namespace Sws2Flashlight;

/// <summary>
/// Main plugin entry point. Plugin metadata is defined in <c>Metadata.cs</c>.
/// </summary>
public partial class Sws2Flashlight : BasePlugin
{
    /// <summary>
    /// Current configuration, hot-reload aware.
    /// </summary>
    private FlashlightConfig _config = new();

    /// <summary>
    /// Flashlight manager service (owns light entities and their lifecycle).
    /// </summary>
    private FlashlightManager _manager = null!;

    /// <summary>
    /// DI provider.
    /// </summary>
    private ServiceProvider _provider = null!;

    public Sws2Flashlight(ISwiftlyCore core) : base(core)
    {
    }

    public override void Load(bool hotReload)
    {
        // 1. Initialize configuration (auto-generated from template on first run)
        Core.Configuration
            .InitializeWithTemplate("config.jsonc", "config.template.jsonc")
            .Configure(builder =>
            {
                builder.AddJsonFile("config.jsonc", optional: false, reloadOnChange: true);
            });

        // 2. Bind config through DI + IOptionsMonitor (hot reload)
        var services = new ServiceCollection();
        services.AddSwiftly(Core)
            .AddOptionsWithValidateOnStart<FlashlightConfig>()
            .BindConfiguration("sws2-flashlight");

        _provider = services.BuildServiceProvider();
        var options = _provider.GetRequiredService<IOptionsMonitor<FlashlightConfig>>();

        // Initial load
        _config = options.CurrentValue;

        // Hot reload support
        options.OnChange(newConfig =>
        {
            _config = newConfig;
            Core.Logger.LogInformation("[sws2-flashlight] Configuration reloaded (debug: {Debug})", _config.Debug);
            _manager?.ApplyConfig(_config);
        });

        // 3. Setup flashlight manager
        _manager = new FlashlightManager(Core, _config);
        _manager.Start();

        // 4. Register events
        Core.Event.OnClientKeyStateChanged += OnClientKeyStateChanged;
        Core.Event.OnClientDisconnected += OnClientDisconnected;
        Core.Event.OnMapUnload += OnMapUnload;

        // 5. Register game events: player death, team switch
        Core.GameEvent.HookPost<EventPlayerDeath>(OnPlayerDeath);
        Core.GameEvent.HookPost<EventPlayerTeam>(OnPlayerTeam);

        Core.Logger.LogInformation("[sws2-flashlight] Plugin loaded (hotReload: {HotLoad})", hotReload);
    }

    public override void Unload()
    {
        // Unsubscribe events
        Core.Event.OnClientKeyStateChanged -= OnClientKeyStateChanged;
        Core.Event.OnClientDisconnected -= OnClientDisconnected;
        Core.Event.OnMapUnload -= OnMapUnload;

        // Unhook game events
        Core.GameEvent.UnhookPost<EventPlayerDeath>();
        Core.GameEvent.UnhookPost<EventPlayerTeam>();

        // Destroy all flashlights and stop the manager
        _manager?.Stop();
        _manager?.Dispose();

        _provider?.Dispose();
    }
}
