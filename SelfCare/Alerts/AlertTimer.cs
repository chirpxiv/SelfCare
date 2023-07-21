using System;
using System.Timers;

namespace SelfCare.Alerts;

public delegate void OnTimerElapsed(AlertTimer sender);

public class AlertTimer : IDisposable {
	// Encapsulated classes
	
	public readonly Reminder Reminder;
	
	private readonly Timer Timer;
	
	// Wrappers

	public bool Enabled {
		get => Reminder.Enabled;
		set {
			Reminder.Enabled = value;
			if (value)
				Start();
			else
				Stop();
		}
	}

	public uint WaitTime {
		get => Reminder.WaitTime;
		set {
			Reminder.WaitTime = value;
			Timer.Interval = value * 1000;
		}
	}
	
	// State
	
	public DateTime? DispatchedAt;
	
	// Constructor

	public AlertTimer(Reminder reminder) {
		Reminder = reminder;

		Timer = new Timer();
		Timer.Elapsed += OnElapsedHandler;
	}
	
	// Timer management

	public event OnTimerElapsed? OnElapsed;

	public void Start() {
		Timer.Interval = Reminder.WaitTime * 1000;
		Timer.Start();
	}

	public void Stop() => Timer.Stop();

	private void OnElapsedHandler(object? sender, ElapsedEventArgs args) {
		// Elapsed event can be invoked while timer is stopped or disposed; ignore if this happens.
		// Also ignore if the reminder has been disabled by user. Shouldn't happen but check just in case.
		if (IsDisposed || !Timer.Enabled || !Reminder.Enabled) return;
		
		// Update timer state and invoke event for the alert manager to handle.
		OnElapsed?.Invoke(this);
	}
	
	// Display

	public bool CanShow() => Reminder.Enabled && Reminder.Type.HasFlag(ReminderType.Popup) && Reminder.DismissType switch {
		DismissType.AfterTime => IsDismissTicking(),
		_ => true
	};

	private bool IsDismissTicking() => DispatchedAt is DateTime start
		&& DateTime.Now < start.AddSeconds(Reminder.DismissTimer);

	// Disposal

	private bool IsDisposed;

	public void Dispose() {
		if (IsDisposed) return;

		OnElapsed = null;

		Timer.Elapsed -= OnElapsedHandler;
		Timer.Stop();
		Timer.Close();

		IsDisposed = true;

		GC.SuppressFinalize(this);
	}

	~AlertTimer() => Dispose();
}