using System;

using SelfCare.Core;

namespace SelfCare.Alerts;

internal class Notifier : IDisposable {
	private readonly AlertManager AlertManager;

	internal Notifier(AlertManager manager)
		=> AlertManager = manager;

	// Dispatch handler

	internal void Subscribe()
		=> AlertManager.OnDispatch += OnDispatchHandler;

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
		if (LastPlayed != null && (timeNow - LastPlayed.Value).TotalSeconds < SelfCare.Config.SoundRepeatWait)
			return;

		if (sfx.Play())
			LastPlayed = timeNow;
	}
	
	// Disposal

	private bool IsDisposed;
	
	public void Dispose() {
		if (IsDisposed) return;

		AlertManager.OnDispatch -= OnDispatchHandler;
		
		IsDisposed = true;

		GC.SuppressFinalize(this);
	}

	~Notifier() => Dispose();
}