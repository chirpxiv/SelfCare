using Dalamud.Plugin;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;

using SelfCare.Interface;
using SelfCare.Extensions;

namespace SelfCare {
	public sealed class SelfCare : IDalamudPlugin {
		public string Name => "SelfCare";
		public string CommandName = "/selfcare";

		public static WindowSystem Windows = new("SelfCare");

		public static Configuration Config { get; internal set; } = null!;

		public SelfCare(DalamudPluginInterface dalamud) {
			Services.Init(dalamud);

			Alerts.SoundAlert.Init();

			Configuration.LoadConfig();

			Services.Interface.UiBuilder.DisableGposeUiHide = true;
			Services.Interface.UiBuilder.Draw += Windows.Draw;

			Services.Interface.UiBuilder.OpenConfigUi += ToggleConfig;

			Windows.AddWindow(new AlertWindow());
			Windows.AddWindow(new ConfigWindow());

			Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = $"Show the {Name} configuration window."
			});
		}

		public void Dispose() {
			Windows.RemoveAllWindows();

			Services.CommandManager.RemoveHandler(CommandName);
		}

		private void OnCommand(string _, string arguments)
			=> ToggleConfig();

		private void ToggleConfig()
			=> Windows.GetWindow<ConfigWindow>()?.Toggle();

		internal static string GetVersion()
			=> typeof(SelfCare).Assembly.GetName().Version!.ToString(fieldCount: 3);
	}
}