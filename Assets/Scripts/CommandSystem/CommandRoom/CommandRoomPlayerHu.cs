using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandRoomPlayerHu : Command
{
	public Character mHuPlayer;
	public MAHJONG mMahjong;
	public List<HU_TYPE> mHuList;
	public Character mDroppedPlayer;
	public override void init()
	{
		base.init();
		mHuPlayer = null;
		mMahjong = MAHJONG.M_MAX;
		mHuList = null;
		mDroppedPlayer = null;
	}
	public override void execute()
	{
		Room room = mReceiver as Room;
		room.notifyPlayerHu(mHuPlayer, mDroppedPlayer, mHuList, mMahjong);
	}
	public override string showDebugInfo()
	{
		string name = mHuPlayer != null ? mHuPlayer.getName() : "";
		string huStr = "";
		if(mHuList != null)
		{
			int huCount = mHuList.Count;
			for (int i = 0; i < huCount; ++i)
			{
				huStr += mHuList[i];
				huStr += ", ";
			}
			StringUtility.removeLastComma(ref huStr);
		}
		return base.showDebugInfo() + " : mahjong : " + mMahjong + ", player : " + name + ", hu type : " + huStr;
	}
}