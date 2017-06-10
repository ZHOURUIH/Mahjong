using System;
using System.Collections;
using System.Collections.Generic;

public class SCReadyRet : SocketPacket
{
	protected bool mReady;
	public SCReadyRet(PACKET_TYPE type)
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
		return sizeof(byte);
	}
	public override void execute()
	{
		CommandCharacterNotifyReady cmd = new CommandCharacterNotifyReady();
		cmd.mReady = mReady;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getMyself());
	}
}