using System;

using Dalamud.Interface;

namespace SelfCare.Alerts;

public class Reminder {
	// Config properties
	
	public bool Enabled;

	public string Name;

	public FontAwesomeIcon Icon = FontAwesomeIcon.None;
	
	public uint WaitTime;
	public uint DismissTimer = 10;
	
	public string Message = string.Empty;

	public SoundEffect SoundEffect = SoundEffect.Default;

	public ReminderType Type = ReminderType.Default;
	public ReminderCond Condition = ReminderCond.Default;

	public Reminder(string name, uint timer) {
		Enabled = true;
		Name = name;
		WaitTime = timer;
	}
}

// Display types for the reminder to show up, one or more of these can be activated at once.

[Flags]
public enum ReminderType {
	None  = 0x0,
	Popup = 0x1,
	Chat  = 0x2,
	Sound = 0x4,
	Default = Popup | Chat | Sound
}

// Conditions for the reminder to show under. The timer can be elapsed at any time,
// but it should not be shown to the user or resumed until all conditions are met.

[Flags]
public enum ReminderCond {
	None       = 0x0,
	IsInGame   = 0x1,
	NotInCombat   = 0x2,
	NotInCutscene = 0x4,
	IsUiVisible  = 0x8,
	Default = IsInGame | NotInCombat | NotInCutscene
}