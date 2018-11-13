using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class USHORT : OBJECT
{
	public ushort mValue;
	public USHORT()
	{
		mType = typeof(ushort);
		mSize = sizeof(ushort);
	}
	public USHORT(ushort value)
	{
		mValue = value;
		mType = typeof(ushort);
		mSize = sizeof(ushort);
	}
	public override void zero()
	{
		mValue = 0;
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		mValue = readUShort(buffer, ref index);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeUShort(buffer, ref index, mValue);
	}
}