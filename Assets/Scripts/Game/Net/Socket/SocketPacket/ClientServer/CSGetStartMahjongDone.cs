using System;
using System.Collections;
using System.Collections.Generic;

public class CSGetStartMahjongDone : SocketPacket
{
	public CSGetStartMahjongDone(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{}
}