using System;
using System.Collections.Generic;

using Dalamud.Logging;
using Dalamud.Configuration;
using Dalamud.Interface;

using SelfCare.Alerts;
using SelfCare.Core.Legacy;

namespace SelfCare.Core;

public class PluginConfig : IPluginConfiguration {
	// Base
	
	public int Version { get; set; } = 1;

	// Alerts
	
	public List<Reminder> Reminders = new();

	public int SoundRepeatWait = 5; // Default: 5s

	// Methods for config creation, loading & saving

	private const int PostureTime = 30 * 60; // Default: 30m
	private const int HydrationTime = 60 * 60; // Default: 1h

	private static PluginConfig Create() {
		var cfg = new PluginConfig();
		
		cfg.Reminders.AddRange(new[] {
			new Reminder("Posture Check", PostureTime) {
				Message = "Remember to check your posture!",
				Icon = FontAwesomeIcon.Chair
			},
			new Reminder("Hydration Check", HydrationTime) {
				Message = "Remember to hydrate!",
				Icon = FontAwesomeIcon.GlassWhiskey
			}
		});

		return cfg;
	}

	public static PluginConfig Load() {
		PluginConfig? cfg = null;

		try {
			var cfgBase = Services.PluginApi.GetPluginConfig();
			
			PluginLog.Information(cfgBase is null ? "No config found, creating new file" : $"Loading configuration (Version {cfgBase.Version})");
			
			cfg = cfgBase?.Version switch {
				null => Create(), // doesn't exist, create a new one
				0 => ConfigV0.Upgrade(cfgBase), // legacy config from v0.1
				_ => cfgBase as PluginConfig // try to load as current version
			};
		} catch (Exception err) {
			// config failed to load - might be best to implement some extra handling for this later,
			// such as displaying an alert to the user and/or backing up the previous file.
			PluginLog.Error($"Failed to load configuration:\n{err}");
		} finally {
			// fallback to a new one if loading fails
			cfg ??= Create();
		}

		return cfg;
	}

	public void Save() => Services.PluginApi.SavePluginConfig(this);
}