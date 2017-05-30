using System;
using System.Collections;
using System.Collections.Generic;

public class CSLogin : SocketPacket
{
	protected byte[] mAccount = new byte[16];
	protected byte[] mPassword = new byte[16];
	public CSLogin(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public void setAccount(string account)
	{
		BinaryUtility.memset<byte>(mAccount, 0);
		BinaryUtility.memcpy(mAccount, account.ToCharArray(), 0, 0, MathUtility.getMin(mAccount.Length, account.Length));
	}
	public void setPassword(string password)
	{
		BinaryUtility.memset<byte>(mPassword, 0);
		BinaryUtility.memcpy(mPassword, password.ToCharArray(), 0, 0, MathUtility.getMin(mPassword.Length, password.Length));
	}
	public override void read(byte[] data)
	{
		int index = 0;
		BinaryUtility.readBytes(data, ref index, -1, mAccount, -1, -1);
		BinaryUtility.readBytes(data, ref index, -1, mPassword, -1, -1);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeBytes(data, ref index, -1, mAccount, -1, -1);
		BinaryUtility.writeBytes(data, ref index, -1, mPassword, -1, -1);
	}
	public override int getSize()
	{
		return mAccount.Length * sizeof(byte) + mPassword.Length * sizeof(byte);
	}
}