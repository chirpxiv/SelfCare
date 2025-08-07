using Dalamud.Bindings.ImGui;

namespace SelfCare.Extensions {
	public static class ImGuiExtensions {
		public static void Spacing(int ct = 1) {
			for (var i = 0; i < ct; i++) ImGui.Spacing();
		}
	}
}