using System;

using Dalamud.Logging;
using Dalamud.Interface.Windowing;

using ImGuiNET;

using SelfCare.Interface.Windows.CfgTabs;

namespace SelfCare.Interface.Windows; 

public class ConfigWindow : Window {
	private readonly RemindersTab Reminders;

	public ConfigWindow() : base(
		"SelfCare"
	) {
		Reminders = new RemindersTab();
	}

	// Draw UI code
	
	public override void Draw() {
		if (ImGui.BeginTabBar("SelfCareConfigTabs")) {
			if (ImGui.BeginTabItem("Reminders"))
				DrawTab(Reminders);
			
			ImGui.EndTabBar();
		}
	}

	private void DrawTab<T>(T tab) where T : IConfigTab {
		try {
			tab.Draw();
		} catch (Exception err) {
			PluginLog.Error($"Error while drawing {typeof(T).Name}:\n{err}");
		} finally {
			ImGui.EndTabItem();	
		}
	}
}