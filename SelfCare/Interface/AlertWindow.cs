using System;
using System.Collections.Generic;

using ImGuiNET;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Game.ClientState.Conditions;

using SelfCare.Alerts;
using SelfCare.Extensions;

namespace SelfCare.Interface {
	public class AlertWindow : Window {
		internal List<Alert> Alerts = new() { SelfCare.Config.Hydrate, SelfCare.Config.Posture }; // TODO: Streamline this

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
			// Disable in cutscene
			canOpen &= !(SelfCare.Config.DisableInCutscene && (
				Services.Condition[ConditionFlag.WatchingCutscene]
				|| Services.Condition[ConditionFlag.OccupiedInCutSceneEvent]
				|| Services.Condition[ConditionFlag.OccupiedInQuestEvent]
			));
			// Disable in combat
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

			var shouldClose = false;
			if (SelfCare.Config.DismissMode == DismissMode.OnClick)
				shouldClose = ImGui.IsWindowHovered() && ImGui.IsMouseClicked(SelfCare.Config.DismissButton);

			for (var i = 0; i < Alerts.Count; i++) {
				var alert = Alerts[i];
				if (!alert.Enabled) continue;

				var visible = alert.IsVisible || IsConfiguring;

				if (!IsConfiguring) {
					if (alert.HasTimerElapsed) {
						visible = true;
						alert.Trigger();
					} else if (!visible) continue;

					if (SelfCare.Config.DismissMode == DismissMode.OnTimer)
						shouldClose = (DateTime.Now - alert.VisibleSince).TotalMilliseconds >= SelfCare.Config.DismissTimer;

					if (shouldClose) {
						alert.IsVisible = false;
						alert.RestartTimer();
					}
				}

				if (visible) {
					if (active)
						ImGui.Spacing();

					active = true;

					SelfCare.Config.FontColor.Draw(() => DrawReminder(alert.Icon, alert.Text));
				}
			}

			if (!active) {
				if (IsConfiguring)
					ImGui.Text("Reminders will show up here when enabled.");
				else
					IsOpen = false;
			}
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
		}
	}
}