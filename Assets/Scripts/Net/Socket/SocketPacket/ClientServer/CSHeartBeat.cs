using System;
using System.Collections;
using System.Collections.Generic;

public class CSHeartBeat : SocketPacket
{
	public int mHeartBeatTimes;
	public CSHeartBeat(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mHeartBeatTimes = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mHeartBeatTimes);
	}
	public override int getSize()
	{
		return sizeof(int);
	}
}