using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterDrop : Command
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
		character.dropMahjong(mMah);

		mScriptMahjongHandIn.notifyDropMahjong(data.mPosition, mMah, mIndex);
		mScriptMahjongDrop.notifyDropMahjong(data.mPosition, data.mDropList, mMah);
		if (character.getType() == CHARACTER_TYPE.CT_MYSELF)
		{
			mScriptMahjongFrame.notifyInfo("");

			// 打出一张牌后,锁定玩家手里的牌,玩家不能点击手里的麻将
			mScriptMahjongHandIn.notifyCanDrop(false);
			// 确认麻将操作按钮已经隐藏
			mScriptPlayerAction.notifyActionAsk(null);
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ", mahjong : " + mMah;
	}
}