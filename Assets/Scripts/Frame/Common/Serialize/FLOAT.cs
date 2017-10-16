using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FLOAT : OBJECT
{
	public float mValue;
	public FLOAT()
	{
		mType = typeof(float);
		mSize = sizeof(float);
	}
	public FLOAT(float value)
	{
		mValue = value;
		mType = typeof(float);
		mSize = sizeof(float);
	}
	public override void zero()
	{
		mValue = 0.0f;
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		mValue = BinaryUtility.readFloat(buffer, ref index);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.writeFloat(buffer, ref index, mValue);
	}
}