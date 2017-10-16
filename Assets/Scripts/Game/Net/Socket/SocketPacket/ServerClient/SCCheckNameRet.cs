using System;
using System.Collections;
using System.Collections.Generic;

public class SCCheckNameRet : SocketPacket
{
	public BYTE mResult = new BYTE();  // 0表示成功,1表示失败
	public SCCheckNameRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mResult);
	}
	public override void execute()
	{
		// 创建房间成功,进入麻将场景
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