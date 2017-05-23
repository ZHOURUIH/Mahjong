using UnityEngine;
using System.Collections;

public class LogoSceneExit : SceneProcedure
{
	public LogoSceneExit()
	{ }
	public LogoSceneExit(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 进入到主场景
		CommandGameSceneManagerEnter cmdEnterMain = new CommandGameSceneManagerEnter(true, true);
		cmdEnterMain.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
		mCommandSystem.pushDelayCommand(cmdEnterMain, mGameSceneManager);
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