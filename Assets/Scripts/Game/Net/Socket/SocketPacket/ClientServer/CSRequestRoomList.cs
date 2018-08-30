using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSRequestRoomList : SocketPacket
{
	public SHORT mMinIndex = new SHORT();
	public SHORT mMaxIndex = new SHORT();
	public CSRequestRoomList(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mMinIndex);
		pushParam(mMaxIndex);
	}
}