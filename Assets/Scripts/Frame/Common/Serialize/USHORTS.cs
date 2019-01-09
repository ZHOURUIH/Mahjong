using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class USHORTS : OBJECT
{
	public ushort[] mValue;
	public USHORTS()
	{
		mType = typeof(ushort[]);
		mSize = 0;
	}
	public USHORTS(int count)
	{
		mValue = new ushort[count];
		mType = typeof(ushort[]);
		mSize = sizeof(ushort) * mValue.Length;
	}
	public USHORTS(ushort[] value)
	{
		mValue = value;
		mType = typeof(ushort[]);
		mSize = sizeof(ushort) * mValue.Length;
	}
	public override void zero()
	{
		memset(mValue, (ushort)0);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		readUShorts(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeUShorts(buffer, ref index, mValue);
	}
	public void setValue(ushort[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		memcpy(mValue, value, 0, 0, minCount);
	}
}