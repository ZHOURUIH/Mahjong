using System;
using System.Collections;
using System.Collections.Generic;

public class CSReady : SocketPacket
{
	public bool mReady;
	public CSReady(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mReady = BinaryUtility.readBool(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeBool(data, ref index, mReady);
	}
	public override int getSize()
	{
		return sizeof(bool);
	}
}