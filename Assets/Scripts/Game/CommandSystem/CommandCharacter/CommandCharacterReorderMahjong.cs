using UnityEngine;
using System.Collections;

public class CommandCharacterReorderMahjong : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		character.reorderMahjong();
		// 通知布局麻将被重新排列
		CharacterData data = character.getCharacterData();
		mScriptMahjongHandIn.notifyReorder(data.mPosition, data.mHandIn);
	}
}