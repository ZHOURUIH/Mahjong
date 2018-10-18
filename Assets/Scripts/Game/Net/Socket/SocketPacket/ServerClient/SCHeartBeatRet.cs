using System;
using System.Collections;
using System.Collections.Generic;

public class SCHeartBeatRet : SocketPacket
{
	public INT mHeartBeatTimes = new INT();
	public SCHeartBeatRet(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mHeartBeatTimes);
	}
	public override void execute()
	{
		UnityUtility.logInfo("心跳 : " + mHeartBeatTimes.mValue);
		mConnect.notifyHeartBeatRet(mHeartBeatTimes.mValue);
	}
}