using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayCommand
{
	public float mDelayTime;
	public Command mCommand;
	public CommandReceiver mReceiver;
	public DelayCommand(float delayTime, Command cmd, CommandReceiver receiver)
	{
		mDelayTime = delayTime;
		mCommand = cmd;
		mReceiver = receiver;
	}
};

public class CommandSystem
{
	protected CommandPool mCommandPool;
	protected List<DelayCommand> mCommandBufferProcess = new List<DelayCommand>();	// 用于处理的命令列表
	protected List<DelayCommand> mCommandBufferInput = new List<DelayCommand>();		// 用于放入命令的命令列表
	protected ThreadLock mBufferLock;
	protected bool mTraceCommand;	// 是否追踪命令的来源
	public CommandSystem()
	{
		mBufferLock = new ThreadLock();
		mTraceCommand = false;
		mCommandPool = new CommandPool();
	}
	public virtual void init(bool showDebug = true)
	{
		mCommandPool.init();
	}
	protected void syncCommandBuffer()
	{
		mBufferLock.waitForUnlock(LOCK_TYPE.LT_READ);
		int inputCount = mCommandBufferInput.Count;
		for (int i = 0; i < inputCount; ++i)
		{
			mCommandBufferProcess.Add(mCommandBufferInput[i]);
		}
		mCommandBufferInput.Clear();
		mBufferLock.unlock(LOCK_TYPE.LT_READ);
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
	// 创建命令
	public T newCmd<T>(bool show = true, bool delay = false) where T : Command, new()
	{
		T cmd = mCommandPool.newCmd<T>(show, delay);
#if UNITY_EDITOR
		if(mTraceCommand)
		{
			int line = 0;
			string file = "";
			int frame = 2;
			while (true)
			{
				file = UnityUtility.getCurSourceFileName(frame);
				line = UnityUtility.getLineNum(frame);
				if (!file.EndsWith("LayoutTools.cs"))
				{
					break;
				}
				++frame;
			}
			cmd.mLine = line;
			cmd.mFile = file;
		}
#endif
		return cmd;
	}
	// 中断命令
	public bool interruptCommand(int assignID)
	{
		if(assignID < 0)
		{
			UnityUtility.logError("assignID invalid!");
			return false;
		}
		syncCommandBuffer();
		foreach (var item in mCommandBufferProcess)
		{
			if (item.mCommand.mAssignID == assignID)
			{
				UnityUtility.logInfo("CommandSystem : interrupt command : " + item.mCommand.showDebugInfo() + ", receiver : " + item.mReceiver.getName(), LOG_LEVEL.LL_HIGH);
				mCommandBufferProcess.Remove(item);
				// 销毁回收命令
				mCommandPool.destroyCmd(item.mCommand);
				return true;
			}
		}
		UnityUtility.logError("not find cmd with assignID!");
		return false;
	}
	// 执行命令
	public void pushCommand(Command cmd, CommandReceiver cmdReceiver)
	{
		if (cmd == null || cmdReceiver == null)
		{
			UnityUtility.logError("cmd or receiver is null!");
			return;
		}
		if(!cmd.isValid())
		{
			UnityUtility.logError("cmd is invalid! make sure create cmd use CommandSystem.newCmd!");
			return;
		}
		if(cmd.isDelayCommand())
		{
			UnityUtility.logError("cmd is a delay cmd! can not use pushCommand!");
			return;
		}
		cmd.setReceiver(cmdReceiver);
		if (cmd.getShowDebugInfo())
		{
			UnityUtility.logInfo("CommandSystem : " + cmd.showDebugInfo() + ", receiver : " + cmdReceiver.getName(), LOG_LEVEL.LL_NORMAL);				
		}
		cmdReceiver.receiveCommand(cmd);

		// 销毁回收命令
		mCommandPool.destroyCmd(cmd);
	}
	// delayExecute是命令延时执行的时间,默认为0,只有new出来的命令才能延时执行
	// 子线程中发出的命令必须是延时执行的命令!
	public void pushDelayCommand(Command cmd, CommandReceiver cmdReceiver, float delayExecute = 0.001f)
	{
		if (cmd == null || cmdReceiver == null)
		{
			UnityUtility.logError("cmd or receiver is null!");
			return;
		}
		if(!cmd.isValid())
		{
			UnityUtility.logError("cmd is invalid! make sure create cmd use CommandSystem.newCmd!");
			return;
		}
		if(!cmd.isDelayCommand())
		{
			UnityUtility.logError("cmd is not a delay command, Command : " + cmd.showDebugInfo());
			return;
		}
		if (delayExecute < 0.0f)
		{
			delayExecute = 0.0f;
		}
		if (cmd.getShowDebugInfo())
		{
			UnityUtility.logInfo("CommandSystem : delay cmd : " + delayExecute + ", info : " + cmd.showDebugInfo() + ", receiver : " + cmdReceiver.getName(), LOG_LEVEL.LL_NORMAL);
		}
		DelayCommand delayCommand = new DelayCommand(delayExecute, cmd, cmdReceiver);
		
		mBufferLock.waitForUnlock(LOCK_TYPE.LT_READ);
		mCommandBufferInput.Add(delayCommand);
		mBufferLock.unlock(LOCK_TYPE.LT_READ);
	}
	public void destroy()
	{
		mCommandPool.destroy();
		mCommandBufferInput.Clear();
		mCommandBufferProcess.Clear();
	}
	public virtual void notifyReceiverDestroied(CommandReceiver receiver)
	{
		mBufferLock.waitForUnlock(LOCK_TYPE.LT_READ);
		for (int i = 0; i < mCommandBufferInput.Count; ++i)
		{
			if (mCommandBufferInput[i].mReceiver == receiver)
			{
				// 命令的延迟执行时间到了,则执行命令
				mCommandBufferInput.Remove(mCommandBufferInput[i]);
				--i;
			}
		}
		mBufferLock.unlock(LOCK_TYPE.LT_READ);

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
	//------------------------------------------------------------------------------------------------------------------------------------
	protected void destroyCmd(Command cmd)
	{
		if(!cmd.isValid())
		{
			UnityUtility.logError("cmd is invalid, can not destroy it!");
			return;
		}
	}
}