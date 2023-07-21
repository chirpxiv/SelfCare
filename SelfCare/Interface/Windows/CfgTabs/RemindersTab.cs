using System.Numerics;

using ImGuiNET;

using SelfCare.Alerts;

namespace SelfCare.Interface.Windows.CfgTabs; 

public class RemindersTab : IConfigTab {
	private Reminder? Selected;
	
	public void Draw() {
		DrawList();

		ImGui.SameLine();

		DrawMainRegion();
	}
	
	// Reminder list

	private void DrawList() {
		ImGui.BeginGroup();
		
		var listSize = new Vector2(ImGui.GetFontSize() * 10, -1);
		if (ImGui.BeginChildFrame(0xCF4, listSize)) {
			SelfCare.Config.Reminders.ForEach(DrawListItem);
			
			ImGui.EndChildFrame();
		}

		ImGui.EndGroup();
	}

	private void DrawListItem(Reminder item) {
		var isDisabled = !item.Enabled;
		if (isDisabled)
			ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled));
		
		var isSelected = item == Selected;
		if (ImGui.Selectable(item.Name, isSelected))
			Selected = isSelected ? null : item;

		if (isDisabled)
			ImGui.PopStyleColor();
	}
	
	// Draw selected

	private void DrawMainRegion() {
		ImGui.BeginGroup();
		
		if (Selected is null) {
			ImGui.Text("No reminder currently selected.");
			ImGui.Text("Choose one from the list on the right by clicking on its name.");
		} else {
			DrawSelected(Selected);
		}
		
		ImGui.EndGroup();
	}

	private void DrawSelected(Reminder item) {
		// TODO
	}
}