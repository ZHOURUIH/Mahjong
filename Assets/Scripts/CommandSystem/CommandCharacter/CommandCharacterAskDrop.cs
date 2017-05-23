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
		}
		// 如果是其他玩家,则在0.3秒后自动打出一张牌
		else
		{
			CommandCharacterAutoDrop cmdAutoDrop = new CommandCharacterAutoDrop(true, true);
			mCommandSystem.pushDelayCommand(cmdAutoDrop, character, 0.3f);
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo();
	}
}