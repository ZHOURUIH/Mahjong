using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class ThreadTimeLock
{
	protected int mFrameTimeMS;
	protected DateTime mLastTime;
	public ThreadTimeLock(int frameTimeMS)
	{
		mFrameTimeMS = frameTimeMS;
	}
	public void update()
	{
		DateTime curTime = DateTime.Now;
		TimeSpan delta = curTime - mLastTime;
		int deltaMS = mFrameTimeMS - (int)delta.TotalMilliseconds;
		if(deltaMS > 0)
		{
			Thread.Sleep(deltaMS);
		}
		mLastTime = DateTime.Now;
	}
};