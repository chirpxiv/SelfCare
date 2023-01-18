using System;
using Dalamud.Logging;
using ImGuiNET;

namespace SelfCare.Interface.Components {
	public static class TimeInput {
		public static bool Draw(string id, ref int val) {
			var result = false;

			var t = TimeSpan.FromMilliseconds(val);

			ImGui.PushItemWidth(ImGui.GetFontSize() * 2);

			var hr = t.Hours;
			result |= ImGui.InputInt($"h ##H_{id}", ref hr, 0);

			ImGui.SameLine();

			var min = t.Minutes;
			result |= ImGui.InputInt($"m ##M_{id}", ref min, 0);

			ImGui.SameLine();

			var sec = t.Seconds;
			result |= ImGui.InputInt($"s##S_{id}", ref sec, 0);

			ImGui.PopItemWidth();

			if (result)
				result &= !ImGui.IsItemActive();

			if (result)
				val = Math.Max(0, ((hr * 3600) + (min * 60) + sec) * 1000);

			return result;
		}
	}
}