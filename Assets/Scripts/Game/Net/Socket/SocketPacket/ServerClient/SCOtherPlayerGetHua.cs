using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerGetHua : SocketPacket
{
	public BYTE mMahjong = new BYTE();
    public INT mOtherPlayerGUID = new INT();
    public SCOtherPlayerGetHua(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mMahjong);
        pushParam(mOtherPlayerGUID);
	}
	public override void execute()
	{
		CommandCharacterGetHua cmdShowHua = newCmd(out cmdShowHua);
		cmdShowHua.mMah = (MAHJONG)mMahjong.mValue;
		pushCommand(cmdShowHua, mCharacterManager.getCharacter(mOtherPlayerGUID.mValue));
	}
}