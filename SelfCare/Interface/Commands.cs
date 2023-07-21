using System;

using Dalamud.Game.Command;

using SelfCare.Core;

namespace SelfCare.Interface;

public class Commands : ServiceBase {
	private const string CommandName = "/selfcare";

	private Action ToggleConfig => Services.Interface.ConfigWindow.Toggle;
	
	// Initialization

	public override void Init() {
		Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommandHandler) {
			HelpMessage = "Shows the SelfCare configuration window."
		});

		Services.PluginApi.UiBuilder.OpenConfigUi += ToggleConfig;
	}
	
	// Command handler

	private void OnCommandHandler(string _name, string _args) {
		ToggleConfig();
	}
	
	// Disposal

	public override void Dispose() {
		Services.CommandManager.RemoveHandler(CommandName);

		Services.PluginApi.UiBuilder.OpenConfigUi -= ToggleConfig;
	}
}