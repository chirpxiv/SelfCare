using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace SelfCare {
	internal class Services {
		[PluginService] internal static IDalamudPluginInterface Interface { get; set; } = null!;
		[PluginService] internal static ICommandManager CommandManager { get; set; } = null!;
		[PluginService] internal static IClientState ClientState { get; set; } = null!;
		[PluginService] internal static ISigScanner SigScanner { get; set; } = null!;
		[PluginService] internal static ICondition Condition { get; set; } = null!;
		[PluginService] internal static IChatGui ChatGui { get; set; } = null!;
		[PluginService] internal static IPluginLog Log { get; set; } = null!;

		internal static void Init(IDalamudPluginInterface dalamud) => dalamud.Create<Services>();
	}
}
