using System;
using System.Collections;
using System.Collections.Generic;

public class SCReadyRet : SocketPacket
{
	public BOOL mReady = new BOOL();
	public SCReadyRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mReady);
	}
	public override void execute()
	{
		CommandCharacterNotifyReady cmd = mCommandSystem.newCmd<CommandCharacterNotifyReady>();
		cmd.mReady = mReady.mValue;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getMyself());
	}
}