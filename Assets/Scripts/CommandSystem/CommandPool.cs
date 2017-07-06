using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommandPool : GameBase
{
	protected Dictionary<Type, List<Command>> mInusedList;
	protected Dictionary<Type, List<Command>> mUnusedList;
	protected ThreadLock mInuseLock;
	protected ThreadLock mUnuseLock;
	protected int mNewCount;
	public CommandPool()
	{
		mInusedList = new Dictionary<Type, List<Command>>();
		mUnusedList = new Dictionary<Type, List<Command>>();
		mInuseLock = new ThreadLock();
		mUnuseLock = new ThreadLock();
	}
	public void init()
	{
		;
	}
	public void destroy()
	{
		mInusedList.Clear();
		mUnusedList.Clear();
		mInusedList = null;
		mUnusedList = null;
		GC.Collect();
	}
	public T newCmd<T>(bool show = true, bool delay = false) where T : Command, new()
	{
		// 首先从未使用的列表中获取,获取不到再重新创建一个
		T cmd = null;
		Type t = typeof(T);
		if(mUnusedList.ContainsKey(t))
		{
			if(mUnusedList[t].Count > 0)
			{
				cmd = mUnusedList[t][0] as T;
				// 从未使用列表中移除
				removeUnuse(cmd);
			}
		}
		// 没有找到可以用的,则创建一个
		if(cmd == null)
		{
			cmd = new T();
			cmd.init();
			cmd.setType(typeof(T));
			++mNewCount;
			UnityUtility.logInfo("new cmd : " + mNewCount);
		}
		// 设置为可用命令
		cmd.setValid(true);
		cmd.setShowDebugInfo(show);
		cmd.setDelayCommand(delay);
		// 加入已使用列表
		addInuse(cmd);
		return cmd;
	}
	public void destroyCmd(Command cmd) 
	{
		// 销毁命令时,初始化命令数据,并设置为不可用命令
		cmd.init();
		cmd.setValid(false);
		addUnuse(cmd);
		removeInuse(cmd);
	}
	//------------------------------------------------------------------------------------------------------------------
	protected void addInuse(Command cmd)
	{
		mInuseLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
		// 添加到使用列表中
		Type t = cmd.getType();
		if (!mInusedList.ContainsKey(t))
		{
			mInusedList.Add(t, new List<Command>());
		}
		List<Command> list = mInusedList[t];
		list.Add(cmd);
		mInuseLock.unlock(LOCK_TYPE.LT_WRITE);
	}
	protected void addUnuse(Command cmd)
	{
		mUnuseLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
		// 添加到未使用列表中
		Type t = cmd.getType();
		if (!mUnusedList.ContainsKey(t))
		{
			mUnusedList.Add(t, new List<Command>());
		}
		mUnusedList[t].Add(cmd);
		mUnuseLock.unlock(LOCK_TYPE.LT_WRITE);
	}
	protected void removeInuse(Command cmd)
	{
		mInuseLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
		Type t = cmd.getType();
		if (mInusedList.ContainsKey(t))
		{
			mInusedList[t].Remove(cmd);
		}
		mInuseLock.unlock(LOCK_TYPE.LT_WRITE);
	}
	protected void removeUnuse(Command cmd)
	{
		mUnuseLock.waitForUnlock(LOCK_TYPE.LT_WRITE);
		Type t = cmd.getType();
		if (mUnusedList.ContainsKey(t))
		{
			mUnusedList[t].Remove(cmd);
		}
		mUnuseLock.unlock(LOCK_TYPE.LT_WRITE);
	}
}
