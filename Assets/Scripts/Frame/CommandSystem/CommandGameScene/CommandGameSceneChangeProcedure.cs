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
		// 当流程正在准备跳转流程时,不允许再跳转
		SceneProcedure curProcedure = gameScene.getCurSceneProcedure();
		if(curProcedure != null	&& curProcedure.isPreparingExit())
		{
			UnityUtility.logError("procedure is preparing to change, can not change again!");
		}
		else
		{
			gameScene.changeProcedure(mProcedure, mIntent);
			mLogSystem.logProcedure("进入流程 : " + mProcedure.ToString());
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : procedure : " + mProcedure + ", intent : " + mIntent;
	}
}