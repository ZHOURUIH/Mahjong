using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneLoading : SceneProcedure
{
	public MahjongSceneLoading()
	{ }
	public MahjongSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO, 2);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_MAHJONG_GAME_FRAME, 0);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_MAHJONG_DROP, 1);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_MAHJONG_HAND_IN, 1);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_DICE, 3);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_PLAYER_ACTION, 2);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_GAME_ENDING, 2);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_ADD_PLAYER, 4);

		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_WAITING;
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