using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSRequestDrop : SocketPacket
{
	protected BYTE mIndex = new BYTE();
	public CSRequestDrop(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	public void setIndex(byte index)
	{
		mIndex.mValue = index;
	}
	protected override void fillParams()
	{
		pushParam(mIndex);
	}
}