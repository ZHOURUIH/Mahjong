using System;
using System.Collections;
using System.Collections.Generic;

public class SCAddMahjongRobotRet : SocketPacket
{
	public BOOL mResult = new BOOL();
	public SCAddMahjongRobotRet(PACKET_TYPE type)
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
		if(mResult.mValue)
		{
			logInfo("添加机器人成功");
		}
		else
		{
			logInfo("添加机器人失败");
		}
	}
}