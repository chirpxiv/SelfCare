using System;
using System.Collections.Generic;

using Dalamud.Logging;
using Dalamud.Plugin;

using SelfCare.Core;
using SelfCare.Alerts;
using SelfCare.Interface;

namespace SelfCare; 

public sealed class SelfCare : IDalamudPlugin {
	// Plugin info
	
	public string Name { get; init; } = "SelfCare";

	// Internal services & config
	
	public readonly PluginConfig PluginConfig;

	public readonly PluginUi Interface;
	public readonly AlertManager AlertManager;

	// Constructor called on plugin load
	
	private readonly List<ServiceBase> Services = new();
	
	public SelfCare(DalamudPluginInterface api) {
		Core.Services.Init(api);
		
		PluginConfig = PluginConfig.Load();

		Interface = this.Create<PluginUi>();
		AlertManager = this.Create<AlertManager>();

		Services.ForEach(inst => inst.Init(this));
	}

	private T Create<T>() where T : ServiceBase, new() {
		var instance = new T();
		Services.Add(instance);
		return instance;
	}

	// Disposal

	public void Dispose() {
		try {
			PluginConfig.Save();
		} catch (Exception err) {
			PluginLog.Error($"Configuration failed to save:\n{err}");
		}

		Services.ForEach(Dispose);
		Services.Clear();
	}

	private void Dispose(IDisposable inst) {
		try {
			inst.Dispose();
		} catch (Exception err) {
			PluginLog.Error($"Failed to dispose {inst.GetType().Name}:\n{err}");
		}
	}
}