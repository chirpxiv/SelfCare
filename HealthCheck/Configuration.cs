using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Logging;
using Dalamud.Interface;
using Dalamud.Configuration;

using HealthCheck.Alerts;
using HealthCheck.Extensions;
using HealthCheck.Interface;

namespace HealthCheck {
	[Serializable]
	public class Configuration : IPluginConfiguration {
		public const int _ConfigVer = 0;

		public int Version { get; set; } = _ConfigVer;
		public string PluginVersion { get; set; } = HealthCheck.GetVersion();

		public bool IsFirstTime { get; set; } = true;

		// General

		public SoundEffect SoundEffect { get; set; } = SoundEffect.Se9;

		public DismissMode DismissMode { get; set; } = DismissMode.OnClick;
		public ImGuiMouseButton DismissButton { get; set; } = ImGuiMouseButton.Right;
		public bool DismissButtonDouble { get; set; } = false;

		public UiColor BgColor { get; set; } = new(ImGuiCol.WindowBg);
		public UiColor FontColor { get; set; } = new(ImGuiCol.Text);

		// Alerts

		public Alert Posture = new(FontAwesomeIcon.Chair, "Remember to check your posture!");
		public Alert Hydrate = new(FontAwesomeIcon.GlassWhiskey, "Remember to hydrate!");

		// Methods

		public void Init() {
			if (Version != _ConfigVer)
				Upgrade();

			var curVer = HealthCheck.GetVersion();
			if (PluginVersion != curVer) {
				// TODO: Changelog?
				PluginVersion = curVer;
			}

			if (IsFirstTime) {
				IsFirstTime = false;
				HealthCheck.Windows.GetWindow<ConfigWindow>()?.Show();
			}
		}

		public void Save() => Services.Interface.SavePluginConfig(this);

		private void Upgrade() {
			PluginLog.Warning(string.Format(
				"Upgrading config version from {0} to {1}.\nThis is nothing to worry about, but some settings may change or reset!",
				Version, _ConfigVer
			));

			Version = _ConfigVer;
		}

		public static void LoadConfig() {
			try {
				HealthCheck.Config = Services.Interface.GetPluginConfig() as Configuration ?? new();
			} catch (Exception e) {
				PluginLog.Error("Failed to load config. Settings have been reset.", e);
				HealthCheck.Config = new();
			}
			HealthCheck.Config.Init();
		}
	}

	public class UiColor {
		internal ImGuiCol ImGuiCol;

		public bool Active { get; set; } = false;
		public Vector4? Color { get; set; }

		public UiColor(ImGuiCol col) {
			ImGuiCol = col;
		}

		public void Draw(Action inner) {
			Push();
			inner.Invoke();
			Pop();
		}

		internal void Push() { if (Active) ImGui.PushStyleColor(ImGuiCol, ToU32()); }
		internal void Pop() { if (Active) ImGui.PopStyleColor(); }

		private uint ToU32() => ImGui.ColorConvertFloat4ToU32((Vector4)Color!);

		internal unsafe void GetColorFromStyle() {
			var ptr = ImGui.GetStyleColorVec4(ImGuiCol);
			if (ptr != null) Color = *ptr;
		}
	}

	public enum DismissMode {
		OnClick,
		OnTimer
	}
}