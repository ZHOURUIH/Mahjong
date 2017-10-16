using System;
using System.Collections;
using System.Collections.Generic;

public class CSCheckAccount : SocketPacket
{
	public BYTES mAccount = new BYTES(16);
	public CSCheckAccount(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	public void setAccount(string account)
	{
		byte[] accountBytes = BinaryUtility.stringToBytes(account);
		mAccount.setValue(accountBytes);
	}
	protected override void fillParams()
	{
		pushParam(mAccount);
	}
}