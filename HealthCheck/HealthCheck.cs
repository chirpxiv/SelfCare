using Dalamud.Plugin;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;

using HealthCheck.Interface;
using HealthCheck.Extensions;

namespace HealthCheck {
	public sealed class HealthCheck : IDalamudPlugin {
		public string Name => "HealthCheck";
		public string CommandName = "/healthcheck";

		public string[] CommandAliases = { "/healthcheck", "/hchecks", "/hcheck" };

		public static WindowSystem Windows = new("HealthCheck");

		public static Configuration Config { get; internal set; } = null!;

		public HealthCheck(DalamudPluginInterface dalamud) {
			Services.Init(dalamud);

			Alerts.SoundAlert.Init();

			Configuration.LoadConfig();

			Services.Interface.UiBuilder.DisableGposeUiHide = true;
			Services.Interface.UiBuilder.Draw += Windows.Draw;

			Windows.AddWindow(new AlertWindow());
			Windows.AddWindow(new ConfigWindow());

			Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = $"Show the {Name} configuration window.\nAliases: {string.Join(", ", CommandAliases)}"
			});

			foreach (var alias in CommandAliases)
				Services.CommandManager.AddHandler(alias, new CommandInfo(OnCommand) { ShowInHelp = false });
		}

		public void Dispose() {
			Windows.RemoveAllWindows();

			Services.CommandManager.RemoveHandler(CommandName);
			foreach (var alias in CommandAliases)
				Services.CommandManager.RemoveHandler(alias);
		}

		private void OnCommand(string _, string arguments)
			=> Windows.GetWindow<ConfigWindow>()?.Show();

		internal static string GetVersion()
			=> typeof(HealthCheck).Assembly.GetName().Version!.ToString(fieldCount: 3);
	}
}