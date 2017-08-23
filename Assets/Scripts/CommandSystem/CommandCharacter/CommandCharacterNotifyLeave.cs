using UnityEngine;
using System.Collections;

public class CommandCharacterNotifyLeave : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		character.clearRoomData();
		character.clearMahjongData();
	}
}