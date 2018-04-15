using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterGet : Command
{
	public MAHJONG mMahjong;
	public override void init()
	{
		base.init();
		mMahjong = MAHJONG.M_MAX;
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		character.getMahjong(mMahjong);
		CharacterData data = character.getCharacterData();
		// 通知布局
		mScriptMahjongHandIn.notifyGetMahjong(data.mPosition, mMahjong);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mahjong : " + mMahjong;
	}
}