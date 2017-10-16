using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSConfirmAction : SocketPacket
{
	public BYTE mAction = new BYTE();
	public CSConfirmAction(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mAction);
	}
}