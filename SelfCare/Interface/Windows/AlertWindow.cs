using Dalamud.Interface.Windowing;

using ImGuiNET;

namespace SelfCare.Interface.Windows; 

public class AlertWindow : Window {
	public AlertWindow() : base(
		"SelfCare Alert",
		ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDecoration
	) {
		RespectCloseHotkey = false;
	}

	public override void PreDraw() { }
	
	public override void Draw() { }
}