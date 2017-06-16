using System;
using System.Collections;
using System.Collections.Generic;

public class CSReady : SocketPacket
{
	protected BOOL mReady = new BOOL();
	public CSReady(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	public void setReady(bool ready) { mReady.mValue = ready; }
	protected override void fillParams()
	{
		pushParam(mReady);
	}
}