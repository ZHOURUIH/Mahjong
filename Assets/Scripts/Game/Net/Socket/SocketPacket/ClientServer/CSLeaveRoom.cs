using System;
using System.Collections;
using System.Collections.Generic;

public class CSLeaveRoom : SocketPacket
{
	public CSLeaveRoom(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams(){}
}