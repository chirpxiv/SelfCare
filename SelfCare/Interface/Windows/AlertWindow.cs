using System.Linq;
using System.Collections.Generic;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;

using ImGuiNET;

using SelfCare.Alerts;
using SelfCare.Core;

namespace SelfCare.Interface.Windows;

public class AlertWindow : Window {
	private bool IsConfigOpen => Services.Interface.IsConfigOpen;
	
	// Window constructor
	
	public AlertWindow() : base(
		"SelfCare Alert",
		ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove
	) {
		RespectCloseHotkey = false;
	}
	
	// Alerts to display. PluginUi pushes onto this when a timer is dispatched.

	private readonly List<AlertTimer> Alerts = new();

	public void AddAlert(AlertTimer alert) {
		if (Alerts.Contains(alert)) return;
		Alerts.Add(alert);
		IsOpen = true;
	}

	private IEnumerable<AlertTimer> GetAlerts() {
		if (!IsConfigOpen)
			return Alerts;
		
		return Services.Alerts.GetEnabled()
			.Where(t => t.Reminder.Type.HasFlag(ReminderType.Popup));
	}

	// Check whether the window should be open this frame.

	public override void PreOpenCheck() {
		// Close the window if all alerts have been dismissed.
		IsOpen = Alerts.Count > 0 || IsConfigOpen;
	}
	
	// Draw UI

	public override void PreDraw() {
		if (IsConfigOpen)
			Flags &= ~ImGuiWindowFlags.NoMove;
		else
			Flags |= ImGuiWindowFlags.NoMove;
	}

	public override void Draw() {
		var alerts = GetAlerts();

		// Iterates over and displays each alert.
		// Inserts spacing between alerts if there is more than one.
		var i = 0;
		foreach (var alert in alerts) {
			if (i++ > 0)
				ImGui.Spacing();
			DrawReminder(alert.Reminder);
		}

		if (i == 0) DrawMessage(FontAwesomeIcon.Shrimp, "Pop-up messages will appear here when enabled.");
	}

	private void DrawReminder(Reminder item)
		=> DrawMessage(item.Icon, item.Message);

	private void DrawMessage(FontAwesomeIcon icon, string message) {
		// Displays the icon and message associated with the timer.
		// Attempts to align them so that multiple alerts can display together in a consistent manner.
		
		var font = UiBuilder.IconFont;
		ImGui.PushFont(font);

		var str = icon.ToIconString();
		
		var start = font.FontSize;
		ImGui.SetCursorPosX(start - ImGui.CalcTextSize(str).X / 2);
		
		ImGui.Text(str);
		ImGui.PopFont();
		
		ImGui.SameLine();
		ImGui.SetCursorPosX(start + ImGui.GetStyle().ItemSpacing.X * 2);
		ImGui.Text(message);
	}
	
	// Code to run after window is done drawing.

	public override void PostDraw() {
		// Remove alerts when their dismiss timers have ended.
		Alerts.RemoveAll(alert => !alert.CanShow());
	}
}