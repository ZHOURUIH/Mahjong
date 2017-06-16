using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FLOATS : OBJECT
{
	public float[] mValue;
	public FLOATS()
	{
		mType = typeof(float[]);
		mSize = 0;
	}
	public FLOATS(float[] value)
	{
		mValue = value;
		mType = typeof(float[]);
		mSize = sizeof(float) * mValue.Length;
	}
	public override void zero()
	{
		BinaryUtility.memset(mValue, 0.0f);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.readFloats(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.writeFloats(buffer, ref index, mValue);
	}
	public void setValue(float[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		BinaryUtility.memcpy(mValue, value, 0, 0, minCount);
	}
}