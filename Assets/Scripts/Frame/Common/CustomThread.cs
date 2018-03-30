using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public delegate void CustomThreadCallback();

public class CustomThread
{
	protected bool mRunning;
	protected bool mFinish;
	protected bool mPause;
	protected CustomThreadCallback mCallback;
	protected Thread mThread;
	protected ThreadTimeLock mTimeLock;
	protected string mName;
	public CustomThread(string name)
	{
		mName = name;
		mFinish = true;
		mRunning = false;
		mCallback = null;
		mThread = null;
		mTimeLock = null;
		mPause = false;
	}
	public void destroy()
	{
		stop();
	}
	public void start(CustomThreadCallback callback, int frameTimeMS = 15)
	{
		if(mThread != null)
		{
			return;
		}
		mTimeLock = new ThreadTimeLock(frameTimeMS);
		mRunning = true;
		mCallback = callback;
		mThread = new Thread(run);
		mThread.Start();
	}
	public void pause(bool isPause)
	{
		mPause = isPause;
	}
	public void stop()
	{
		mRunning = false;
		while (!mFinish) { }
		if (mThread != null)
		{
			mThread.Abort();
			mThread = null;
		}
		mCallback = null;
		mTimeLock = null;
		mPause = false;
	}
	protected void run()
	{
		mFinish = false;
		while (mRunning)
		{
			mTimeLock.update();
			try
			{
				if(mPause)
				{
					continue;
				}
				mCallback();
			}
			catch (Exception e)
			{
				UnityUtility.logInfo("捕获线程异常! 线程名 : " + mName + ", " + e.Message + ", " + e.StackTrace, LOG_LEVEL.LL_FORCE);
			}
		}
		mFinish = true;
		UnityUtility.logInfo("线程退出完成! 线程名 : " + mName, LOG_LEVEL.LL_FORCE);
	}
};