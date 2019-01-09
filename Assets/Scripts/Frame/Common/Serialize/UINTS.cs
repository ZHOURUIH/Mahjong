using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UINTS : OBJECT
{
	public uint[] mValue;
	public UINTS()
	{
		mType = typeof(uint[]);
		mSize = 0;
	}
	public UINTS(int count)
	{
		mValue = new uint[count];
		mType = typeof(uint[]);
		mSize = sizeof(uint) * mValue.Length;
	}
	public UINTS(uint[] value)
	{
		mValue = value;
		mType = typeof(uint[]);
		mSize = sizeof(uint) * mValue.Length;
	}
	public override void zero()
	{
		memset(mValue, (uint)0);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		readUInts(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeUInts(buffer, ref index, mValue);
	}
	public void setValue(int[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		memcpy(mValue, value, 0, 0, minCount);
	}
}