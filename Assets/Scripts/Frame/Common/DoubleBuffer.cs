using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// 双缓冲,线程安全的缓冲区,可在不同的线程进行读写数据
public class DoubleBuffer<T> : GameBase
{
	protected List<T>[] mBufferList;
	protected int mWriteIndex;
	protected int mReadIndex;
	protected ThreadLock mBufferLock;
	public DoubleBuffer()
	{
		mBufferList = new List<T>[2];
		mBufferList[0] = new List<T>();
		mBufferList[1] = new List<T>();
		mWriteIndex = 0;
		mReadIndex = 1;
		mBufferLock = new ThreadLock();
	}
	// 切换缓冲区,获得可读列表,在遍历可读列表期间,不能再次调用getReadList,否则会出现不可预知的错误,并且该函数只能在一个线程中调用
	public List<T> getReadList()
	{
		mBufferLock.waitForUnlock();
		swap(ref mReadIndex, ref mWriteIndex);
		mBufferLock.unlock();
		return mBufferList[mReadIndex];
	}
	// 向可写列表中添加数据,可在不同线程中调用
	public void addToBuffer(T value)
	{
		mBufferLock.waitForUnlock();
		mBufferList[mWriteIndex].Add(value);
		mBufferLock.unlock();
	}
}