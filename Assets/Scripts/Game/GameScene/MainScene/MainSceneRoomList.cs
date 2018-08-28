using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneRoomList : SceneProcedure
{
	public MainSceneRoomList(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_BACK_TO_MAIN_HALL);
		// 向服务器请求房间列表
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BACK_TO_MAIN_HALL);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}