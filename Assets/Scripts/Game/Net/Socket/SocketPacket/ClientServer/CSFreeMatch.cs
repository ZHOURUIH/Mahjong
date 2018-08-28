using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSFreeMatch : SocketPacket
{
	public CSFreeMatch(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		;
	}
}