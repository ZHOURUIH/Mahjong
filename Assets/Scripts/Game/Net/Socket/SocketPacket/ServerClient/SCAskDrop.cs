using System;
using System.Collections;
using System.Collections.Generic;

public class SCAskDrop : SocketPacket
{
	public SCAskDrop(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{ }
	public override void execute()
	{
		pushCommand<CommandCharacterAskDrop>(mCharacterManager.getMyself());
	}
}