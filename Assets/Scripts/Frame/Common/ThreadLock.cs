using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public enum LOCK_TYPE
{
	LT_WRITE,
	LT_READ,
}

public class ThreadLock
{
	public volatile int mLockCount = 0;         // 是否锁定
	public volatile LOCK_TYPE mRead;        // 锁定后是想要读(true)或者写(false)
	public volatile int mReadLockCount; // 锁定读的次数,当锁定读的次数为0时才能完全解锁
	public volatile int mShowLockError;
	public ThreadLock()
	{
		mLockCount = 0;
		mRead = LOCK_TYPE.LT_WRITE;
		mReadLockCount = 0;
		mShowLockError = 0;
	}
	public void waitForUnlock(LOCK_TYPE read = LOCK_TYPE.LT_WRITE, bool showDebug = false)
	{
		while (Interlocked.Exchange(ref mLockCount, 1) != 0)
		{
		}
		mRead = read;
		if (showDebug)
		{
			UnityUtility.logError("lock : read : " + (mRead == LOCK_TYPE.LT_READ ? "true" : "false") + ", read count : " + mReadLockCount);
		}
	}
	public void unlock(LOCK_TYPE read = LOCK_TYPE.LT_WRITE, bool showDebug = false)
	{
		Interlocked.Exchange(ref mLockCount, 0);
		if (showDebug)
		{
			UnityUtility.logError("unlock : read : " + (mRead == LOCK_TYPE.LT_READ ? "true" : "false") + ", read count : " + mReadLockCount);
		}
	}
};