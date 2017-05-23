using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SocketPacket : GameBase
{
	public SocketPacket(SOCKET_PACKET type, int dataCount)
	{
		mType = type;
		mDataCount = dataCount;
	}
	public virtual void fillData() { }
	public virtual void readData(byte[] data, int dataSize) { }
	public SOCKET_PACKET getPacketType() { return mType; }
	public byte[] getData() { return mData; }
	public int getSize() { return mDataCount; }
	public virtual void execute() { }

	protected SOCKET_PACKET mType;
	protected byte[] mData;
	protected int mDataCount;
}