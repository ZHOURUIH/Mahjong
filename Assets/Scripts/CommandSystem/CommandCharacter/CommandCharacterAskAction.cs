using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 询问玩家是否确定要碰杠胡
public class CommandCharacterAskAction : Command
{
	public List<MahjongAction> mActionList;
	public CommandCharacterAskAction(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		// 如果是自己则通知布局
		if (character.getType() == CHARACTER_TYPE.CT_MYSELF)
		{
			ScriptPlayerAction playerAction = mLayoutManager.getScript(LAYOUT_TYPE.LT_PLAYER_ACTION) as ScriptPlayerAction;
			playerAction.notifyActionAsk(mActionList);
		}
		// 其他玩家则请求麻将系统做出第一个操作
		else
		{
			if(mActionList[0].mType == ACTION_TYPE.AT_HU)
			{
				CommandMahjongSystemRequestHu cmd = new CommandMahjongSystemRequestHu();
				cmd.mCharacter = character;
				mCommandSystem.pushCommand(cmd, mMahjongSystem);
			}
			else if(mActionList[0].mType == ACTION_TYPE.AT_GANG)
			{
				CommandMahjongSystemRequestGang cmd = new CommandMahjongSystemRequestGang();
				cmd.mCharacter = character;
				mCommandSystem.pushCommand(cmd, mMahjongSystem);
			}
			else if(mActionList[0].mType == ACTION_TYPE.AT_PENG)
			{
				CommandMahjongSystemRequestPeng cmd = new CommandMahjongSystemRequestPeng();
				cmd.mCharacter = character;
				mCommandSystem.pushCommand(cmd, mMahjongSystem);
			}
		}
	}
	public override string showDebugInfo()
	{
		string str = "";
		int count = mActionList.Count;
		for(int i = 0; i < count; ++i)
		{
			str += ", " + mActionList[i];
		}
		return base.showDebugInfo() + " : " + str;
	}
}