using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Dalamud.Interface.Windowing;

namespace SelfCare.Extensions {
	public static class WindowingExtensions {
		private static readonly FieldInfo WindowsField = typeof(WindowSystem).GetField("windows", BindingFlags.Instance | BindingFlags.NonPublic)!;
		private static List<Window> WindowsList(WindowSystem system) => (List<Window>?)WindowsField.GetValue(system)!;

		public static Window? GetWindow<T>(this WindowSystem system)
			=> WindowsList(system).FirstOrDefault(window => window is T);

		public static void Show(this Window window)
			=> window.IsOpen = true;
	}
}