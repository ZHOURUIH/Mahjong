using System;
using System.Collections;
using System.Collections.Generic;

public class CSLogin : SocketPacket
{
	protected BYTES mAccount = new BYTES(16);
	protected BYTES mPassword = new BYTES(16);
	public CSLogin(PACKET_TYPE type)
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
	public void setPassword(string password)
	{
		byte[] passwordBytes = BinaryUtility.stringToBytes(password);
		mPassword.setValue(passwordBytes);
	}
	protected override void fillParams()
	{
		pushParam(mAccount);
		pushParam(mPassword);
	}
}