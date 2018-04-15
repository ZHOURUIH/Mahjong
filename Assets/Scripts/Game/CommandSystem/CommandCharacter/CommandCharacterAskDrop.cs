using UnityEngine;
using System.Collections;

// 通知玩家需要打一张牌
public class CommandCharacterAskDrop : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		// 如果是玩家自己,则通知布局可以打出一张牌
		if(character.getType() == CHARACTER_TYPE.CT_MYSELF)
		{
			mScriptMahjongHandIn.notifyCanDrop(true);
			mScriptMahjongFrame.notifyInfo("请打出一张牌");
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo();
	}
}