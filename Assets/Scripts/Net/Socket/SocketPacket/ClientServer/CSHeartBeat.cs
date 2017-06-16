using System;
using System.Collections;
using System.Collections.Generic;

public class CSHeartBeat : SocketPacket
{
	protected INT mHeartBeatTimes = new INT();
	public CSHeartBeat(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	public void setHeartBeatTimes(int times) { mHeartBeatTimes.mValue = times; }
	protected override void fillParams()
	{
		pushParam(mHeartBeatTimes);
	}
}