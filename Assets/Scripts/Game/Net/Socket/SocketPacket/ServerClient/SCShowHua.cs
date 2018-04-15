using System;
using System.Collections;
using System.Collections.Generic;

public class SCShowHua : SocketPacket
{
	public BYTE mIndex = new BYTE();
	public BYTE mMahjong = new BYTE();
    public SCShowHua(PACKET_TYPE type)
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
		CommandCharacterShowHua cmdShowHua = newCmd(out cmdShowHua);
		cmdShowHua.mIndex = mIndex.mValue;
		cmdShowHua.mMah = (MAHJONG)mMahjong.mValue;
		pushCommand(cmdShowHua, mCharacterManager.getMyself());
	}
}