using UnityEngine;
using System.Collections;

public class CommandCharacterTakeDrop : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		character.takeDrop();
		CharacterData data = character.getCharacterData();
		// 通知布局
		mScriptMahjongDrop.notifyTakeDroppedMahjong(data.mPosition, data.mDropList.Count);
	}
}