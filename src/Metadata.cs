using SwiftlyS2.Shared;

namespace SWS2Flashlight;

// WORKFLOW_VERSION is replaced by the release workflow with the GitVersion-computed version.
// Locally (and in PR builds) the plugin runs as version "Local".
[PluginMetadata(
    Id = "SWS2Flashlight",
#if WORKFLOW
    Version = "WORKFLOW_VERSION",
#else
    Version = "Local",
#endif
    Name = "SWS2 Flashlight",
    Author = "SWS2Flashlight Team",
    Description = "A flashlight plugin for SwiftlyS2: press F to toggle your flashlight.",
    Website = "https://github.com/XBDJ504764827/sws2-flashlight"
)]
public partial class SWS2Flashlight;
