using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandRoomMahjongPoolSize : Command
{
	public int mCount;
	public override void init()
	{
		base.init();
		mCount = 0;
	}
	public override void execute()
	{
		Room room = mReceiver as Room;
		room.setMahjongPoolSize(mCount);
		mScriptMahjongFrame.setMahjongPoolSize(mCount);
	}
}