using UnityEngine;
using System.Collections;

public class CommandCharacterManagerDestroy : Command
{
	public string mName;
	public CommandCharacterManagerDestroy(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		CharacterManager characterManager = (mReceiver) as CharacterManager;
		characterManager.destroyCharacter(mName);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " :Name : " + mName;
	}
}