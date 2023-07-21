using System.Collections.Generic;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;

using ImGuiNET;

using SelfCare.Alerts;

namespace SelfCare.Interface.Windows;

public class AlertWindow : Window {
	public AlertWindow() : base(
		"SelfCare Alert",
		ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDecoration
	) {
		RespectCloseHotkey = false;
	}
	
	// Alerts to display. PluginUi adds these when a timer is dispatched.

	private readonly List<AlertTimer> Alerts = new();

	public void AddAlert(AlertTimer alert) {
		if (Alerts.Contains(alert)) return;
		Alerts.Add(alert);
		IsOpen = true;
	}
	
	// Check whether the window should be open this frame.

	public override void PreOpenCheck() {
		// Close the window if all alerts have been dismissed.
		IsOpen = Alerts.Count > 0;
	}
	
	// Draw UI

	public override void Draw() {
		// Iterates and displays each alert.
		// Inserts spacing between alerts if there is more than one.
		for (var i = 0; i < Alerts.Count; i++) {
			if (i > 0) ImGui.Spacing();
			DrawAlert(Alerts[i]);
		}
	}

	private void DrawAlert(AlertTimer alert) {
		// Displays the icon and message associated with the timer.
		// Attempts to align them so that multiple alerts can display together in a consistent manner.
		
		var font = UiBuilder.IconFont;
		ImGui.PushFont(font);

		var icon = alert.Reminder.Icon.ToIconString();
		
		var start = font.FontSize;
		ImGui.SetCursorPosX(start - ImGui.CalcTextSize(icon).X / 2);
		
		ImGui.Text(icon);
		ImGui.PopFont();
		
		ImGui.SameLine();
		ImGui.SetCursorPosX(start + ImGui.GetStyle().ItemSpacing.X * 2);
		ImGui.Text(alert.Reminder.Message);
	}
	
	// Code run after window finishes drawing.

	public override void PostDraw() {
		// Remove alerts when their dismiss timers have ended.
		Alerts.RemoveAll(alert => !alert.CanShow());
	}
}