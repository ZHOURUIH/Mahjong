using System;
using System.Collections;
using System.Collections.Generic;

public class SCStartGame : SocketPacket
{
	public SCStartGame(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		;
	}
	public override void write(byte[] data)
	{
		;
	}
	public override int getSize()
	{
		return 0;
	}
	public override void execute()
	{
		UnityUtility.logInfo("开始游戏");
	}
}