using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSRequestDrop : SocketPacket
{
	public BYTE mIndex = new BYTE();
	public CSRequestDrop(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mIndex);
	}
}