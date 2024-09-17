using System.Collections.Generic;

using Dalamud.Plugin;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

using SelfCare.Interface;
using SelfCare.Extensions;

namespace SelfCare {
	public sealed class SelfCare : IDalamudPlugin {
		public string Name => "SelfCare";
		public string CommandName = "/selfcare";

		public static WindowSystem Windows = new("SelfCare");

		public static Configuration Config { get; internal set; } = null!;

		public SelfCare(IDalamudPluginInterface dalamud) {
			Services.Init(dalamud);

			Configuration.LoadConfig();

			Services.Interface.UiBuilder.DisableGposeUiHide = true;
			Services.Interface.UiBuilder.Draw += Windows.Draw;

			Services.Interface.UiBuilder.OpenConfigUi += ToggleConfig;

			var alertWindow = new AlertWindow();
			Windows.AddWindow(alertWindow);

			var cfgWindow = new ConfigWindow();
			Windows.AddWindow(cfgWindow);
			if (Config._IsFirstTime_)
				cfgWindow.Show();

			Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = $"Show the {Name} configuration window."
			});

			if (Config.PrintToChat) {
				var ct = alertWindow.Alerts.FindAll(a => a.Enabled).Count;

				var msg = new SeString(new List<Payload>() {
					new TextPayload($"[{Name}] "),
					new TextPayload(string.Format(
						"You have {0} alert{1} enabled. Type ",
						ct, ct == 1 ? "" : "s"
					)),
					new UIForegroundPayload(500),
					new TextPayload(CommandName),
					new UIForegroundPayload(0),
					new TextPayload(" to configure them.")
				});
				
				Services.ChatGui.Print(msg);
			}
		}

		public void Dispose() {
			Windows.RemoveAllWindows();

			Config.Save();

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