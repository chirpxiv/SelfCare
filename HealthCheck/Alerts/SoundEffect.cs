using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HealthCheck.Alerts {
	public enum SoundEffect {
		None = 0,
		//Error = 7,
		IncomingTell = 37,
		//Unk1 = 36,
		Se2 = 38, // ...
		Se9 = 45,
		IncomingMail = 59,
		Enmity = 60,
		DutyPop = 67,
		LimitBreak = 70
	}

	public static class SoundAlert {
		internal delegate IntPtr PlaySoundDelegate(SoundEffect id, IntPtr a2, IntPtr a3);
		internal static PlaySoundDelegate Invoke = null!;

		// SoundEffect => Name

		public static Dictionary<SoundEffect, string> EffectToName = new() {
			{ SoundEffect.IncomingTell, "Incoming Tell" },
			{ SoundEffect.Se2, "<se.2>" },
			{ SoundEffect.Se9, "<se.9>" },
			{ SoundEffect.IncomingMail, "Incoming Mail" },
			{ SoundEffect.DutyPop, "Duty Popped" },
			{ SoundEffect.LimitBreak, "Limit Break" }
		};

		// Init

		public static void Init() {
			var soundAddr = Services.SigScanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE");
			Invoke = Marshal.GetDelegateForFunctionPointer<PlaySoundDelegate>(soundAddr);
		}

		// Methods

		public static string GetName(SoundEffect sound)
			=> EffectToName.TryGetValue(sound, out var name) ? name : sound.ToString();

		public static void PlayCurrent() {
			var sfx = HealthCheck.Config.SoundEffect;
			if (sfx != SoundEffect.None)
				Invoke(sfx, 0, 0);
		}
	}
}