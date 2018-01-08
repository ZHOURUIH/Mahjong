using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class ThreadLock
{
	protected int mLockCount = 0;         // 是否锁定
	public ThreadLock()
	{
		mLockCount = 0;
	}
	public void waitForUnlock()
	{
		while (Interlocked.Exchange(ref mLockCount, 1) != 0){}
	}
	public void unlock()
	{
		Interlocked.Exchange(ref mLockCount, 0);
	}
};