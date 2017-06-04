using System;
using System.Collections;
using System.Collections.Generic;

public class SCCheckAccountRet : SocketPacket
{
	protected byte mResult;  // 0表示成功,1表示失败
	public SCCheckAccountRet(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mResult = BinaryUtility.readByte(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeByte(data, ref index, mResult);
	}
	public override int getSize()
	{
		return sizeof(byte) + sizeof(int);
	}
	public override void execute()
	{
		// 创建房间成功,进入麻将场景
		if(mResult == 0)
		{
			UnityUtility.logInfo("成功!");
		}
		else
		{
			UnityUtility.logInfo("失败!");
		}
	}
}