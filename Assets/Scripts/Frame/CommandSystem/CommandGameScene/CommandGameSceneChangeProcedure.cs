using UnityEngine;
using System.Collections;

public class CommandGameSceneChangeProcedure : Command
{
	public PROCEDURE_TYPE	mProcedure = PROCEDURE_TYPE.PT_NONE;
	public string			mIntent = "";
	public override void init()
	{
		base.init();
		mProcedure = PROCEDURE_TYPE.PT_NONE;
		mIntent = "";
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		gameScene.changeProcedure(mProcedure, mIntent);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : procedure : " + mProcedure + ", intent : " + mIntent;
	}
}