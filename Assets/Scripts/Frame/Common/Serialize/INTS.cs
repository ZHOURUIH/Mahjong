using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class INTS : OBJECT
{
	public int[] mValue;
	public INTS()
	{
		mType = typeof(int[]);
		mSize = 0;
	}
	public INTS(int count)
	{
		mValue = new int[count];
		mType = typeof(int[]);
		mSize = sizeof(int) * mValue.Length;
	}
	public INTS(int[] value)
	{
		mValue = value;
		mType = typeof(int[]);
		mSize = sizeof(int) * mValue.Length;
	}
	public override void zero()
	{
		memset(mValue, 0);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		readInts(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeInts(buffer, ref index, mValue);
	}
	public void setValue(int[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		memcpy(mValue, value, 0, 0, minCount);
	}
}