using System;
using System.Collections;
using System.Collections.Generic;

public class CSCreateRoom : SocketPacket
{
	public CSCreateRoom(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		;
	}
	public override void write(byte[] data)
	{
		;
	}
	public override int getSize()
	{
		return 0;
	}
}