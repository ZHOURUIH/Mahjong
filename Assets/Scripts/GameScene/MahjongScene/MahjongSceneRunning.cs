using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneRunning : SceneProcedure
{
	public MahjongSceneRunning()
	{ }
	public MahjongSceneRunning(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		;
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_FRAME);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}