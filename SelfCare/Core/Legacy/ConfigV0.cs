using Dalamud.Configuration;
using Dalamud.Logging;

namespace SelfCare.Core.Legacy; 

public class ConfigV0 : IPluginConfiguration {
	// Version info

	public int Version { get; set; } = 0;
	
	// Migration

	public static PluginConfig Upgrade(IPluginConfiguration cfgBase) {
		var result = new PluginConfig();

		PluginLog.Information("Upgrading configuration from version 0 (v0.1.0)");
		
		// TODO
		
		return result;
	}
}