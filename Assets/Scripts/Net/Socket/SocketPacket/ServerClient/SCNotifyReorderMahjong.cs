using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyReorderMahjong : SocketPacket
{
	protected INT mPlayerGUID = new INT();
	public SCNotifyReorderMahjong(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
	}
	public override void execute()
	{
		CommandCharacterReorderMahjong cmd = new CommandCharacterReorderMahjong();
		mCommandSystem.pushCommand(cmd, mCharacterManager.getCharacterByGUID(mPlayerGUID.mValue));
	}
}