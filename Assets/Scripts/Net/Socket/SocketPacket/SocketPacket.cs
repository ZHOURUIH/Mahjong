using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class SocketPacket : GameBase
{
	protected PACKET_TYPE mType;
	public SocketPacket()
	{
		;
	}
	public SocketPacket(PACKET_TYPE type)
	{
		mType = type;
	}
	public PACKET_TYPE getPacketType() { return mType; }
	// 如果是服务器向客户端发送的消息,则需要重写该函数
	public virtual void execute() { }
	// 从data中读取消息参数
	public abstract void read(byte[] data);
	// 将消息参数写入data
	public abstract void write(byte[] data);
	public abstract int getSize();
}