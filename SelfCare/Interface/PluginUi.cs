using Dalamud.Interface.Windowing;

namespace SelfCare.Interface; 

public class PluginUi {
	// Window manager
	
	private readonly WindowSystem Windows;

	// PluginUi constructor
	
	public PluginUi() {
		Windows = new WindowSystem("SelfCare");
	}
}