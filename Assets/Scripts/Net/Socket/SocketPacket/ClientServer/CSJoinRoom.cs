using System;
using System.Collections;
using System.Collections.Generic;

public class CSJoinRoom : SocketPacket
{
	public int mRoomID;
	public CSJoinRoom(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mRoomID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mRoomID);
	}
	public override int getSize()
	{
		return sizeof(int);
	}
}