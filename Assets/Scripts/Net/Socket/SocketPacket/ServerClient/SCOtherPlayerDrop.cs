using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerDrop : SocketPacket
{
	protected INT mPlayerGUID = new INT();
	protected INT mIndex = new INT();
	protected BYTE mMahjong = new BYTE();
	public SCOtherPlayerDrop(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
		pushParam(mIndex);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		;
	}
}