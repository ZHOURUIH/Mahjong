using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyBanker : SocketPacket
{
	protected int mGUID;		// 庄家ID
	public SCNotifyBanker(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mGUID);
	}
	public override int getSize()
	{
		return sizeof(int);
	}
	public override void execute()
	{
		UnityUtility.logInfo("玩家" + mGUID + "成为庄家");
	}
}