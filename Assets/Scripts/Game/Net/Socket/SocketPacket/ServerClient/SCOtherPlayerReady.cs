using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerReady : SocketPacket
{
	public BOOL mReady = new BOOL();		// 是否已准备
	public INT mPlayerGUID = new INT();	// 玩家GUID
	public SCOtherPlayerReady(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mReady);
		pushParam(mPlayerGUID);
	}
	public override void execute()
	{
		CommandCharacterNotifyReady cmd = mCommandSystem.newCmd<CommandCharacterNotifyReady>();
		cmd.mReady = mReady.mValue;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getCharacter(mPlayerGUID.mValue));
	}
}