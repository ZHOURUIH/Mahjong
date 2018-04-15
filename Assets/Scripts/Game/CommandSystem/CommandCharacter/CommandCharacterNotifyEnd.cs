using UnityEngine;
using System.Collections;

// 通知玩家需要打一张牌
public class CommandCharacterNotifyEnd : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		mScriptMahjongHandIn.notifyEnd(data.mPosition, data.mHandIn);
	}
}