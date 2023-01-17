using System.Numerics;
using System.Collections.Generic;

using ImGuiNET;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;

using HealthCheck.Alerts;
using HealthCheck.Alerts;
using HealthCheck.Extensions;

namespace HealthCheck.Interface {
	public class AlertWindow : Window {
		private List<Alert> Alerts = new() { HealthCheck.Config.Posture, HealthCheck.Config.Hydrate };

		private bool IsConfiguring = false;

		public AlertWindow() : base(
			"HealthCheck Alert",
			ImGuiWindowFlags.AlwaysAutoResize ^ ImGuiWindowFlags.NoDecoration
		) { }

		public override void PreOpenCheck() {
			IsConfiguring = HealthCheck.Windows.GetWindow<ConfigWindow>()?.IsOpen ?? false;
			if (IsConfiguring && !IsOpen) IsOpen = true;

			var canOpen = true;
			if (!IsOpen && canOpen) {
				foreach (var alert in Alerts) {
					if (alert.HasTimerElapsed) {
						IsOpen = true;
						SoundAlert.PlayCurrent();
						break;
					}
				}
			}
		}

		public override void PreDraw() {
			base.PreDraw();
			HealthCheck.Config.BgColor.Push();
		}

		public override void Draw() {
			var active = false;

			var isClicked = ImGui.IsWindowHovered() && (HealthCheck.Config.DismissButtonDouble
				? ImGui.IsMouseDoubleClicked(HealthCheck.Config.DismissButton)
				: ImGui.IsMouseClicked(HealthCheck.Config.DismissButton));
			var shouldClose = HealthCheck.Config.DismissMode == DismissMode.OnClick && isClicked;

			for (var i = 0; i < Alerts.Count; i++) {
				var alert = Alerts[i];

				if (alert.HasTimerElapsed)
					alert.IsVisible = true;

				if (shouldClose) {
					alert.IsVisible = false;
					alert.RestartTimer();
				}

				if (alert.IsVisible || IsConfiguring) {
					active = true;

					//DrawReminder(alert.Icon, alert.Text);

					HealthCheck.Config.FontColor.Draw(() => DrawReminder(alert.Icon, alert.Text));

					if (i < Alerts.Count - 1)
						ImGui.Spacing();
				}
			}

			if (!active)
				IsOpen = false;
		}

		public override void PostDraw() {
			base.PostDraw();
			HealthCheck.Config.BgColor.Pop();
		}

		private static void DrawReminder(FontAwesomeIcon icon, string text) {
			var cfg = HealthCheck.Config;

			ImGui.PushFont(UiBuilder.IconFont);
			ImGui.Text(icon.ToIconString());
			ImGui.PopFont();

			ImGui.SameLine();

			ImGui.Text(text);

			//cfg.FontColor.Draw(() => ImGui.Text(text));
		}
	}
}