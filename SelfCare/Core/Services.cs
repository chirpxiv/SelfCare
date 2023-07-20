using Dalamud.Plugin;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;

namespace SelfCare.Core; 

internal class Services {
	// Dalamud plugin services

	internal static DalamudPluginInterface PluginApi = null!;

	[PluginService] internal static GameGui GameGui { get; set; } = null!;
	[PluginService] internal static ChatGui ChatGui { get; set; } = null!;
	[PluginService] internal static Framework Framework { get; set; } = null!;
	[PluginService] internal static Condition Condition { get; set; } = null!;
	[PluginService] internal static ClientState ClientState { get; set; } = null!;
	[PluginService] internal static CommandManager CommandManager { get; set; } = null!;

	// Service injection and creation

	public static void Init(DalamudPluginInterface api) {
		PluginApi = api;
		PluginApi.Create<Services>();
	}
}