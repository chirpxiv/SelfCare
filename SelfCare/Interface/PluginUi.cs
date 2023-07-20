using Dalamud.Interface.Windowing;

using SelfCare.Core;
using SelfCare.Interface.Windows;

namespace SelfCare.Interface; 

public class PluginUi : ServiceBase {
	// Window manager
	
	private readonly WindowSystem Windows;

	internal readonly AlertWindow AlertWindow;

	// Constructor & initialization
	
	public PluginUi() {
		Windows = new WindowSystem("SelfCare");

		AlertWindow = this.Add<AlertWindow>();
	}

	private T Add<T>() where T : Window, new() {
		var window = new T();
		Windows.AddWindow(window);
		return window;
	}
	
	public override void Init(SelfCare plugin) {
		Services.PluginApi.UiBuilder.DisableGposeUiHide = true;
		Services.PluginApi.UiBuilder.Draw += Windows.Draw;
	}
	
	// Disposal

	public override void Dispose() {
		Windows.RemoveAllWindows();
		Services.PluginApi.UiBuilder.Draw -= Windows.Draw;
	}
}