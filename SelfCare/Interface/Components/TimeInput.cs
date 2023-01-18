using System;

using ImGuiNET;

namespace SelfCare.Interface.Components {
	public static class TimeInput {
		public static bool Draw(string id, ref int val, int minimum = 5000) {
			var result = false;

			var t = TimeSpan.FromMilliseconds(val);

			ImGui.PushItemWidth(ImGui.GetFontSize() * 2);

			var hours = (int)t.TotalHours;
			if (ImGui.InputInt($"h ##H_{id}", ref hours, 0))
				result = hours < 168 && !ImGui.IsItemActive();

			ImGui.SameLine();

			var minutes = t.Minutes;
			if (ImGui.InputInt($"m ##M_{id}", ref minutes, 0))
				result = minutes < 1440 && !ImGui.IsItemActive();

			ImGui.SameLine();

			var seconds = t.Seconds;
			if (ImGui.InputInt($"s##S_{id}", ref seconds, 0))
				result = seconds < 3600 && !ImGui.IsItemActive();

			ImGui.PopItemWidth();

			if (result)
				val = Math.Max(minimum, ((hours * 3600) + (minutes * 60) + seconds) * 1000);

			return result;
		}
	}
}