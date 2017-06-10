using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneExit : SceneProcedure
{
	public MahjongSceneExit()
	{ }
	public MahjongSceneExit(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_DROP, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_DROP, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_HAND_IN, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_DICE, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_PLAYER_ACTION, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_GAME_ENDING, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ADD_PLAYER, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_FRAME, true);
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