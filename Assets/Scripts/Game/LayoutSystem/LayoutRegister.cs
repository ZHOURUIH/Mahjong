using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutRegister : GameBase
{
	public static void registeAllLayout()
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
		registeLayout<ScriptMessageOK>(LAYOUT_TYPE.LT_MESSAGE_OK, "UIMessageOK");
		registeLayout<ScriptMainLoading>(LAYOUT_TYPE.LT_MAIN_LOADING, "UIMainLoading");
		registeLayout<ScriptMahjongLoading>(LAYOUT_TYPE.LT_MAHJONG_LOADING, "UIMahjongLoading");
		if (mLayoutManager.getLayoutCount() < (int)LAYOUT_TYPE.LT_MAX)
		{
			UnityUtility.logError("error : not all script added! max count : " + (int)LAYOUT_TYPE.LT_MAX + ", added count :" + mLayoutManager.getLayoutCount());
		}
	}
	public static void onScriptChanged(LayoutScript script, bool created = true)
	{
		// 只有布局与脚本唯一对应的才能使用变量快速访问
		if (mLayoutManager.getScriptMappingCount(script.GetType()) > 1)
		{
			return;
		}
		if (assign(ref mScriptGlobalTouch, script, created)) return;
		if (assign(ref mScriptLogin, script, created)) return;
		if (assign(ref mScriptRegister, script, created)) return;
		if (assign(ref mScriptMainFrame, script, created)) return;
		if (assign(ref mScriptBillboard, script, created)) return;
		if (assign(ref mScriptCharacter, script, created)) return;
		if (assign(ref mScriptRoomMenu, script, created)) return;
		if (assign(ref mScriptMahjongHandIn, script, created)) return;
		if (assign(ref mScriptMahjongDrop, script, created)) return;
		if (assign(ref mScriptAllCharacterInfo, script, created)) return;
		if (assign(ref mScriptDice, script, created)) return;
		if (assign(ref mScriptMahjongBackFrame, script, created)) return;
		if (assign(ref mScriptPlayerAction, script, created)) return;
		if (assign(ref mScriptGameEnding, script, created)) return;
		if (assign(ref mScriptAddPlayer, script, created)) return;
		if (assign(ref mScriptMahjongFrame, script, created)) return;
		if (assign(ref mScriptJoinRoomDialog, script, created)) return;
		if (assign(ref mScriptMessageOK, script, created)) return;
		if (assign(ref mScriptMainLoading, script, created)) return;
		if (assign(ref mScriptMahjongLoading, script, created)) return;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	protected static void registeLayout<T>(LAYOUT_TYPE layout, string name) where T : LayoutScript
	{
		mLayoutManager.registeLayout(typeof(T), layout, name);
	}
	protected static bool assign<T>(ref T thisScript, LayoutScript value, bool created) where T : LayoutScript
	{
		if (typeof(T) == value.GetType())
		{
			thisScript = created ? value as T : null;
			return true;
		}
		else
		{
			return false;
		}
	}
}