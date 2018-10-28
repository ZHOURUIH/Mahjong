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
	public FLOATS(int count)
	{
		mValue = new float[count];
		mType = typeof(float[]);
		mSize = sizeof(float) * mValue.Length;
	}
	public FLOATS(float[] value)
	{
		mValue = value;
		mType = typeof(float[]);
		mSize = sizeof(float) * mValue.Length;
	}
	public override void zero()
	{
		memset(mValue, 0.0f);
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		readFloats(buffer, ref index, mValue);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeFloats(buffer, ref index, mValue);
	}
	public void setValue(float[] value)
	{
		int minCount = value.Length < mValue.Length ? value.Length : mValue.Length;
		memcpy(mValue, value, 0, 0, minCount);
	}
}