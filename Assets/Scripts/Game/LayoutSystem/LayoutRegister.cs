using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutRegister : GameBase
{
	public void registeAllLayout()
	{
        registeLayout<ScriptGlobalTouch>(LAYOUT_TYPE.LT_GLOBAL_TOUCH, "UIGlobalTouch");
        registeLayout<ScriptLogin>(LAYOUT_TYPE.LT_LOGIN, "UILogin");
        registeLayout<ScriptRegister>(LAYOUT_TYPE.LT_REGISTER, "UIRegister");
        registeLayout<ScriptMainFrame>(LAYOUT_TYPE.LT_MAIN_FRAME, "UIMainFrame");
        registeLayout<ScriptBillboard>(LAYOUT_TYPE.LT_BILLBOARD, "UIBillboard");
        registeLayout<ScriptCharacter>(LAYOUT_TYPE.LT_CHARACTER, "UICharacter");
        registeLayout<ScriptRoomMenu>(LAYOUT_TYPE.LT_ROOM_MENU, "UIRoomMenu");
        registeLayout<ScriptMahjongHandIn>(LAYOUT_TYPE.LT_MAHJONG_HAND_IN, "UIMahjongHandIn");
        registeLayout<ScriptMahjongDrop>(LAYOUT_TYPE.LT_MAHJONG_DROP, "UIMahjongDrop");
        registeLayout<ScriptAllCharacterInfo>(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO, "UIAllCharacterInfo");
        registeLayout<ScriptDice>(LAYOUT_TYPE.LT_DICE, "UIDice");
        registeLayout<ScriptMahjongBackFrame>(LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME, "UIMahjongBackFrame");
        registeLayout<ScriptPlayerAction>(LAYOUT_TYPE.LT_PLAYER_ACTION, "UIPlayerAction");
        registeLayout<ScriptGameEnding>(LAYOUT_TYPE.LT_GAME_ENDING, "UIGameEnding");
        registeLayout<ScriptAddPlayer>(LAYOUT_TYPE.LT_ADD_PLAYER, "UIAddPlayer");
        registeLayout<ScriptMahjongFrame>(LAYOUT_TYPE.LT_MAHJONG_FRAME, "UIMahjongFrame");
        registeLayout<ScriptJoinRoomDialog>(LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG, "UIJoinRoomDialog");
        if (mLayoutManager.getLayoutCount() < (int)LAYOUT_TYPE.LT_MAX)
		{
			UnityUtility.logError("error : not all script added! max count : " + (int)LAYOUT_TYPE.LT_MAX + ", added count :" + mLayoutManager.getLayoutCount());
		}
	}
	public void onScriptCreated(LayoutScript script)
	{
		// 只有布局与脚本唯一对应的才能使用变量快速访问
		if (mLayoutManager.getScriptMappingCount(script.GetType()) > 1)
		{
			return;
		}
		if (assign(ref mScriptGlobalTouch, script)) return;
		if (assign(ref mScriptLogin, script)) return;
		if (assign(ref mScriptRegister, script)) return;
		if (assign(ref mScriptMainFrame, script)) return;
		if (assign(ref mScriptBillboard, script)) return;
		if (assign(ref mScriptCharacter, script)) return;
		if (assign(ref mScriptRoomMenu, script)) return;
		if (assign(ref mScriptMahjongHandIn, script)) return;
		if (assign(ref mScriptMahjongDrop, script)) return;
		if (assign(ref mScriptAllCharacterInfo, script)) return;
		if (assign(ref mScriptDice, script)) return;
		if (assign(ref mScriptMahjongBackFrame, script)) return;
		if (assign(ref mScriptPlayerAction, script)) return;
		if (assign(ref mScriptGameEnding, script)) return;
		if (assign(ref mScriptAddPlayer, script)) return;
		if (assign(ref mScriptMahjongFrame, script)) return;
		if (assign(ref mScriptJoinRoomDialog, script)) return;
	}
	public void onScriptDestroy(LayoutScript script)
	{
		if (mLayoutManager.getScriptMappingCount(script.GetType()) > 1)
		{
			return;
		}
		if (clear(ref mScriptGlobalTouch, script)) return;
		if (clear(ref mScriptLogin, script)) return;
		if (clear(ref mScriptRegister, script)) return;
		if (clear(ref mScriptMainFrame, script)) return;
		if (clear(ref mScriptBillboard, script)) return;
		if (clear(ref mScriptCharacter, script)) return;
		if (clear(ref mScriptRoomMenu, script)) return;
		if (clear(ref mScriptMahjongHandIn, script)) return;
		if (clear(ref mScriptMahjongDrop, script)) return;
		if (clear(ref mScriptAllCharacterInfo, script)) return;
		if (clear(ref mScriptDice, script)) return;
		if (clear(ref mScriptMahjongBackFrame, script)) return;
		if (clear(ref mScriptPlayerAction, script)) return;
		if (clear(ref mScriptGameEnding, script)) return;
		if (clear(ref mScriptAddPlayer, script)) return;
		if (clear(ref mScriptMahjongFrame, script)) return;
		if (clear(ref mScriptJoinRoomDialog, script)) return;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	protected void registeLayout<T>(LAYOUT_TYPE layout, string name) where T : LayoutScript
	{
		mLayoutManager.registeLayout(typeof(T), layout, name);
	}
	protected bool assign<T>(ref T thisScript, LayoutScript value) where T : LayoutScript
	{
		if (typeof(T) == value.GetType())
		{
			thisScript = value as T;
			return true;
		}
		else
		{
			return false;
		}
	}
	protected bool clear<T>(ref T thisScript, LayoutScript value) where T : LayoutScript
	{
		if (typeof(T) == value.GetType())
		{
			thisScript = null;
			return true;
		}
		else
		{
			return false;
		}
	}
}