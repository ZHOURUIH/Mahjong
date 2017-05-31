using System;
using System.Collections;
using System.Collections.Generic;

public class SCJoinRoomRet : SocketPacket
{
	protected byte mResult;  // 0表示成功,1表示失败
	protected int mRoomID;
	public SCJoinRoomRet(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mResult = BinaryUtility.readByte(data, ref index);
		mRoomID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeByte(data, ref index, mResult);
		BinaryUtility.writeInt(data, ref index, mRoomID);
	}
	public override int getSize()
	{
		return sizeof(byte) + sizeof(int);
	}
	public override void execute()
	{
		if(mResult == 0)
		{
			UnityUtility.logInfo("加入房间成功, 房间ID:" + mRoomID);
		}
		else
		{
			UnityUtility.logInfo("加入房间失败, 原因:" + mResult);
		}
	}
}