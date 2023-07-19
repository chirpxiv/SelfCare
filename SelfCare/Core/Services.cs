using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.Command;

namespace SelfCare.Core; 

internal class Services {
	// Dalamud plugin services

	internal static DalamudPluginInterface PluginApi = null!;

	[PluginService] internal static CommandManager CommandManager { get; set; } = null!;
	
	// Service injection

	public static void Init(DalamudPluginInterface api) {
		PluginApi = api;
		PluginApi.Create<Services>();
	}
}