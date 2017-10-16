using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SocketFactory
{
	protected PACKET_TYPE mType;
	protected Type mClassType;
	protected int mPacketSize;
	public SocketFactory(Type classType, PACKET_TYPE type)
	{
		mType = type;
		mClassType = classType;
		SocketPacket packet = createPacket();
		mPacketSize = packet.getSize();
	}
	public SocketPacket createPacket()
	{
		object[] param = new object[] { mType };  //构造器参数
		SocketPacket pack = UnityUtility.createInstance<SocketPacket>(mClassType, param);
		return pack;
	}
	public PACKET_TYPE getType() { return mType; }
	public int getPacketSize() { return mPacketSize; }
}