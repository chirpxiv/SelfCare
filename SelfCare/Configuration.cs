using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Logging;
using Dalamud.Interface;
using Dalamud.Configuration;

using SelfCare.Alerts;

namespace SelfCare {
	[Serializable]
	public class Configuration : IPluginConfiguration {
		public const int _ConfigVer = 0;

		public int Version { get; set; } = _ConfigVer;
		public string PluginVersion = SelfCare.GetVersion();

		internal bool _IsFirstTime_;
		public bool IsFirstTime = true;

		// General

		public SoundEffect SoundEffect = SoundEffect.Se9;

		public DismissMode DismissMode = DismissMode.OnClick;
		public ImGuiMouseButton DismissButton = ImGuiMouseButton.Right;
		public uint DismissTimer = 10 * 1000; // Default: 10s

		public bool DisableInCombat = true;
		public bool DisableInCutscene = true;

		public bool PrintToChat = true;

		public UiColor BgColor = new(ImGuiCol.WindowBg);
		public UiColor FontColor = new(ImGuiCol.Text);

		// Alerts

		public Alert Hydrate = new(FontAwesomeIcon.GlassWhiskey, "Remember to hydrate!", 60 * 60 * 1000); // Default: 1h
		public Alert Posture = new(FontAwesomeIcon.Chair, "Remember to check your posture!", 30 * 60 * 1000); // Default: 30m

		// Methods

		public void Init() {
			if (Version != _ConfigVer)
				Upgrade();

			var curVer = SelfCare.GetVersion();
			if (PluginVersion != curVer) {
				// TODO: Changelog?
				PluginVersion = curVer;
			}

			PluginLog.Information($"Init {IsFirstTime}");

			_IsFirstTime_ = IsFirstTime;
			if (IsFirstTime)
				IsFirstTime = false;
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
				SelfCare.Config = Services.Interface.GetPluginConfig() as Configuration ?? new();
			} catch (Exception e) {
				PluginLog.Error("Failed to load config. Settings have been reset.", e);
				SelfCare.Config = new();
			}
			SelfCare.Config.Init();
		}
	}

	public class UiColor {
		internal ImGuiCol ImGuiCol;

		public bool Active = false;
		public Vector4? Color = null;

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