using System;
using System.Collections.Generic;

using Dalamud.Plugin;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;

using SelfCare.Alerts;
using SelfCare.Interface;

namespace SelfCare.Core;

internal class Services {
	// Dalamud plugin services

	internal static DalamudPluginInterface PluginApi = null!;

	[PluginService] internal static GameGui GameGui { get; set; } = null!;
	[PluginService] internal static ChatGui ChatGui { get; set; } = null!;
	[PluginService] internal static Framework Framework { get; set; } = null!;
	[PluginService] internal static Condition Condition { get; set; } = null!;
	[PluginService] internal static ClientState ClientState { get; set; } = null!;
	[PluginService] internal static CommandManager CommandManager { get; set; } = null!;
	
	// Internal services

	internal static AlertManager Alerts = null!;
	internal static PluginUi Interface = null!;
	internal static Commands Commands = null!;

	// Service injection and creation

	private readonly static List<ServiceBase> Registered = new();

	internal static void Create(DalamudPluginInterface api) {
		PluginApi = api;
		PluginApi.Create<Services>();
		
		Alerts = Create<AlertManager>();
		Interface = Create<PluginUi>();
		Commands = Create<Commands>();
	}

	internal static void Init() => Registered
		.ForEach(inst => inst.Init());

	private static T Create<T>() where T : ServiceBase, new() {
		var inst = new T();
		Registered.Add(inst);
		return inst;
	}
	
	// Disposal

	internal static void Dispose() {
		Registered.ForEach(Dispose);
		Registered.Clear();
	}

	private static void Dispose(IDisposable inst) {
		try {
			inst.Dispose();
		} catch (Exception err) {
			PluginLog.Error($"Failed to dispose {inst.GetType().Name}:\n{err}");
		}
	}
}