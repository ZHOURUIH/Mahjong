using System;
using System.Collections;
using System.Collections.Generic;

public class CSCheckName : SocketPacket
{
	public byte[] mName = new byte[16];
	public CSCheckName(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		BinaryUtility.readBytes(data, ref index, -1, mName, -1, -1);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeBytes(data, ref index, -1, mName, -1, -1);
	}
	public override int getSize()
	{
		return sizeof(byte) * mName.Length;
	}
}