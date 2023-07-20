using System;

using FFXIVClientStructs.FFXIV.Client.UI;

using SelfCare.Core;

namespace SelfCare.Alerts; 

internal class Notifier {
	private readonly AlertManager AlertManager;

	private PluginConfig Config = null!;
	
	internal Notifier(AlertManager manager)
		=> AlertManager = manager;

	// Dispatch handler

	internal void Subscribe(PluginConfig cfg) {
		Config = cfg;
		AlertManager.OnDispatch += OnDispatchHandler;
	}

	private void OnDispatchHandler(AlertTimer timer) {
		var type = timer.Reminder.Type;

		if (type.HasFlag(ReminderType.Chat))
			Services.ChatGui.Print($"[SelfCare] {timer.Reminder.Message}");

		if (type.HasFlag(ReminderType.Sound))
			PlaySound(timer.Reminder.SoundEffect);
	}
	
	// Sound alerts

	private DateTime? LastPlayed;

	private void PlaySound(SoundEffect sfx) {
		var timeNow = DateTime.Now;
		if (LastPlayed != null && (timeNow - LastPlayed.Value).TotalSeconds < Config.SoundRepeatWait)
			return;

		if (UIModule.PlaySound((uint)sfx, 0, 0, 0))
			LastPlayed = timeNow;
	}
}