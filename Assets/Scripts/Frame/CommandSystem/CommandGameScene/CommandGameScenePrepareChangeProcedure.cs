using UnityEngine;
using System.Collections;

public class CommandGameScenePrepareChangeProcedure : Command
{
	public PROCEDURE_TYPE	mProcedure = PROCEDURE_TYPE.PT_NONE;
	public string			mIntent = "";
	public float			mPrepareTime;
	public override void init()
	{
		base.init();
		mProcedure = PROCEDURE_TYPE.PT_NONE;
		mIntent = "";
		mPrepareTime = -1.0f;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		// 准备时间必须大于0
		SceneProcedure curProcedure = gameScene.getCurSceneProcedure();
		if(mPrepareTime <= 0.0f)
		{
			UnityUtility.logError("preapare time must be larger than 0!");
		}
		// 正在准备跳转时,不允许再次准备跳转
		else if(curProcedure.isPreparingExit())
		{
			UnityUtility.logError("procedure is preparing to exit, can not prepare again!");
		}
		else
		{
			gameScene.prepareChangeProcedure(mProcedure, mPrepareTime, mIntent);
			if(mFrameLogSystem != null)
			{
				mFrameLogSystem.logProcedure("准备进入流程 : " + mProcedure.ToString());
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : procedure : " + mProcedure + ", intent : " + mIntent + ", prepare time : " + mPrepareTime;
	}
}