using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyReorderMahjong : SocketPacket
{
	protected int mPlayerGUID;
	public SCNotifyReorderMahjong(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mPlayerGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mPlayerGUID);
	}
	public override int getSize()
	{
		return sizeof(int);
	}
	public override void execute()
	{
		CommandCharacterReorderMahjong cmd = new CommandCharacterReorderMahjong();
		mCommandSystem.pushCommand(cmd, mCharacterManager.getCharacterByGUID(mPlayerGUID));
	}
}