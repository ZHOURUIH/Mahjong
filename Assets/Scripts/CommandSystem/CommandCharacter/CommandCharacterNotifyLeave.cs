using UnityEngine;
using System.Collections;

public class CommandCharacterNotifyLeave : Command
{
	public CommandCharacterNotifyLeave(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		data.mReady = false;
		data.mBanker = false;
		data.mServerPosition = PLAYER_POSITION.PP_MAX;
		data.mPosition = PLAYER_POSITION.PP_MAX;
		for (int i = 0; i < CommonDefine.MAX_PENG_TIMES; ++i)
		{
			data.mPengGangList[i].mType = ACTION_TYPE.AT_MAX;
			data.mPengGangList[i].mMahjong = MAHJONG.M_MAX;
		}
		data.mHandIn.Clear();
		data.mDropList.Clear();
		data.mRoomID = -1;
	}
}