using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterGet : Command
{
	public MAHJONG mMahjong;
	public CommandCharacterGet(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		character.getMahjong(mMahjong);
		CharacterData data = character.getCharacterData();
		// 通知布局
		ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
		handIn.notifyGetMahjong(data.mPosition, mMahjong);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mahjong : " + mMahjong;
	}
}