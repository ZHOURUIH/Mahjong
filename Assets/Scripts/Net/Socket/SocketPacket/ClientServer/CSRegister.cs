using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSRegister : SocketPacket
{
	protected byte[] mAccount = new byte[16];
	protected byte[] mPassword = new byte[16];
	protected byte[] mName = new byte[16];
	protected int mHead;
	public CSRegister(PACKET_TYPE type)
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
	public void setName(string name)
	{
		BinaryUtility.memset<byte>(mName, 0);
		byte[] nameByte = BinaryUtility.stringToBytes(name, Encoding.UTF8);
		BinaryUtility.memcpy(mName, nameByte, 0, 0, MathUtility.getMin(mName.Length, nameByte.Length));
	}
	public void setHead(int head)
	{
		mHead = head;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		BinaryUtility.readBytes(data, ref index, -1, mAccount, -1, -1);
		BinaryUtility.readBytes(data, ref index, -1, mPassword, -1, -1);
		BinaryUtility.readBytes(data, ref index, -1, mName, -1, -1);
		mHead = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeBytes(data, ref index, -1, mAccount, -1, -1);
		BinaryUtility.writeBytes(data, ref index, -1, mPassword, -1, -1);
		BinaryUtility.writeBytes(data, ref index, -1, mName, -1, -1);
		BinaryUtility.writeInt(data, ref index, mHead);
	}
	public override int getSize()
	{
		return mAccount.Length * sizeof(byte) + mPassword.Length * sizeof(byte) + mName.Length * sizeof(byte) + sizeof(int);
	}
}