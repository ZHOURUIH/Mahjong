using System;
using System.Collections;
using System.Collections.Generic;

public class CSCreateRoom : SocketPacket
{
	public CSCreateRoom(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{}
}