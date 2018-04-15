using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterShowHua : Command
{
	public MAHJONG mMah;
	public int mIndex;
	public override void init()
	{
		base.init();
		mMah = MAHJONG.M_MAX;
		mIndex = 0;
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		character.showHua(mMah);
		mScriptMahjongHandIn.notifyShowHua(data.mPosition, mMah, mIndex);
		mScriptMahjongHandIn.notifyShowHua(data.mPosition, data.mHuaList);

		// 打一张牌以后需要重新排列
		pushCommand<CommandCharacterReorderMahjong>(character);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ", mahjong : " + mMah;
	}
}