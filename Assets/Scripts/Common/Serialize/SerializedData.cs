using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class SerializedData : GameBase
{
	protected List<OBJECT> mParameterInfoList;
	protected int mDataSize;
	public SerializedData()
	{
		mDataSize = 0;
		mParameterInfoList = new List<OBJECT>();
	}
	// 从buffer中读取数据到所有参数中
	public void read(byte[] buffer)
	{
		int bufferOffset = 0;
		int parameterCount = mParameterInfoList.Count;
		for (int i = 0; i < parameterCount; ++i)
		{
			mParameterInfoList[i].readFromBuffer(buffer, ref bufferOffset);
		}
	}
	// 将所有参数的值写入buffer
	public void write(byte[] buffer)
	{
		int bufferOffset = 0;
		int parameterCount = mParameterInfoList.Count;
		for (int i = 0; i < parameterCount; ++i)
		{
			mParameterInfoList[i].writeToBuffer(buffer, ref bufferOffset);
		}
	}
	public int getSize() { return mDataSize; }
	//----------------------------------------------------------------------------------------------------------------------------------------
	// 在子类构造中调用
	protected abstract void fillParams();
	// 在子类构造中调用
	protected void zeroParams()
	{
		mDataSize = 0;
		int count = mParameterInfoList.Count;
		for (int i = 0; i < count; ++i)
		{
			mParameterInfoList[i].zero();
			mDataSize += mParameterInfoList[i].mSize;
		}
	}
	protected void pushParam(OBJECT param)
	{
		mParameterInfoList.Add(param);
	}
};