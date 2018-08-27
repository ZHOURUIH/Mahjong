using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerOffline : SocketPacket
{
	public INT mPlayerGUID = new INT();		// 离线的玩家GUID
	public SCOtherPlayerOffline(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
	}
	public override void execute()
	{
		UnityUtility.logInfo("有玩家离线, id : " + mPlayerGUID.mValue);
	}
}