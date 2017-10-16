using UnityEngine;
using System.Collections;

public class CommandCharacterNotifyReady : Command
{
	public bool mReady;
	public override void init()
	{
		base.init();
		mReady = false;
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		if(data.mReady == mReady)
		{
			return;
		}
		data.mReady = mReady;
		// 通知布局
		ScriptAllCharacterInfo allInfo = mLayoutManager.getScript(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO) as ScriptAllCharacterInfo;
		allInfo.notifyCharacterReady(character, mReady);
		// 如果是自己的准备状态改变
		if(character.getType() == CHARACTER_TYPE.CT_MYSELF)
		{
			ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
			mahjongFrame.notifyReady(mReady);
		}
	}
}