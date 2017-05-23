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
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_DROP);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_HAND_IN);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO);
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