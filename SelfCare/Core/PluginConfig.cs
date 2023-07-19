using System;

using Dalamud.Logging;
using Dalamud.Configuration;

using SelfCare.Core.Legacy;

namespace SelfCare.Core;

public class PluginConfig : IPluginConfiguration {
	public int Version { get; set; } = 1;
	
	// Loading & saving methods

	public static PluginConfig Load() {
		PluginConfig? cfg = null;

		try {
			var cfgBase = Services.PluginApi.GetPluginConfig();
			
			PluginLog.Information(cfgBase is null ? "No config found, creating new file" : $"Loading configuration (Version {cfgBase.Version})");
			
			cfg = cfgBase?.Version switch {
				null => new PluginConfig(), // doesn't exist, create a new one
				0 => ConfigV0.Upgrade(cfgBase), // legacy config from v0.1
				_ => cfgBase as PluginConfig // try to load as current version
			};
		} catch (Exception err) {
			// config failed to load - might be best to implement some extra handling for this later,
			// such as displaying an alert to the user and/or backing up the previous file.
			PluginLog.Error($"Failed to load configuration:\n{err}");
		} finally {
			// fallback to a new one if loading fails
			cfg ??= new PluginConfig();
		}

		return cfg;
	}

	public void Save() => Services.PluginApi.SavePluginConfig(this);
}