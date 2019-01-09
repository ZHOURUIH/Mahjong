using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UINT : OBJECT
{
	public uint mValue;
	public UINT()
	{
		mType = typeof(uint);
		mSize = sizeof(uint);
	}
	public UINT(uint value)
	{
		mValue = value;
		mType = typeof(uint);
		mSize = sizeof(uint);
	}
	public override void zero()
	{
		mValue = 0;
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		mValue = readUInt(buffer, ref index);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeUInt(buffer, ref index, mValue);
	}
}