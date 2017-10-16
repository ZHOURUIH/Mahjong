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
		ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
		handIn.notifyEnd(data.mPosition, data.mHandIn);
	}
}