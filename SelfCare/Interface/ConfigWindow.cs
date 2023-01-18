using System;
using System.Numerics;

using ImGuiNET;

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
				DrawTab("Popup", PopupTab);
				ImGui.EndTabBar();
			}
		}

		private void DrawTab(string text, Action draw) {
			if (ImGui.BeginTabItem(text)) {
				draw.Invoke();
				ImGui.EndTabItem();
			}
		}

		// "Popup" config tab

		private unsafe void PopupTab() {
			ImGui.Spacing();

			ImGui.Text("Alerts");
			ImGui.Separator();
			ImGui.Spacing();

			// Dismissal

			ImGui.Text("Dismiss Method:");

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

			var noCutscene = Config.DisableInCutscene;
			if (ImGui.Checkbox("Disabled in Cutscene", ref noCutscene))
				Config.DisableInCutscene = noCutscene;

			ImGui.SameLine();

			var noCombat = Config.DisableInCombat;
			if (ImGui.Checkbox("Disabled in Combat", ref noCombat))
				Config.DisableInCombat = noCombat;

			ImGuiExtensions.Spacing(2);

			// Style

			ImGui.Text("Window Style");
			ImGui.Separator();
			ImGui.Spacing();

			var fontCol = Config.FontColor;
			if (DrawUiColor("Override font color", "##FontCol", ref fontCol))
				Config.FontColor = fontCol;

			ImGui.Spacing();

			var bgCol = Config.BgColor;
			if (DrawUiColor("Override background color", "##BgCol", ref bgCol))
				Config.BgColor = bgCol;
		}

		private void DrawDismissMethod(string label, DismissMode mode) {
			if (ImGui.RadioButton(label, Config.DismissMode == mode))
				Config.DismissMode = mode;
		}

		private unsafe bool DrawUiColor(string activeText, string editText, ref UiColor data) {
			var result = false;

			var active = data.Active;
			if (ImGui.Checkbox(activeText, ref active))
				data.Active = active;

			if (data.Color == null) {
				result = true;
				data.GetColorFromStyle();
			}

			ImGui.BeginDisabled(!active);

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

		// Save on close

		public override void OnClose() {
			base.OnClose();
			Config.Save();
		}
	}
}