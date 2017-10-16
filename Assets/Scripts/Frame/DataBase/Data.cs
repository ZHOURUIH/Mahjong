using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Data
{
	protected DATA_TYPE mType;
	public Data(DATA_TYPE type)
	{
		mType = type;
	}
	public DATA_TYPE getType() { return mType; }
	public virtual void read(byte[] data, int dataCount) { }
	public virtual int getDataSize() { return 0; }
};