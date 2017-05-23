using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum LOCK_TYPE
{
	LT_WRITE,
	LT_READ,
}

public class ThreadLock
{
	public volatile bool mLock;			// 是否锁定
	public volatile LOCK_TYPE mRead;		// 锁定后是想要读(true)或者写(false)
	public volatile int mReadLockCount;	// 锁定读的次数,当锁定读的次数为0时才能完全解锁
	public volatile int mShowLockError;
	public ThreadLock()
	{
		mLock = false;
		mRead = LOCK_TYPE.LT_WRITE;
		mReadLockCount = 0;
		mShowLockError = 0;
	}
	public void waitForUnlock(LOCK_TYPE read, bool showDebug = false)
	{
		while (mLock)
		{
		}
		// 解锁后立即上锁
		mLock = true;

		mLock = true;
		mRead = read;
		if (showDebug)
		{
			UnityUtility.logError("lock : read : " + (mRead == LOCK_TYPE.LT_READ ? "true" : "false") + ", read count : " + mReadLockCount);
		}
	}
	public void unlock(LOCK_TYPE read, bool showDebug = false)
	{
		mLock = false;
		if (showDebug)
		{
			UnityUtility.logError("unlock : read : " + (mRead == LOCK_TYPE.LT_READ ? "true" : "false") + ", read count : " + mReadLockCount);
		}
	}
};