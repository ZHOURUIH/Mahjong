using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerJoinRoom : SocketPacket
{
	protected int mGUID;		// 加入房间的玩家GUID
	public SCOtherPlayerJoinRoom(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mGUID);
	}
	public override int getSize()
	{
		return sizeof(int);
	}
	public override void execute()
	{
		UnityUtility.logInfo("有其他玩家加入房间, id : " + mGUID);
	}
}