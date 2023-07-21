using System;
using System.Timers;

namespace SelfCare.Alerts;

public delegate void OnTimerElapsed(AlertTimer sender);

public class AlertTimer : IDisposable {
	// Encapsulated classes
	
	public readonly Reminder Reminder;
	
	private readonly Timer Timer;
	
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
		if (!Timer.Enabled || IsDisposed || !Reminder.Enabled) return;
		
		// Update timer state and invoke event for the alert manager to handle.
		OnElapsed?.Invoke(this);
	}
	
	// Display
	
	public bool CanShow()
		=> Reminder.Enabled && DispatchedAt != null && DateTime.Now < DispatchedAt.Value.AddSeconds(Reminder.DismissTimer);

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