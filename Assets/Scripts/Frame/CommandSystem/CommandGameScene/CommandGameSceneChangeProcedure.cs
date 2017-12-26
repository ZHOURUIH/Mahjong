using UnityEngine;
using System.Collections;

public class CommandGameSceneChangeProcedure : Command
{
	public PROCEDURE_TYPE	mProcedure = PROCEDURE_TYPE.PT_NONE;
	public string			mIntent = "";
	public bool				mForceChange;	// 是否强制跳转,流程有准备退出时也会跳转
	public override void init()
	{
		base.init();
		mProcedure = PROCEDURE_TYPE.PT_NONE;
		mIntent = "";
		mForceChange = false;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		// 强制跳转,或者当前流程没有准备跳转机制才会直接跳转
		if((gameScene.getCurSceneProcedure() == null || !gameScene.getCurSceneProcedure().hasPrepareExit()) || mForceChange)
		{
			gameScene.changeProcedure(mProcedure, mIntent);
		}
		else
		{
			gameScene.prepareChangeProcedure(mProcedure, mIntent);
		}
		mLogSystem.logProcedure("进入流程 : " + mProcedure.ToString());
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : procedure : " + mProcedure + ", intent : " + mIntent + ", force change : " + mForceChange;
	}
}