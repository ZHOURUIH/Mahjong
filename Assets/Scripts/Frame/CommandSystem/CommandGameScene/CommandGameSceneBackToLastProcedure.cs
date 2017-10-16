using UnityEngine;
using System.Collections;

public class CommandGameSceneBackToLastProcedure : Command
{
	public string mIntent;
	public override void init()
	{
		base.init();
		mIntent = "";
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		gameScene.backToLastProcedure(mIntent);
	}
}