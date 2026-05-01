using System.Linq;

using Dalamud.Interface.Windowing;

namespace SelfCare.Extensions {
	public static class WindowingExtensions {
		public static T? GetWindow<T>(this WindowSystem system)
			where T : Window
			=> system.Windows.OfType<T>().FirstOrDefault();

		public static void Show(this Window window)
			=> window.IsOpen = true;
	}
}
