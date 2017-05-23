using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandSystem
{
	public List<DelayCommand> mCommandBufferProcess = new List<DelayCommand>();	// 用于处理的命令列表
	public List<DelayCommand> mCommandBufferInput = new List<DelayCommand>();		// 用于放入命令的命令列表
	public bool mLockBuffer;
	public bool mShowDebugInfo;
	public class DelayCommand
	{
		public DelayCommand(float delayTime, Command cmd, CommandReceiver receiver)
		{
			mDelayTime = delayTime;
			mCommand = cmd;
			mReceiver = receiver;
		}
		public float mDelayTime;
		public Command mCommand;
		public CommandReceiver mReceiver;
	};
	public CommandSystem()
	{
		mLockBuffer = false;
		mShowDebugInfo = true;
	}
	public virtual void init(bool showDebug = true)
	{
		mShowDebugInfo = showDebug;
	}
	public void setShowDebugInfo(bool show)
	{
		mShowDebugInfo = show;
		UnityUtility.logInfo("debug info : " + mShowDebugInfo);
	}
	public bool getShowDebugInfo() { return mShowDebugInfo; }
	protected void syncCommandBuffer()
	{
		// 等待解锁缓冲区
		waitUnlockBuffer();
		// 锁定缓冲区
		lockBuffer();
		int inputCount = mCommandBufferInput.Count;
		for (int i = 0; i < inputCount; ++i)
		{
			mCommandBufferProcess.Add(mCommandBufferInput[i]);
		}
		mCommandBufferInput.Clear();
		// 解锁缓冲区
		unlockBuffer();
	}
	public void update(float elapsedTime)
	{
		// 同步命令输入列表到命令处理列表中
		syncCommandBuffer();

		// 如果一帧时间大于1秒,则认为是无效更新
		if (elapsedTime >= 1.0f)
		{
			return;
		}
		List<DelayCommand> executeList = new List<DelayCommand>();
		// 开始处理命令处理列表
		for (int i = 0; i < mCommandBufferProcess.Count; ++i)
		{
			mCommandBufferProcess[i].mDelayTime -= elapsedTime;
			if (mCommandBufferProcess[i].mDelayTime <= 0.0f)
			{
				// 命令的延迟执行时间到了,则执行命令
				executeList.Add(mCommandBufferProcess[i]);
				mCommandBufferProcess.Remove(mCommandBufferProcess[i]);
				--i;
			}
		}
		foreach(var cmd in executeList)
		{
			cmd.mCommand.setDelayCommand(false);
			pushCommand(cmd.mCommand, cmd.mReceiver);
		}
	}
	//中断命令
	public bool interruptCommand(Command cmd)
	{
		if (cmd == null)
		{
			UnityUtility.logError("cmd is null");
			return false;
		}
		if (!cmd.isDelayCommand())
		{
			UnityUtility.logError("cmd is not a delay command");
			return false;
		}
		// 中断命令之前需要同步延迟命令列表
		syncCommandBuffer();
		foreach (var item in mCommandBufferProcess)
		{
			if (item.mCommand == cmd)
			{
				UnityUtility.logInfo("CommandSystem : interrupt command : " + cmd.showDebugInfo() + "receiver : " + item.mReceiver.getName());
				mCommandBufferProcess.Remove(item);
				return true;
			}
		}
		return false;
	}
	//执行命令
	public void pushCommand(Command cmd, CommandReceiver cmdReceiver)
	{
		if (cmd == null || cmdReceiver == null)
		{
			UnityUtility.logError("error : cmd or receiver is null!");
			return;
		}
		if(cmd.isDelayCommand())
		{
			UnityUtility.logError("error : cmd is a delay cmd! can not use pushCommand!");
			return;
		}
		cmd.setReceiver(cmdReceiver);
		if (mShowDebugInfo && cmd.getShowDebugInfo())
		{
			UnityUtility.logInfo("CommandSystem : " + cmd.showDebugInfo() + ", receiver : " + cmdReceiver.getName());				
		}
		cmdReceiver.receiveCommand(cmd);
	}
	// delayExecute是命令延时执行的时间,默认为0,只有new出来的命令才能延时执行
	// 子线程中发出的命令必须是延时执行的命令!
	public void pushDelayCommand(Command cmd, CommandReceiver cmdReceiver, float delayExecute = 0.001f)
	{
		if (cmd == null || cmdReceiver == null)
		{
			UnityUtility.logError("error : cmd or receiver is null!");
			return;
		}
		if (delayExecute < 0.0f)
		{
			delayExecute = 0.0f;
		}
		if (mShowDebugInfo && cmd.getShowDebugInfo())
		{
			UnityUtility.logInfo("CommandSystem : delay cmd : " + delayExecute + ", info : " + cmd.showDebugInfo() + ", receiver : " + cmdReceiver.getName());
		}
		if (cmd.isDelayCommand())
		{
			DelayCommand delayCommand = new DelayCommand(delayExecute, cmd, cmdReceiver);

			// 等待解锁缓冲区
			waitUnlockBuffer();
			// 锁定缓冲区
			lockBuffer();
			mCommandBufferInput.Add(delayCommand);
			// 解锁缓冲区
			unlockBuffer();
		}
		else
		{
			UnityUtility.logError("error : cmd is not a delay command, Command : " + cmd.showDebugInfo());
		}
	}
	public void destroy()
	{
		mCommandBufferInput.Clear();
		mCommandBufferProcess.Clear();
	}
	public void waitUnlockBuffer()
	{
		while (mLockBuffer)
		{ }
	}
	public void lockBuffer() { mLockBuffer = true; }
	public void unlockBuffer() { mLockBuffer = false; }
	public virtual void notifyReceiverDestroied(CommandReceiver receiver)
	{
		// 等待解锁缓冲区
		waitUnlockBuffer();
		// 锁定缓冲区
		lockBuffer();
		for (int i = 0; i < mCommandBufferInput.Count; ++i)
		{
			if (mCommandBufferInput[i].mReceiver == receiver)
			{
				// 命令的延迟执行时间到了,则执行命令
				mCommandBufferInput.Remove(mCommandBufferInput[i]);
				--i;
			}
		}
		// 解锁缓冲区
		unlockBuffer();

		for (int i = 0; i < mCommandBufferProcess.Count; ++i)
		{
			if (mCommandBufferProcess[i].mReceiver == receiver)
			{
				// 命令的延迟执行时间到了,则执行命令
				mCommandBufferProcess.Remove(mCommandBufferProcess[i]);
				--i;
			}
		}
	}
}