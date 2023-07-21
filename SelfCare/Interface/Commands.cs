using Dalamud.Game.Command;

using SelfCare.Core;

namespace SelfCare.Interface;

public class Commands : ServiceBase {
	private const string CommandName = "/selfcare";
	
	// Initialization

	public override void Init() {
		Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommandHandler) {
			HelpMessage = "Shows the SelfCare configuration window."
		});
	}
	
	// Command handler

	private void OnCommandHandler(string _name, string _args) {
		// TODO: Toggle window
	}
	
	// Disposal

	public override void Dispose() {
		Services.CommandManager.RemoveHandler(CommandName);
	}
}