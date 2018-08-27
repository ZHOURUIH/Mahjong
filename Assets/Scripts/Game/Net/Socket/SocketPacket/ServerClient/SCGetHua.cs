using System;
using System.Collections;
using System.Collections.Generic;

public class SCGetHua : SocketPacket
{
	public BYTE mMahjong = new BYTE();
    public SCGetHua(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mMahjong);
	}
	public override void execute()
	{
		CommandCharacterGetHua cmdShowHua = newCmd(out cmdShowHua);
		cmdShowHua.mMah = (MAHJONG)mMahjong.mValue;
		pushCommand(cmdShowHua, mCharacterManager.getMyself());
	}
}