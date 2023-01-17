using System.Timers;

using Dalamud.Interface;

namespace SelfCare.Alerts {
	// I'm over-engineering this but it's far too late to go back

	// TODO: Wait for battle/cutscene to end

	public class Alert {
		public FontAwesomeIcon Icon;

		// Configurable

		public string Text { get; set; } = "";

		private int _Interval = 30000; // TODO: Configurable
		public int Interval {
			get => _Interval;
			internal set {
				_Interval = value;
				Timer.Interval = value;
			}
		}

		// Timer

		internal Timer Timer;
		internal bool HasTimerElapsed = true;

		public bool IsVisible = false;

		// Constructor

		public Alert(FontAwesomeIcon icon, string text) {
			Icon = icon;
			Text = text;

			Timer = new();
			Timer.Interval = Interval;
			Timer.Elapsed += OnTimerElapsed;
		}

		// Methods

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