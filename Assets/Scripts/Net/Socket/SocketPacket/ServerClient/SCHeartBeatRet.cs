using System;
using System.Collections;
using System.Collections.Generic;

public class SCHeartBeatRet : SocketPacket
{
	protected INT mHeartBeatTimes = new INT();
	public SCHeartBeatRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mHeartBeatTimes);
	}
	public override void execute()
	{
		UnityUtility.logInfo("心跳 : " + mHeartBeatTimes.mValue);
		mSocketNetManager.notifyHeartBeatRet(mHeartBeatTimes.mValue);
	}
}