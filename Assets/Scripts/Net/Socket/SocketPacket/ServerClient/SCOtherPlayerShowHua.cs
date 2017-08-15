using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerShowHua : SocketPacket
{
	public BYTE mIndex = new BYTE();
	public BYTE mMahjong = new BYTE();
    public INT mOtherPlayerGUID = new INT();
    public SCOtherPlayerShowHua(PACKET_TYPE type)
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
        pushParam(mOtherPlayerGUID);
	}
	public override void execute()
	{
		CommandCharacterShowHua cmdShowHua = mCommandSystem.newCmd<CommandCharacterShowHua>();
		cmdShowHua.mIndex = mIndex.mValue;
		cmdShowHua.mMah = (MAHJONG)mMahjong.mValue;
		mCommandSystem.pushCommand(cmdShowHua, mCharacterManager.getCharacterByGUID(mOtherPlayerGUID.mValue));
	}
}