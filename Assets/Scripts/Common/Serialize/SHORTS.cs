using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SHORTS : OBJECT
{
	public short[] mValue;
	public SHORTS()
	{
		mType = typeof(short[]);
		mSize = 0;
	}
	public SHORTS(short[] value)
	{
		mValue = value;
		mType = typeof(short[]);
		mSize = sizeof(short) * mValue.Length;
	}
	public override void zero()
	{
		BinaryUtility.memset(mValue, (short)0);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.readShorts(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.writeShorts(buffer, ref index, mValue);
	}
	public void setValue(short[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		BinaryUtility.memcpy(mValue, value, 0, 0, minCount);
	}
}