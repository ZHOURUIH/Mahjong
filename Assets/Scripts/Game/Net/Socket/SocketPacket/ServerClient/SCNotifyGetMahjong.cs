using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyGetMahjong : SocketPacket
{
	public INT mPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public SCNotifyGetMahjong(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		CommandCharacterGet cmd = newCmd(out cmd);
		cmd.mMahjong = (MAHJONG)mMahjong.mValue;
		pushCommand(cmd, mCharacterManager.getCharacter(mPlayerGUID.mValue));
	}
}