using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneExit : SceneProcedure
{
	public MainSceneExit()
	{ }
	public MainSceneExit(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_CHARACTER);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BILLBOARD);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ROOM_MENU);

		CommandGameSceneManagerEnter cmd = new CommandGameSceneManagerEnter(true, true);
		cmd.mSceneType = GAME_SCENE_TYPE.GST_MAHJONG;
		mCommandSystem.pushDelayCommand(cmd, mGameSceneManager);
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