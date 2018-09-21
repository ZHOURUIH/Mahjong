using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using UnityEngine.SceneManagement;

public class LocalLog
{
	protected CustomThread mWriteLogThread;
	// 日志双缓冲,使用双缓冲可以快速进行前后台切换,避免数据同步时出现耗时操作
	protected List<string>[] mLogBufferList;
	protected int mLogIndex;
	protected int mWriteIndex;
	protected ThreadLock mLogListLock;
	protected string mLogFilePath;
	public LocalLog()
	{
		mLogIndex = 0;
		mWriteIndex = 1;
		mLogBufferList = new List<string>[2];
		mLogBufferList[mLogIndex] = new List<string>();
		mLogBufferList[mWriteIndex] = new List<string>();
		mLogListLock = new ThreadLock();
		mWriteLogThread = new CustomThread("WriteLocalLog");
		mLogFilePath = CommonDefine.F_ASSETS_PATH + "log.txt";
		// 清空已经存在的日志文件
		FileUtility.writeTxtFile(mLogFilePath, "");
	}
	public void init()
	{
		mWriteLogThread.start(writeLocalLog);
	}
	public void destroy()
	{
		mWriteLogThread.destroy();
		// 线程停止后仍然需要保证将列表中的全部日志信息写入文件
		writeLogToFile();
	}
	public void log(string info)
	{
		// 将日志保存到当前缓冲中
		mLogListLock.waitForUnlock();
		mLogBufferList[mLogIndex].Add(info);
		mLogListLock.unlock();
	}
	//-----------------------------------------------------------------------------------------------------------
	protected bool writeLocalLog()
	{
		// 将当前写入缓冲区中的内容写入文件
		writeLogToFile();
		return true;
	}
	protected void writeLogToFile()
	{
		// 切换缓冲区
		mLogListLock.waitForUnlock();
		MathUtility.swap(ref mLogIndex, ref mWriteIndex);
		mLogListLock.unlock();
		string totalString = "";
		int count = mLogBufferList[mWriteIndex].Count;
		if (count > 0)
		{
			for (int i = 0; i < count; ++i)
			{
				totalString += mLogBufferList[mWriteIndex][i] + "\r\n";
			}
			FileUtility.writeTxtFile(mLogFilePath, totalString, true);
			mLogBufferList[mWriteIndex].Clear();
		}
	}
}