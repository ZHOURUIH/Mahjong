using System;
using System.Collections;
using System.Collections.Generic;

public class CSHeartBeat : SocketPacket
{
	public INT mHeartBeatTimes = new INT();
	public CSHeartBeat(PACKET_TYPE type)
		: base(type) { }
	public void setHeartBeatTimes(int times) { mHeartBeatTimes.mValue = times; }
	protected override void fillParams()
	{
		pushParam(mHeartBeatTimes);
	}
}