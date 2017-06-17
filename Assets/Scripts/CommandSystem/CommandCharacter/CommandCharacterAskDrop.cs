using UnityEngine;
using System.Collections;

// 通知玩家需要打一张牌
public class CommandCharacterAskDrop : Command
{
	public CommandCharacterAskDrop(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = mReceiver as Character;
		// 如果是玩家自己,则通知布局可以打出一张牌
		if(character.getType() == CHARACTER_TYPE.CT_MYSELF)
		{
			ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
			handIn.notifyCanDrop(true);
			ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
			mahjongFrame.notifyInfo("请打出一张牌");
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo();
	}
}