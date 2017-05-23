using UnityEngine;
using System.Collections;

public class CommandGameSceneChangeProcedure : Command
{
	public PROCEDURE_TYPE mProcedure;
	public string mIntent;
	public CommandGameSceneChangeProcedure(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		gameScene.changeProcedure(mProcedure, mIntent);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + ": procedure : " + mProcedure;
	}
}