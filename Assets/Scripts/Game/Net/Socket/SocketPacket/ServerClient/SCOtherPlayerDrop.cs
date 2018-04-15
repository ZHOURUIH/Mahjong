using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerDrop : SocketPacket
{
	public INT mPlayerGUID = new INT();
	public INT mIndex = new INT();
	public BYTE mMahjong = new BYTE();
	public SCOtherPlayerDrop(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
		pushParam(mIndex);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		// 清空提示信息
		mScriptMahjongFrame.notifyInfo("");
		CommandCharacterDrop cmd = newCmd(out cmd);
		cmd.mMah = (MAHJONG)mMahjong.mValue;
		cmd.mIndex = mIndex.mValue;
		pushCommand(cmd, mCharacterManager.getCharacter(mPlayerGUID.mValue));
	}
}