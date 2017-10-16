using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutRegister : GameBase
{
	public void registeAllLayout()
	{
        registeLayout(typeof(ScriptGlobalTouch), LAYOUT_TYPE.LT_GLOBAL_TOUCH, "UIGlobalTouch");
        registeLayout(typeof(ScriptLogin), LAYOUT_TYPE.LT_LOGIN, "UILogin");
        registeLayout(typeof(ScriptRegister), LAYOUT_TYPE.LT_REGISTER, "UIRegister");
        registeLayout(typeof(ScriptMainFrame), LAYOUT_TYPE.LT_MAIN_FRAME, "UIMainFrame");
        registeLayout(typeof(ScriptBillboard), LAYOUT_TYPE.LT_BILLBOARD, "UIBillboard");
        registeLayout(typeof(ScriptCharacter), LAYOUT_TYPE.LT_CHARACTER, "UICharacter");
        registeLayout(typeof(ScriptRoomMenu), LAYOUT_TYPE.LT_ROOM_MENU, "UIRoomMenu");
        registeLayout(typeof(ScriptMahjongHandIn), LAYOUT_TYPE.LT_MAHJONG_HAND_IN, "UIMahjongHandIn");
        registeLayout(typeof(ScriptMahjongDrop), LAYOUT_TYPE.LT_MAHJONG_DROP, "UIMahjongDrop");
        registeLayout(typeof(ScriptAllCharacterInfo), LAYOUT_TYPE.LT_ALL_CHARACTER_INFO, "UIAllCharacterInfo");
        registeLayout(typeof(ScriptDice), LAYOUT_TYPE.LT_DICE, "UIDice");
        registeLayout(typeof(ScriptMahjongBackFrame), LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME, "UIMahjongBackFrame");
        registeLayout(typeof(ScriptPlayerAction), LAYOUT_TYPE.LT_PLAYER_ACTION, "UIPlayerAction");
        registeLayout(typeof(ScriptGameEnding), LAYOUT_TYPE.LT_GAME_ENDING, "UIGameEnding");
        registeLayout(typeof(ScriptAddPlayer), LAYOUT_TYPE.LT_ADD_PLAYER, "UIAddPlayer");
        registeLayout(typeof(ScriptMahjongFrame), LAYOUT_TYPE.LT_MAHJONG_FRAME, "UIMahjongFrame");
        registeLayout(typeof(ScriptJoinRoomDialog), LAYOUT_TYPE.LT_JOIN_ROOM_DIALOG, "UIJoinRoomDialog");
        if (mLayoutManager.getLayoutCount() < (int)LAYOUT_TYPE.LT_MAX)
		{
			UnityUtility.logError("error : not all script added! max count : " + (int)LAYOUT_TYPE.LT_MAX + ", added count :" + mLayoutManager.getLayoutCount());
		}
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	protected void registeLayout(Type script, LAYOUT_TYPE layout, string name)
	{
		mLayoutManager.registeLayout(script, layout, name);
	}
}