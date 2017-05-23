using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PacketInfo
{
	public PacketInfo(SOCKET_PACKET type, Type classType, int dataCount)
	{
		mType = type;
		mClassType = classType;
		mDataCount = dataCount;
	}
	public SOCKET_PACKET mType;
	public Type mClassType;
	public int mDataCount;
}

public class SocketFactory
{
	protected Dictionary<SOCKET_PACKET, PacketInfo> mSocketPacketTypeList = new Dictionary<SOCKET_PACKET, PacketInfo>();
	public void init()
	{
		;
	}
	public SocketPacket createPacket(SOCKET_PACKET type)
	{
		if (mSocketPacketTypeList.ContainsKey(type))
		{
			PacketInfo info = mSocketPacketTypeList[type];
			object[] param = new object[] { info.mType, info.mDataCount };  //构造器参数
			SocketPacket pack = UnityUtility.createInstance<SocketPacket>(mSocketPacketTypeList[type].mClassType, param);
			return pack;
		}
		return null;
	}
	public int getPacketSize(SOCKET_PACKET type)
	{
		return mSocketPacketTypeList[type].mDataCount;
	}
	public SOCKET_PACKET getSocketType(byte[] buff, int bufflength)
	{
		return SOCKET_PACKET.SP_MAX;
	}
	//-------------------------------------------------------------------------------------------------------------------------------
	protected void registerPacket<T>(SOCKET_PACKET type, int dataCount)
	{
		PacketInfo info = new PacketInfo(type, typeof(T), dataCount);
		mSocketPacketTypeList.Add(type, info);
	}
}