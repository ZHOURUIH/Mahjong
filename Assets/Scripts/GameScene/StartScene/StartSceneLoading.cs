using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneLoading : SceneProcedure
{
	public LogoSceneLoading()
	{ }
	public LogoSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 加载所有布局
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_START, 0);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_GLOBAL_TOUCH, 100);

		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_RUNNING;
		mCommandSystem.pushDelayCommand(cmd, mGameScene);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}