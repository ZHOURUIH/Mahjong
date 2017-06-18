using System;
using System.Collections;
using System.Collections.Generic;

public class SCRequestDropRet : SocketPacket
{
	protected BYTE mIndex = new BYTE();
	protected BYTE mMahjong = new BYTE();
	public SCRequestDropRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mIndex);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		;
	}
}