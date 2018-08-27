using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterGetHua : Command
{
	public MAHJONG mMah;
	public override void init()
	{
		base.init();
		mMah = MAHJONG.M_MAX;
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		character.showHua(mMah);
		mScriptMahjongHandIn.notifyShowHua(data.mPosition, data.mHuaList);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ", mahjong : " + mMah;
	}
}