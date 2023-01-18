using System;
using System.Timers;

using Dalamud.Interface;

namespace SelfCare.Alerts {
	public class Alert {
		public FontAwesomeIcon Icon;

		// Configurable

		public bool Enabled = true;

		public string Text = "";

		private int _Interval;
		public int Interval {
			get => _Interval;
			set {
				_Interval = value;
				if (Timer != null) Timer.Interval = value;
			}
		}

		// Timer

		internal Timer Timer;
		internal bool HasTimerElapsed = false;

		internal DateTime VisibleSince;
		public bool IsVisible = false;

		// Constructor

		public Alert(FontAwesomeIcon icon, string text, int interval) {
			Icon = icon;
			Text = text;

			_Interval = interval;

			Timer = new();
			Timer.Interval = Interval;
			Timer.Elapsed += OnTimerElapsed;
			Timer.Start();
		}

		// Methods

		public void Trigger() {
			IsVisible = true;
			VisibleSince = DateTime.Now;
			HasTimerElapsed = false;

			if (SelfCare.Config.PrintToChat)
				Services.ChatGui.Print($"[SelfCare] {Text}");
		}

		public void RestartTimer() {
			HasTimerElapsed = false;
			Timer.Start();
		}

		private void OnTimerElapsed(object? sender, ElapsedEventArgs e) {
			Timer.Stop();
			HasTimerElapsed = true;
		}
	}
}