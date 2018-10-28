using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class INT : OBJECT
{
	public int mValue;
	public INT()
	{
		mType = typeof(int);
		mSize = sizeof(int);
	}
	public INT(int value)
	{
		mValue = value;
		mType = typeof(int);
		mSize = sizeof(int);
	}
	public override void zero()
	{
		mValue = 0;
	}
	public override void readFromBuffer(byte[] buffer, ref int index)
	{
		mValue = readInt(buffer, ref index);
	}
	public override void writeToBuffer(byte[] buffer, ref int index)
	{
		writeInt(buffer, ref index, mValue);
	}
}