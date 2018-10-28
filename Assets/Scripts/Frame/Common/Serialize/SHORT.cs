using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SHORT : OBJECT
{
	public short mValue;
	public SHORT()
	{
		mType = typeof(short);
		mSize = sizeof(short);
	}
	public SHORT(short value)
	{
		mValue = value;
		mType = typeof(short);
		mSize = sizeof(short);
	}
	public override void zero()
	{
		mValue = 0;
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		mValue = readShort(buffer, ref index);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeShort(buffer, ref index, mValue);
	}
}