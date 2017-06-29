using System;
using System.Collections;
using System.Collections.Generic;

public class CSJoinRoom : SocketPacket
{
	public INT mRoomID = new INT();
	public CSJoinRoom(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mRoomID);
	}
}