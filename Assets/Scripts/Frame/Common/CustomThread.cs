using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// 返回值表示是否继续运行该线程
public delegate bool CustomThreadCallback();

public class CustomThread
{
	protected bool mRunning;
	protected bool mFinish;
	protected bool mPause;
	protected CustomThreadCallback mCallback;
	protected Thread mThread;
	protected ThreadTimeLock mTimeLock;
	protected string mName;
	protected bool mIsBackground;	// 在终止线程时是否强制终止,false则在终止线程时等待线程执行结束再终止,true则表示终止线程时强制终止
	public CustomThread(string name)
	{
		mName = name;
		mFinish = true;
		mRunning = false;
		mCallback = null;
		mThread = null;
		mTimeLock = null;
		mPause = false;
		mIsBackground = false;
	}
	public void destroy()
	{
		stop();
	}
	public void setBackground(bool background)
	{
		mIsBackground = background;
		if(mThread != null)
		{
			mThread.IsBackground = mIsBackground;
		}
	}
	public void start(CustomThreadCallback callback, int frameTimeMS = 15)
	{
		UnityUtility.logInfo("准备启动线程 : " + mName, LOG_LEVEL.LL_FORCE);
		if (mThread != null)
		{
			return;
		}
		mTimeLock = new ThreadTimeLock(frameTimeMS);
		mRunning = true;
		mCallback = callback;
		mThread = new Thread(run);
		mThread.Name = mName;
		mThread.IsBackground = mIsBackground;
		mThread.Start();
		UnityUtility.logInfo("线程启动成功 : " + mName, LOG_LEVEL.LL_FORCE);
	}
	public void pause(bool isPause)
	{
		mPause = isPause;
	}
	public void stop()
	{
		UnityUtility.logInfo("准备退出线程 : " + mName, LOG_LEVEL.LL_FORCE);
		mRunning = false;
		while (!mIsBackground && !mFinish) { }
		if (mThread != null)
		{
			mThread.Abort();
			mThread = null;
		}
		mCallback = null;
		mTimeLock = null;
		mPause = false;
		UnityUtility.logInfo("线程退出完成! 线程名 : " + mName, LOG_LEVEL.LL_FORCE);
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
				if(!mCallback())
				{
					break;
				}
			}
			catch (Exception e)
			{
				UnityUtility.logInfo("捕获线程异常! 线程名 : " + mName + ", " + e.Message + ", " + e.StackTrace, LOG_LEVEL.LL_FORCE);
			}
		}
		mFinish = true;
	}
}