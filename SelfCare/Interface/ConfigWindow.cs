using System;
using System.Numerics;

using Dalamud.Bindings.ImGui;

using Dalamud.Interface.Windowing;

using SelfCare.Alerts;
using SelfCare.Extensions;
using SelfCare.Interface.Components;

namespace SelfCare.Interface {
	public class ConfigWindow : Window {
		private SoundEffect[] SoundEffectValues = Enum.GetValues<SoundEffect>();
		private ImGuiMouseButton[] ButtonValues = {
			ImGuiMouseButton.Left,
			ImGuiMouseButton.Middle,
			ImGuiMouseButton.Right,
		};

		private Configuration Config {
			get => SelfCare.Config;
			set => SelfCare.Config = value;
		}

		// Constructors

		public ConfigWindow() : base(
			"SelfCare"
		) { }

		// Methods

		public override void Draw() {
			if (ImGui.BeginTabBar("SelfCare Tabs")) {
				DrawTab("General", GeneralTab);
				DrawTab("Reminders", RemindersTab);
				ImGui.EndTabBar();
			}
		}

		private void DrawTab(string text, Action draw) {
			if (ImGui.BeginTabItem(text)) {
				draw.Invoke();
				ImGui.EndTabItem();
			}
		}

		// "General" config tab

		private unsafe void GeneralTab() {
			ImGui.Spacing();

			ImGui.Text("Alerts");
			ImGui.Separator();
			ImGui.Spacing();

			// Dismissal

			ImGui.Text("Dismiss method:");

			ImGui.BeginGroup();

			DrawDismissMethod("Click to dismiss", DismissMode.OnClick);
			DrawDismissMethod("Dismiss after x time", DismissMode.OnTimer);

			ImGui.EndGroup();

			if (Config.DismissMode == DismissMode.OnClick) {
				ImGui.PushItemWidth(ImGui.GetItemRectSize().X);

				if (ImGui.BeginCombo("Button", $"{Config.DismissButton} Click")) {
					foreach (var val in ButtonValues) {
						if (ImGui.Selectable($"{val} Click", Config.DismissButton == val))
							Config.DismissButton = val;
					}
					ImGui.EndCombo();
				}

				ImGui.PopItemWidth();
			} else if (Config.DismissMode == DismissMode.OnTimer) {
				var time = (int)Config.DismissTimer;
				if (TimeInput.Draw("DismissTimer", ref time))
					Config.DismissTimer = (uint)time;
			}

			ImGuiExtensions.Spacing(2);

			// Sound Effect

			var sfxCurrent = SoundAlert.GetName(Config.SoundEffect);
			if (ImGui.BeginCombo("Sound Effect", sfxCurrent)) {
				foreach (var val in SoundEffectValues) {
					if (ImGui.Selectable(SoundAlert.GetName(val), val == Config.SoundEffect)) {
						Config.SoundEffect = val;
						SoundAlert.PlayCurrent();
					}
				}
				ImGui.EndCombo();
			}

			ImGuiExtensions.Spacing(2);

			// Disable in combat / cutscene

			ImGui.Checkbox("Disabled in cutscenes", ref Config.DisableInCutscene);

			ImGui.SameLine();

			ImGui.Checkbox("Disabled in combat", ref Config.DisableInCombat);

			// Print to chat

			ImGui.Spacing();

			ImGui.Checkbox("Print reminders to chat", ref Config.PrintToChat);

			ImGuiExtensions.Spacing(2);

			// Style

			ImGui.Text("Window Style");
			ImGui.Separator();
			ImGui.Spacing();

			DrawUiColor("Override font color", "##FontCol", ref Config.FontColor);

			ImGui.Spacing();

			DrawUiColor("Override background color", "##BgCol", ref Config.BgColor);
		}

		private void DrawDismissMethod(string label, DismissMode mode) {
			if (ImGui.RadioButton(label, Config.DismissMode == mode))
				Config.DismissMode = mode;
		}

		private unsafe bool DrawUiColor(string activeText, string editText, ref UiColor data) {
			var result = false;

			ImGui.Checkbox(activeText, ref data.Active);

			if (data.Color == null) {
				result = true;
				data.GetColorFromStyle();
			}

			ImGui.BeginDisabled(!data.Active);

			var col = (Vector4)data.Color!;
			if (ImGui.ColorEdit4(editText, ref col)) {
				result = true;
				data.Color = col;
			}

			ImGui.SameLine();

			if (ImGui.Button($"Reset##{editText}_Reset")) {
				result = true;
				data.GetColorFromStyle();
			}
			
			ImGui.EndDisabled();

			return result;
		}

		// "Reminders" Tab

		private void RemindersTab() {
			ImGui.Spacing();
			DrawReminder("Hydration", ref Config.Hydrate);
			ImGuiExtensions.Spacing(2);
			DrawReminder("Posture Check", ref Config.Posture);
            ImGuiExtensions.Spacing(2);
            DrawReminder("Break Reminder", ref Config.Break);
        }

		private void DrawReminder(string label, ref Alert alert) {
			ImGui.Text(label);
			ImGui.Separator();
			ImGui.Spacing();

			ImGui.Checkbox($"Enabled##Toggle_{label}", ref alert.Enabled);

			ImGui.Spacing();

			ImGui.Text("Remind me every: ");
			ImGui.SameLine();

			var interval = alert.Interval;
			if (TimeInput.Draw($"##Time_{label}", ref interval, 1000))
				alert.Interval = interval;

			ImGui.Spacing();

			ImGui.InputTextWithHint($"Text##Text_{label}", "Remember to...", ref alert.Text, 200);
		}

		// Save on close

		public override void OnClose() {
			base.OnClose();
			Config.Save();
		}
	}
}