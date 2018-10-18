using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class SocketPacket : SerializedData
{
	protected PACKET_TYPE mType;
	protected SocketConnect mConnect;
	public SocketPacket(PACKET_TYPE type)
	{
		mType = type;
	}
	public void setConnect(SocketConnect connect)
	{
		mConnect = connect;
	}
	public virtual void init()
	{
		fillParams();
		zeroParams();
	}
	public PACKET_TYPE getPacketType() { return mType; }
	// 如果是服务器向客户端发送的消息,则需要重写该函数
	public virtual void execute() { }
}