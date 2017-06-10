using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerReady : SocketPacket
{
	protected bool mReady;		// 是否已准备
	protected int mPlayerGUID;	// 玩家GUID
	public SCOtherPlayerReady(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mReady = BinaryUtility.readBool(data, ref index);
		mPlayerGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeBool(data, ref index, mReady);
		BinaryUtility.writeInt(data, ref index, mPlayerGUID);
	}
	public override int getSize()
	{
		return sizeof(byte) + sizeof(int);
	}
	public override void execute()
	{
		CommandCharacterNotifyReady cmd = new CommandCharacterNotifyReady();
		cmd.mReady = mReady;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getCharacterByGUID(mPlayerGUID));
	}
}