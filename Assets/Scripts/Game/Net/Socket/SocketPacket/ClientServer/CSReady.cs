using System;
using System.Collections;
using System.Collections.Generic;

public class CSReady : SocketPacket
{
	public BOOL mReady = new BOOL();
	public CSReady(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mReady);
	}
}