using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SelfCare.Alerts {
	public enum SoundEffect {
		None = 0,
		//Error = 7,
		Se1 = 37,
		//Unk1 = 36,
		Se2 = 38,
		Se3 = 39,
		Se4 = 40,
		Se5 = 41,
		Se6 = 42,
		Se7 = 43,
		Se8 = 44,
		Se9 = 45,
		Se10 = 46,
		Se11 = 47,
		Se12 = 48,
		Se13 = 49,
		Se14 = 50,
		Se15 = 51,
		Se16 = 52,
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
			{ SoundEffect.Se1, "Sound Effect 1" },
			{ SoundEffect.Se2, "Sound Effect 2" },
			{ SoundEffect.Se3, "Sound Effect 3" },
			{ SoundEffect.Se4, "Sound Effect 4" },
			{ SoundEffect.Se5, "Sound Effect 5" },
			{ SoundEffect.Se6, "Sound Effect 6" },
			{ SoundEffect.Se7, "Sound Effect 7" },
			{ SoundEffect.Se8, "Sound Effect 8" },
			{ SoundEffect.Se9, "Sound Effect 9" },
			{ SoundEffect.Se10, "Sound Effect 10" },
			{ SoundEffect.Se11, "Sound Effect 11" },
			{ SoundEffect.Se12, "Sound Effect 12" },
			{ SoundEffect.Se13, "Sound Effect 13" },
			{ SoundEffect.Se14, "Sound Effect 14" },
			{ SoundEffect.Se15, "Sound Effect 15" },
			{ SoundEffect.Se16, "Sound Effect 16" },
			{ SoundEffect.IncomingMail, "Incoming Mail" },
			{ SoundEffect.DutyPop, "Duty Popped" },
			{ SoundEffect.LimitBreak, "Limit Break" }
		};

		// Init

		public static void Init() {
			// Thanks Ottermandias -
			// https://github.com/Ottermandias/GatherBuddy/blob/main/GatherBuddy/SeFunctions/PlaySound.cs

			var soundAddr = Services.SigScanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE");
			Invoke = Marshal.GetDelegateForFunctionPointer<PlaySoundDelegate>(soundAddr);
		}

		// Methods

		public static string GetName(SoundEffect sound)
			=> EffectToName.TryGetValue(sound, out var name) ? name : sound.ToString();

		public static void PlayCurrent() {
			var sfx = SelfCare.Config.SoundEffect;
			if (sfx != SoundEffect.None)
				Invoke(sfx, 0, 0);
		}
	}
}