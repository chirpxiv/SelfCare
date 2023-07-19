using Dalamud.Plugin;

using SelfCare.Core;
using SelfCare.Interface;

namespace SelfCare; 

public sealed class SelfCare : IDalamudPlugin {
	// Plugin info
	
	public string Name { get; init; } = "SelfCare";

	// Framework
	
	public readonly PluginUi Interface;

	public PluginConfig PluginConfig;

	// Constructor called on plugin load
	
	public SelfCare(DalamudPluginInterface api) {
		Services.Init(api);
		
		Interface = new PluginUi();

		PluginConfig = PluginConfig.Load();
	}

	// Disposal

	public void Dispose() {
		PluginConfig.Save();
	}
}