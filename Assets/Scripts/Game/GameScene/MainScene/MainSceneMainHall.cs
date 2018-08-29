using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneMainHall : SceneProcedure
{
	public MainSceneMainHall(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_CHARACTER);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_BILLBOARD);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_ROOM_MENU);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME_BACK);

		// 显示角色信息
		CharacterMyself myself = mCharacterManager.getMyself();
		if (myself != null)
		{
			CharacterData data = myself.getCharacterData();
			mScriptCharacter.setCharacterInfo(data.mHead, data.mName, data.mGUID, data.mMoney);
		}
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_CHARACTER);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BILLBOARD);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ROOM_MENU);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_FREE_MATCH_TIP);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME_BACK);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}