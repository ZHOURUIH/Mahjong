using System;
using System.Collections;
using System.Collections.Generic;

public class SCRequestDropRet : SocketPacket
{
	protected BYTE mIndex = new BYTE();
	protected BYTE mMahjong = new BYTE();
	public SCRequestDropRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mIndex);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		CommandCharacterDrop cmd = new CommandCharacterDrop();
		cmd.mMah = (MAHJONG)mMahjong.mValue;
		cmd.mIndex = mIndex.mValue;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getMyself());
	}
}