using System;

using Dalamud.Logging;
using Dalamud.Plugin;

using SelfCare.Core;

namespace SelfCare;

public sealed class SelfCare : IDalamudPlugin {
	// Plugin info
	
	public string Name { get; init; } = "SelfCare";

	internal static PluginConfig Config = null!;

	// Constructor called on plugin load

	public SelfCare(DalamudPluginInterface api) {
		Services.Create(api);
		
		Config = PluginConfig.Load();

		Services.Init();
	}

	// Disposal

	public void Dispose() {
		try {
			Config.Save();
		} catch (Exception err) {
			PluginLog.Error($"Configuration failed to save:\n{err}");
		}

		Services.Dispose();
	}
}