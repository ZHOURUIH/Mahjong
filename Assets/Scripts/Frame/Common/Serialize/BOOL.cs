using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BOOL : OBJECT
{
	public bool mValue;
	public BOOL()
	{
		mType = typeof(bool);
		mSize = sizeof(bool);
	}
	public BOOL(bool value)
	{
		mValue = value;
		mType = typeof(bool);
		mSize = sizeof(bool);
	}
	public override void zero()
	{
		mValue = false;
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		mValue = BinaryUtility.readBool(buffer, ref index);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		BinaryUtility.writeBool(buffer, ref index, mValue);
	}
}