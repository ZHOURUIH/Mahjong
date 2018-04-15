using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 询问玩家是否确定要碰杠胡
public class CommandCharacterAskAction : Command
{
	public List<MahjongAction> mActionList = null;
	public override void init()
	{
		base.init();
		mActionList = null;
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		// 如果是自己则通知布局
		if (character.getType() == CHARACTER_TYPE.CT_MYSELF)
		{
			mScriptPlayerAction.notifyActionAsk(mActionList);
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