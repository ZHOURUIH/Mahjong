using System;
using System.Collections;
using System.Collections.Generic;

public class CSLogin : SocketPacket
{
	public BYTES mAccount = new BYTES(16);
	public BYTES mPassword = new BYTES(16);
	public CSLogin(PACKET_TYPE type)
		: base(type) { }
	public void setAccount(string account)
	{
		byte[] accountBytes = stringToBytes(account);
		mAccount.setValue(accountBytes);
	}
	public void setPassword(string password)
	{
		byte[] passwordBytes = stringToBytes(password);
		mPassword.setValue(passwordBytes);
	}
	protected override void fillParams()
	{
		pushParam(mAccount);
		pushParam(mPassword);
	}
}