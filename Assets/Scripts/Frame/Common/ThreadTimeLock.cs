using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class ThreadTimeLock
{
	protected int mFrameTimeMS;
	protected DateTime mLastTime;
	protected int mForceSleep;          // 每帧无暂停时间时强制暂停的毫秒数,避免线程单帧任务繁重时,导致单帧消耗时间大于设定的固定单帧时间时,CPU占用过高的问题
	public ThreadTimeLock(int frameTimeMS)
	{
		mFrameTimeMS = frameTimeMS;
		mForceSleep = 0;
	}
	public void setForceSleep(int timeMS)
	{
		mForceSleep = timeMS;
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
		else if(mForceSleep > 0)
		{
			Thread.Sleep(mForceSleep);
		}
		mLastTime = curTime;
	}
};