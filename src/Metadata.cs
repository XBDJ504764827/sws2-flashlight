using SwiftlyS2.Shared;

namespace Sws2Flashlight;

// WORKFLOW_VERSION is replaced by the release workflow with the GitVersion-computed version.
// Locally (and in PR builds) the plugin runs as version "Local".
[PluginMetadata(
    Id = "sws2-flashlight",
#if WORKFLOW
    Version = "WORKFLOW_VERSION",
#else
    Version = "Local",
#endif
    Name = "sws2-flashlight",
    Author = "XBDJ504764827",
    Description = "A flashlight plugin for SwiftlyS2: press F to toggle your flashlight.",
    Website = "https://github.com/XBDJ504764827/sws2-flashlight"
)]
public partial class Sws2Flashlight;
