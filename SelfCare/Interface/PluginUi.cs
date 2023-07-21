using Dalamud.Interface.Windowing;

using SelfCare.Core;
using SelfCare.Alerts;
using SelfCare.Interface.Windows;

namespace SelfCare.Interface;

public class PluginUi : ServiceBase {
	// Window management
	
	private readonly WindowSystem Windows;

	internal readonly AlertWindow AlertWindow;
	internal readonly ConfigWindow ConfigWindow;

	public bool IsConfigOpen => ConfigWindow.IsOpen;

	// Constructor & initialization
	
	public PluginUi() {
		Windows = new WindowSystem("SelfCare");

		AlertWindow = this.Add<AlertWindow>();
		ConfigWindow = this.Add<ConfigWindow>();
	}

	private T Add<T>() where T : Window, new() {
		var window = new T { RespectCloseHotkey = false };
		Windows.AddWindow(window);
		return window;
	}
	
	public override void Init() {
		Services.PluginApi.UiBuilder.DisableGposeUiHide = true;
		Services.PluginApi.UiBuilder.Draw += Windows.Draw;

		SelfCare.Instance.Alerts.OnDispatch += OnDispatchHandler;
	}
	
	// Open alert window on timer dispatch

	private void OnDispatchHandler(AlertTimer timer) {
		AlertWindow.AddAlert(timer);
	}
	
	// Disposal

	public override void Dispose() {
		Windows.RemoveAllWindows();
		Services.PluginApi.UiBuilder.Draw -= Windows.Draw;

		SelfCare.Instance.Alerts.OnDispatch -= OnDispatchHandler;
	}
}