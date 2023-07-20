using System;
using System.Linq;
using System.Collections.Generic;

using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;

using SelfCare.Core;

namespace SelfCare.Alerts; 

public class AlertManager : ServiceBase {
	// Config access
	
	private readonly Notifier Notifier;
	
	private PluginConfig Config = null!;

	// Constructor & initialization

	public AlertManager() {
		Notifier = new Notifier(this);
	}

	public override void Init(SelfCare plugin) {
		Config = plugin.PluginConfig;

		Notifier.Subscribe(Config);

		Config.Reminders.ForEach(r => Register(r));

		Services.Framework.Update += OnFrameworkUpdate;

		StartAll();
	}
	
	// Timer management

	private readonly List<AlertTimer> Timers = new();

	private AlertTimer Register(Reminder reminder) {
		var alert = new AlertTimer(reminder);
		alert.OnElapsed += OnElapsedHandler;
		Timers.Add(alert);
		return alert;
	}

	private void Remove(AlertTimer alert) {
		alert.Dispose();
		Timers.Remove(alert);
	}

	private void StartAll() => Timers
		.ForEach(t => t.Start());

	private void StopAll() => Timers
		.ForEach(t => t.Stop());
	
	// Elapsed timer handling
	
	private readonly object HandlerLock = new();
	
	private readonly List<AlertTimer> ElapsedQueue = new();

	public event OnTimerElapsed? OnDispatch;

	// Handle elapsed event from AlertTimer and trigger if conditions are met.
	// Otherwise, add it to queue until they are.
	private void OnElapsedHandler(AlertTimer sender) {
		// Use a lock here to avoid race conditions with multiple timers elapsing at once,
		// as the event handler is invoked on an async thread.
		lock (HandlerLock) {
			if (!TryDispatchTimer(sender) && !ElapsedQueue.Contains(sender))
				ElapsedQueue.Add(sender);
		}
	}
	
	// Process queue on every framework tick.
	private void OnFrameworkUpdate(Framework _) {
		// Same as above.
		lock (HandlerLock) {
			var queue = ElapsedQueue.ToList();
			queue.ForEach(t => TryDispatchTimer(t));
		}
	}
	
	// Checks timer conditions and fires an event if all are met.
	private bool TryDispatchTimer(AlertTimer timer) {
		var dispatch = CheckCondition(timer.Reminder.Condition);
		if (dispatch)
			DispatchTimer(timer);
		return dispatch;
	}

	private void DispatchTimer(AlertTimer timer) {
		// Fire event and resume timer.
		
		try {
			OnDispatch?.Invoke(timer);
		} catch (Exception err) {
			PluginLog.Error($"Error while dispatching timer ('{timer.Reminder.Name}'):\n{err}");
		} finally {
			timer.Start();
		}
		
		// Remove from queue if applicable.
		// List.Contains check can be skipped here as List.Remove does it for us.
		ElapsedQueue.Remove(timer);
	}

	// Condition checking
	
	private bool CheckCondition(ReminderCond cond) {
		if (cond == ReminderCond.None)
			return true;
		
		var result = true;
		
		// Player must be in-game
		if (cond.HasFlag(ReminderCond.IsInGame))
			result &= Services.ClientState.IsLoggedIn;

		// Player must not be in combat.
		if (cond.HasFlag(ReminderCond.NotInCombat))
			result &= !Services.Condition[ConditionFlag.InCombat];

		// Player must not be in a cutscene or interacting with a quest.
		if (cond.HasFlag(ReminderCond.NotInCutscene))
			result &= !Services.Condition[ConditionFlag.WatchingCutscene]
				&& !Services.Condition[ConditionFlag.OccupiedInCutSceneEvent]
				&& !Services.Condition[ConditionFlag.OccupiedInQuestEvent];

		// Game UI must be visible.
		if (cond.HasFlag(ReminderCond.IsUiVisible))
			result &= !Services.GameGui.GameUiHidden;

		return result;
	}

	// Disposal

	public override void Dispose() {
		OnDispatch = null;
		
		Services.Framework.Update -= OnFrameworkUpdate;

		Timers.ForEach(t => t.Dispose());
		Timers.Clear();

		ElapsedQueue.Clear();
	}
}