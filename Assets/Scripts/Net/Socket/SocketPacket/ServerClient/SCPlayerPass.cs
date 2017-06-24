using System;
using System.Collections;
using System.Collections.Generic;

public class SCPlayerPass : SocketPacket
{
	protected INT mDroppedPlayerGUID = new INT();
	protected BYTE mMahjong = new BYTE();
	public SCPlayerPass(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mDroppedPlayerGUID);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		;
	}
}