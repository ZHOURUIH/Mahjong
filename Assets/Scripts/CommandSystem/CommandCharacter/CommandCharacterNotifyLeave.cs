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
		CharacterData data = character.getCharacterData();
		data.mReady = false;
		data.mBanker = false;
		data.mServerPosition = PLAYER_POSITION.PP_MAX;
		data.mPosition = PLAYER_POSITION.PP_MAX;
		data.mPengGangList.Clear();
		data.mHandIn.Clear();
		data.mDropList.Clear();
		data.mHuaList.Clear();
		data.mRoomID = -1;
	}
}