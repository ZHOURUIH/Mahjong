using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterDrop : Command
{
	public MAHJONG mMah;
	public int mIndex;
	public CommandCharacterDrop(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		character.dropMahjong(mMah);

		ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
		handIn.notifyDropMahjong(data.mPosition, mMah, mIndex);
		ScriptMahjongDrop scriptDrop = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_DROP) as ScriptMahjongDrop;
		scriptDrop.notifyDropMahjong(data.mPosition, data.mDropList, mMah);
		if (character.getType() == CHARACTER_TYPE.CT_MYSELF)
		{
			ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
			mahjongFrame.notifyInfo("");

			// 打出一张牌后,锁定玩家手里的牌,玩家不能点击手里的麻将
			handIn.notifyCanDrop(false);
			// 确认麻将操作按钮已经隐藏
			ScriptPlayerAction playerAction = mLayoutManager.getScript(LAYOUT_TYPE.LT_PLAYER_ACTION) as ScriptPlayerAction;
			playerAction.notifyActionAsk(null);
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ", mahjong : " + mMah;
	}
}