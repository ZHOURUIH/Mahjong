using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSContinueGame : SocketPacket
{
	public BOOL mContinue = new BOOL();
	public CSContinueGame(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mContinue);
	}
}