using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerPass : SocketPacket
{
	protected INT mOtherPlayerGUID = new INT();
	protected INT mDroppedPlayerGUID = new INT();
	protected BYTE mMahjong = new BYTE();
	public SCOtherPlayerPass(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mOtherPlayerGUID);
		pushParam(mDroppedPlayerGUID);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		;
	}
}