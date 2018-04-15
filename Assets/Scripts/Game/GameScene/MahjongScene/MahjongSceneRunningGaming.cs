using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneRunningGaming : SceneProcedure
{
	public MahjongSceneRunningGaming(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_DROP);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_PLAYER_ACTION);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_DROP);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_PLAYER_ACTION);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_HAND_IN);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}