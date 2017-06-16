using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyGetStartMahjong : SocketPacket
{
	protected int mPlayerGUID;      // 庄家ID
	protected byte mMahjong;
	public SCNotifyGetStartMahjong(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mPlayerGUID = BinaryUtility.readInt(data, ref index);
		mMahjong = BinaryUtility.readByte(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mPlayerGUID);
		BinaryUtility.writeByte(data, ref index, mMahjong);
	}
	public override int getSize()
	{
		return sizeof(int) + sizeof(byte);
	}
	public override void execute()
	{
		CommandCharacterGetStart cmd = new CommandCharacterGetStart();
		cmd.mMahjong = (MAHJONG)mMahjong;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getCharacterByGUID(mPlayerGUID));
	}
}