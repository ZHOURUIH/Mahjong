using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyGetStartMahjong : SocketPacket
{
	protected INT mPlayerGUID = new INT();
	protected BYTE mMahjong = new BYTE();
	public SCNotifyGetStartMahjong(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		CommandCharacterGetStart cmd = new CommandCharacterGetStart();
		cmd.mMahjong = (MAHJONG)mMahjong.mValue;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getCharacterByGUID(mPlayerGUID.mValue));
	}
}