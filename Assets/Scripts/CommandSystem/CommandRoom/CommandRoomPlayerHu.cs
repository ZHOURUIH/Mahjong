using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandRoomPlayerHu : Command
{
	public Character mPlayer;
	public List<HU_TYPE> mHuList;
	public override void init()
	{
		base.init();
		mHuList = null;
		mPlayer = null;
	}
	public override void execute()
	{
		Room room = mReceiver as Room;
		room.notifyPlayerHu(mPlayer, mHuList);
	}
}