using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class ThreadLock
{
	protected int mLockCount = 0;         // 是否锁定
	protected string mFileName;
	protected int mLine;
	public ThreadLock()
	{
		mLockCount = 0;
	}
	public void waitForUnlock()
	{
		while (Interlocked.Exchange(ref mLockCount, 1) != 0){}
		mFileName = UnityUtility.getCurSourceFileName(2);
		mLine = UnityUtility.getLineNum(2);
	}
	public void unlock()
	{
		Interlocked.Exchange(ref mLockCount, 0);
		mFileName = "";
		mLine = 0;
	}
};