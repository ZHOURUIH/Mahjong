using System;
using System.Collections;
using System.Collections.Generic;

public class SCReadyRet : SocketPacket
{
	public BOOL mReady = new BOOL();
	public SCReadyRet(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mReady);
	}
	public override void execute()
	{
		CommandCharacterNotifyReady cmd = newCmd(out cmd);
		cmd.mReady = mReady.mValue;
		pushCommand(cmd, mCharacterManager.getMyself());
	}
}