using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneExit : SceneProcedure
{
	public MainSceneExit(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_CHARACTER, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BILLBOARD, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ROOM_MENU, true);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG, true);
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