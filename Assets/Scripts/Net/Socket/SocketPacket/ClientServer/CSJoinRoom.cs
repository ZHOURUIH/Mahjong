using System;
using System.Collections;
using System.Collections.Generic;

public class CSJoinRoom : SocketPacket
{
	protected INT mRoomID = new INT();
	public CSJoinRoom(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	public void setRoomID(int id) { mRoomID.mValue = id; }
	protected override void fillParams()
	{
		pushParam(mRoomID);
	}
}