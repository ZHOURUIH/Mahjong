using System;
using System.Collections;
using System.Collections.Generic;

public class CSDiceDone : SocketPacket
{
	public CSDiceDone(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{ }
}