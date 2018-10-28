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
		LT.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LT.SHOW_LAYOUT(LAYOUT_TYPE.LT_CHARACTER);
		LT.SHOW_LAYOUT(LAYOUT_TYPE.LT_BILLBOARD);
		LT.SHOW_LAYOUT(LAYOUT_TYPE.LT_ROOM_MENU);
		LT.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME_BACK);

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
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME);
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_CHARACTER);
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_BILLBOARD);
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_ROOM_MENU);
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG);
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_FREE_MATCH_TIP);
		LT.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAIN_FRAME_BACK);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}