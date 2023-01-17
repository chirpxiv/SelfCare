using System.Collections.Generic;

using ImGuiNET;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Game.ClientState.Conditions;

using SelfCare.Alerts;
using SelfCare.Extensions;

namespace SelfCare.Interface {
	public class AlertWindow : Window {
		private List<Alert> Alerts = new() { SelfCare.Config.Posture, SelfCare.Config.Hydrate };

		private bool IsConfiguring = false;

		public AlertWindow() : base(
			"SelfCare Alert",
			ImGuiWindowFlags.AlwaysAutoResize ^ ImGuiWindowFlags.NoDecoration
		) {
			RespectCloseHotkey = false;
		}

		public override void PreOpenCheck() {
			IsConfiguring = SelfCare.Windows.GetWindow<ConfigWindow>()?.IsOpen ?? false;
			if (IsConfiguring && !IsOpen) IsOpen = true;

			var canOpen = true;
			canOpen &= !(SelfCare.Config.DisableInCutscene && (
				Services.Condition[ConditionFlag.WatchingCutscene]
				|| Services.Condition[ConditionFlag.OccupiedInCutSceneEvent]
				|| Services.Condition[ConditionFlag.OccupiedInQuestEvent]
			));
			canOpen &= !(SelfCare.Config.DisableInCombat && Services.Condition[ConditionFlag.InCombat]);

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
			SelfCare.Config.BgColor.Push();
		}

		public override void Draw() {
			var active = false;

			var isClicked = ImGui.IsWindowHovered() && (SelfCare.Config.DismissButtonDouble
				? ImGui.IsMouseDoubleClicked(SelfCare.Config.DismissButton)
				: ImGui.IsMouseClicked(SelfCare.Config.DismissButton));
			var shouldClose = SelfCare.Config.DismissMode == DismissMode.OnClick && isClicked;

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

					SelfCare.Config.FontColor.Draw(() => DrawReminder(alert.Icon, alert.Text));

					if (i < Alerts.Count - 1)
						ImGui.Spacing();
				}
			}

			if (!active)
				IsOpen = false;
		}

		public override void PostDraw() {
			base.PostDraw();
			SelfCare.Config.BgColor.Pop();
		}

		private static void DrawReminder(FontAwesomeIcon icon, string text) {
			var cfg = SelfCare.Config;

			ImGui.PushFont(UiBuilder.IconFont);
			ImGui.Text(icon.ToIconString());
			ImGui.PopFont();

			ImGui.SameLine();

			ImGui.Text(text);

			//cfg.FontColor.Draw(() => ImGui.Text(text));
		}
	}
}