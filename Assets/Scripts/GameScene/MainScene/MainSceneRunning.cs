using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneRunning : SceneProcedure
{
	public MainSceneRunning()
	{ }
	public MainSceneRunning(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_CHARACTER);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_BILLBOARD);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_ROOM_MENU);
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