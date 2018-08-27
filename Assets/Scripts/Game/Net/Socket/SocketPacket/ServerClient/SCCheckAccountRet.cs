using System;
using System.Collections;
using System.Collections.Generic;

public class SCCheckAccountRet : SocketPacket
{
	public BYTE mResult = new BYTE();  // 0表示成功,1表示失败
	public SCCheckAccountRet(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mResult);
	}
	public override void execute()
	{
		if(mResult.mValue == 0)
		{
			UnityUtility.logInfo("成功!");
		}
		else
		{
			UnityUtility.logInfo("失败!");
		}
	}
}