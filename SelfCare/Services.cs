using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Conditions;

namespace SelfCare {
	internal class Services {
		[PluginService] internal static DalamudPluginInterface Interface { get; set; } = null!;
		[PluginService] internal static CommandManager CommandManager { get; set; } = null!;
		[PluginService] internal static SigScanner SigScanner { get; set; } = null!;
		[PluginService] internal static Condition Condition { get; set; } = null!;
		[PluginService] internal static ChatGui ChatGui { get; set; } = null!;

		internal static void Init(DalamudPluginInterface dalamud) => dalamud.Create<Services>();
	}
}