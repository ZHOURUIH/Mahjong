using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerPass : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
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