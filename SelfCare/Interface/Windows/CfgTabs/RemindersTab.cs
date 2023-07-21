using System.Numerics;
using System.Collections.Generic;

using Dalamud.Interface;

using ImGuiNET;

using SelfCare.Alerts;
using SelfCare.Core;
using SelfCare.Interface.Widgets;

namespace SelfCare.Interface.Windows.CfgTabs; 

public class RemindersTab : IConfigTab {
	// Reminders tab
	
	private readonly SoundNameDict SoundEffects;
	
	private AlertTimer? Selected;

	public RemindersTab() {
		SoundEffects = new SoundNameDict();
	}
	
	// Draw UI
	
	public void Draw() {
		DrawList();

		ImGui.SameLine();

		DrawMainRegion();
	}
	
	// Reminder list

	private void DrawList() {
		ImGui.BeginGroup();
		
		var listSize = new Vector2(ImGui.GetFontSize() * 12, -1);
		if (ImGui.BeginChildFrame(0xCF40, listSize)) {
			//SelfCare.Config.Reminders.ForEach(DrawListItem);
			var timers = Services.Alerts.GetAll();
			foreach (var alert in timers)
				DrawListItem(alert);
			
			ImGui.EndChildFrame();
		}

		ImGui.EndGroup();
	}

	private void DrawListItem(AlertTimer item) {
		var isDisabled = !item.Reminder.Enabled;
		if (isDisabled)
			ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled));
		
		var isSelected = item == Selected;
		if (ImGui.Selectable(item.Reminder.Name, isSelected))
			Selected = isSelected ? null : item;

		if (isDisabled)
			ImGui.PopStyleColor();
	}
	
	// Draw selected

	private void DrawMainRegion() {
		ImGui.BeginGroup();

		// Make background invisible for this child frame only;
		// doing it this way to avoid making textboxes etc invisible as well.
		ImGui.PushStyleColor(ImGuiCol.FrameBg, 0);
		var beginFrame = ImGui.BeginChildFrame(0xCF41, -Vector2.One);
		ImGui.PopStyleColor();
		
		if (beginFrame) {
			if (Selected is null) {
				ImGui.Text("No reminder currently selected.");
				ImGui.Text("Choose one from the list on the right by clicking on its name.");
			} else {
				DrawSelected(Selected);
			}
			
			ImGui.EndChildFrame();
		}

		ImGui.EndGroup();
	}

	private void DrawSelected(AlertTimer item) {
		var reminder = item.Reminder;
		
		// Basic info

		var enabled = item.Enabled;
		if (ImGui.Checkbox("##ReminderEnabled", ref enabled))
			item.Enabled = enabled;
		ImGui.SameLine();
		ImGui.InputTextWithHint("##ReminderName", "Type a short name for your timer here...", ref reminder.Name, 28);
		
		// Timer
		
		ImGui.Spacing();
		DrawTimer(item);
		
		// Reminder type

		SpacedSeparator();
		DrawReminderType(reminder);

		// Alert configuration

		if (reminder.Type != ReminderType.None)
			SpacedSeparator();
		
		var hasSound = reminder.Type.HasFlag(ReminderType.Sound);
		
		var isPopup = reminder.Type.HasFlag(ReminderType.Popup);
		var hasMessage = isPopup || reminder.Type.HasFlag(ReminderType.Chat);

		if (hasMessage) {
			DrawMessage(reminder, isPopup);
			if (hasSound)
				ImGui.Spacing();
		}

		if (hasSound)
			DrawSoundSelect(reminder);

		if (isPopup) {
			SpacedSeparator();
			DrawDismissType(reminder);
		}

		// Conditions
		
		SpacedSeparator();
		DrawReminderConditions(reminder);

		ImGui.Spacing();
	}
	
	// Timer

	private void DrawTimer(AlertTimer item) {
		ImGui.Text("Remind me every:");
		var time = item.WaitTime;
		if (TimeInput.Draw("TimerInterval", ref time, 1))
			item.WaitTime = time;
	}
	
	// Reminder types

	private readonly Dictionary<ReminderType, string> ReminderTypes = new() {
		{ ReminderType.Popup, "Pop-up message" },
		{ ReminderType.Chat, "Chat message" },
		{ ReminderType.Sound, "Sound alert" }
	};

	private void DrawReminderType(Reminder item) {
		ImGui.Text("Reminder types:");
		ImGui.Text("Different settings will be available depending on the type(s) chosen.");

		ImGui.Spacing();
		
		foreach (var (flag, text) in ReminderTypes) {
			var hasFlag = item.Type.HasFlag(flag);
			if (ImGui.Checkbox(text, ref hasFlag))
				item.Type ^= flag;
		}
	}

	// Sound effect

	private void DrawSoundSelect(Reminder item) {
		ImGui.Text("Sound to play with this reminder:");

		if (DrawIconButton(FontAwesomeIcon.Play))
			item.SoundEffect.Play();
		
		ImGui.SameLine();
		
		var current = SoundEffects.GetName(item.SoundEffect);
		if (ImGui.BeginCombo("##ReminderSfx", current)) {
			foreach (var (sfx, name) in SoundEffects.Get())
				DrawSoundEffect(sfx, name, ref item.SoundEffect);
			ImGui.EndCombo();
		}
	}

	private void DrawSoundEffect(SoundEffect sfx, string name, ref SoundEffect val) {
		if (ImGui.Selectable(name)) {
			val = sfx;
			sfx.Play();
		}
	}
	
	// Chat/pop-up message

	private readonly Dictionary<DismissType, string> DismissTypes = new() {
		{ DismissType.OnClick, "Click to dismiss" },
		{ DismissType.AfterTime, "Dismiss after x time" }
	};

	private readonly Dictionary<ImGuiMouseButton, string> MouseButtons = new() {
		{ ImGuiMouseButton.Left, "Left Click" },
		{ ImGuiMouseButton.Middle, "Middle Click" },
		{ ImGuiMouseButton.Right, "Right Click" }
	};

	private void DrawMessage(Reminder item, bool isPopup = false) {
		ImGui.Text("Message to show with this reminder:");
		if (isPopup) {
			DrawIconButton(item.Icon);
			ImGui.SameLine();
		}
		ImGui.InputTextWithHint("##ReminderMessage", "Type your message here...", ref item.Message, 64);
	}

	private void DrawDismissType(Reminder item) {
		ImGui.Text("Pop-up dismissal type:");
		foreach (var (type, text) in DismissTypes) {
			if (ImGui.RadioButton(text, item.DismissType == type))
				item.DismissType = type;
		}
		ImGui.Spacing();
		switch (item.DismissType) {
			case DismissType.OnClick:
				SelectMouseButton(ref item.DismissButton);
				break;
			case DismissType.AfterTime:
				TimeInput.Draw("DismissTimer", ref item.DismissTimer, 1);
				break;
		}
	}

	private void SelectMouseButton(ref ImGuiMouseButton value) {
		var preview = MouseButtons.TryGetValue(value, out var valName) ? valName : value.ToString();
		
		if (ImGui.BeginCombo("Button", preview)) {
			foreach (var (button, text) in MouseButtons) {
				if (ImGui.Selectable(text, button == value))
					value = button;
			}
			
			ImGui.EndCombo();
		}
	}
	
	// Conditions

	private readonly Dictionary<ReminderCond, string> Conditions = new() {
		{ ReminderCond.NotInCombat, "You are not engaged in combat." },
		{ ReminderCond.NotInCutscene, "You are not watching a cutscene." },
		{ ReminderCond.IsUiVisible, "Game UI must be visible." }
	};

	private void DrawReminderConditions(Reminder item) {
		ImGui.Text("Reminder conditions:");
		ImGui.Text("This reminder will wait for all selected conditions to be met.");

		ImGui.Spacing();
		
		foreach (var (cond, text) in Conditions) {
			var enabled = item.Condition.HasFlag(cond);
			if (ImGui.Checkbox(text, ref enabled))
				item.Condition ^= cond;
		}
	}
	
	// Helpers

	private void SpacedSeparator() {
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
	}

	private bool DrawIconButton(FontAwesomeIcon icon) {
		ImGui.PushFont(UiBuilder.IconFont);
		
		var size = ImGui.GetFontSize() + ImGui.GetStyle().ItemInnerSpacing.X * 2;
		var result = ImGui.Button(icon.ToIconString(), new Vector2(size, 0));
		
		ImGui.PopFont();

		return result;
	}
}