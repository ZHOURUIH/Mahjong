using System;
using System.Collections;
using System.Collections.Generic;

public class CSCheckName : SocketPacket
{
	public BYTES mName = new BYTES(16);
	public CSCheckName(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	public void setName(string name)
	{
		byte[] nameBytes = BinaryUtility.stringToBytes(name);
		mName.setValue(nameBytes);
	}
	protected override void fillParams()
	{
		pushParam(mName);
	}
}