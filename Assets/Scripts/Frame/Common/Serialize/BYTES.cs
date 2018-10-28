using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BYTES : OBJECT
{
	public byte[] mValue;
	public BYTES()
	{
		mType = typeof(byte[]);
		mSize = 0;
	}
	public BYTES(int count)
	{
		mValue = new byte[count];
		mType = typeof(byte[]);
		mSize = sizeof(byte) * mValue.Length;
	}
	public BYTES(byte[] value)
	{
		mValue = value;
		mType = typeof(byte[]);
		mSize = sizeof(byte) * mValue.Length;
	}
	public override void zero()
	{
		memset(mValue, (byte)0);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		readBytes(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeBytes(buffer, ref index, mValue);
	}
	public void setValue(byte[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		memcpy(mValue, value, 0, 0, minCount);
	}
}