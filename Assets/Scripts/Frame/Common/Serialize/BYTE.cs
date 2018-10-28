using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BYTE : OBJECT
{
	public byte mValue;
	public BYTE()
	{
		mType = typeof(byte);
		mSize = sizeof(byte);
	}
	public BYTE(byte value)
	{
		mValue = value;
		mType = typeof(byte);
		mSize = sizeof(byte);
	}
	public override void zero()
	{
		mValue = 0;
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		mValue = readByte(buffer, ref index);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeByte(buffer, ref index, mValue);
	}
}