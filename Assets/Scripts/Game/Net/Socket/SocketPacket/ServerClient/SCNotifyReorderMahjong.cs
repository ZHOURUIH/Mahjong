using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyReorderMahjong : SocketPacket
{
	public INT mPlayerGUID = new INT();
	public SCNotifyReorderMahjong(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
	}
	public override void execute()
	{
		pushCommand<CommandCharacterReorderMahjong>(mCharacterManager.getCharacter(mPlayerGUID.mValue));
	}
}