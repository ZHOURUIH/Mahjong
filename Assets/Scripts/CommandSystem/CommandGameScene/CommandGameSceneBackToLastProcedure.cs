using UnityEngine;
using System.Collections;

public class CommandGameSceneBackToLastProcedure : Command
{
	public string mIntent;
	public CommandGameSceneBackToLastProcedure(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		gameScene.backToLastProcedure(mIntent);
	}
}