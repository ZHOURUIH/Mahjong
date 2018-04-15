using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerAskAction : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public SCOtherPlayerAskAction(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mOtherPlayerGUID);
	}
	public override void execute()
	{
		Character otherPlayer = mCharacterManager.getCharacter(mOtherPlayerGUID.mValue);
		mScriptMahjongFrame.notifyInfo("正在等待玩家" + otherPlayer.getName() + "做出选择");
	}
}