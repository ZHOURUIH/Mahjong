using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BOOLS : OBJECT
{
	public bool[] mValue;
	public BOOLS()
	{
		mType = typeof(bool[]);
		mSize = 0;
	}
	public BOOLS(int count)
	{
		mValue = new bool[count];
		mType = typeof(bool[]);
		mSize = sizeof(bool) * mValue.Length;
	}
	public BOOLS(bool[] value)
	{
		mValue = value;
		mType = typeof(bool[]);
		mSize = sizeof(bool) * mValue.Length;
	}
	public override void zero()
	{
		BinaryUtility.memset(mValue, false);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.readBools(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.writeBools(buffer, ref index, mValue);
	}
	public void setValue(bool[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		BinaryUtility.memcpy(mValue, value, 0, 0, minCount);
	}
}