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

public class CommandSystem : FrameComponent
{
	protected CommandPool mCommandPool;
	protected List<DelayCommand> mCommandBufferProcess;	// 用于处理的命令列表
	protected List<DelayCommand> mCommandBufferInput;	// 用于放入命令的命令列表
	protected List<DelayCommand> mExecuteList;			// 即将在这一帧执行的命令
	protected ThreadLock mBufferLock;
	protected bool mTraceCommand;   // 是否追踪命令的来源
	protected bool mSystemDestroy;	// 命令系统是否已经销毁
	public CommandSystem(string name)
		:base(name)
	{
		mBufferLock = new ThreadLock();
		mTraceCommand = false;
		mCommandPool = new CommandPool();
		mCommandBufferProcess = new List<DelayCommand>();
		mCommandBufferInput = new List<DelayCommand>();
		mExecuteList = new List<DelayCommand>();
		mSystemDestroy = false;
	}
	public override void init()
	{
		mCommandPool.init();
	}
	public override void destroy()
	{
		mCommandPool.destroy();
		mCommandBufferInput.Clear();
		mCommandBufferProcess.Clear();
		mSystemDestroy = true;
		base.destroy();
	}
	protected void syncCommandBuffer()
	{
		mBufferLock.waitForUnlock();
		int inputCount = mCommandBufferInput.Count;
		for (int i = 0; i < inputCount; ++i)
		{
			mCommandBufferProcess.Add(mCommandBufferInput[i]);
		}
		mCommandBufferInput.Clear();
		mBufferLock.unlock();
	}
	public override void update(float elapsedTime)
	{
		// 同步命令输入列表到命令处理列表中
		syncCommandBuffer();

		// 如果一帧时间大于1秒,则认为是无效更新
		if (elapsedTime >= 1.0f)
		{
			return;
		}
		// 执行之前需要先清空列表
		mExecuteList.Clear();
		// 开始处理命令处理列表
		for (int i = 0; i < mCommandBufferProcess.Count; ++i)
		{
			mCommandBufferProcess[i].mDelayTime -= elapsedTime;
			if (mCommandBufferProcess[i].mDelayTime <= 0.0f)
			{
				// 命令的延迟执行时间到了,则执行命令
				mExecuteList.Add(mCommandBufferProcess[i]);
				mCommandBufferProcess.Remove(mCommandBufferProcess[i]);
				--i;
			}
		}
		int executeCount = mExecuteList.Count;
		for (int i = 0; i < executeCount; ++i)
		{
			mExecuteList[i].mCommand.setDelayCommand(false);
			if(mExecuteList[i].mReceiver != null)
			{
				pushCommand(mExecuteList[i].mCommand, mExecuteList[i].mReceiver);
			}
		}
		// 执行完后清空列表
		mExecuteList.Clear();
	}
	// 创建命令
	public T newCmd<T>(bool show = true, bool delay = false) where T : Command, new()
	{
		T cmd = mCommandPool.newCmd<T>(show, delay);
#if UNITY_EDITOR
		if (mTraceCommand)
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
		// 如果命令系统已经销毁了,则不能再中断命令
		if(mSystemDestroy)
		{
			return true;
		}
		if (assignID < 0)
		{
			UnityUtility.logError("assignID invalid! : " + assignID);
			return false;
		}
		syncCommandBuffer();
		foreach (var item in mCommandBufferProcess)
		{
			if (item.mCommand.mAssignID == assignID)
			{
				UnityUtility.logInfo("CommandSystem : interrupt command " + assignID + " : " + item.mCommand.showDebugInfo() + ", receiver : " + item.mReceiver.getName(), LOG_LEVEL.LL_HIGH);
				mCommandBufferProcess.Remove(item);
				// 销毁回收命令
				mCommandPool.destroyCmd(item.mCommand);
				return true;
			}
		}
		// 在即将执行的列表中查找
		foreach (var item in mExecuteList)
		{
			if (item.mCommand.mAssignID == assignID)
			{
				UnityUtility.logError("cmd is in execute list! can not interrupt!");
				break;
			}
		}
		UnityUtility.logError("not find cmd with assignID! " + assignID);
		return false;
	}
	public new void pushCommand<T>(CommandReceiver cmdReceiver, bool show = true) where T : Command, new()
	{
		T cmd = newCmd<T>(show, false);
		pushCommand(cmd, cmdReceiver);
	}
	// 执行命令
	public new void pushCommand(Command cmd, CommandReceiver cmdReceiver)
	{
		if (cmd == null || cmdReceiver == null)
		{
			UnityUtility.logError("cmd or receiver is null!");
			return;
		}
		if (!cmd.isValid())
		{
			UnityUtility.logError("cmd is invalid! make sure create cmd use CommandSystem.newCmd! pushCommand cmd type : "
				+ cmd.GetType().ToString() + "cmd id : " + cmd.mAssignID);
			return;
		}
		if (cmd.isDelayCommand())
		{
			UnityUtility.logError("cmd is a delay cmd! can not use pushCommand!" + cmd.mAssignID + ", " + cmd.showDebugInfo());
			return;
		}
		cmd.setReceiver(cmdReceiver);
		if (cmd.getShowDebugInfo())
		{
			UnityUtility.logInfo("CommandSystem : " + cmd.mAssignID + ", " + cmd.showDebugInfo() + ", receiver : " + cmdReceiver.getName(), LOG_LEVEL.LL_NORMAL);
		}
		cmdReceiver.receiveCommand(cmd);

		// 销毁回收命令
		mCommandPool.destroyCmd(cmd);
	}
	public new void pushDelayCommand<T>(CommandReceiver cmdReceiver, float delayExecute = 0.001f, bool show = true) where T : Command, new()
	{
		T cmd = newCmd<T>(show, true);
		pushDelayCommand(cmd, cmdReceiver, delayExecute);
	}
	// delayExecute是命令延时执行的时间,默认为0,只有new出来的命令才能延时执行
	// 子线程中发出的命令必须是延时执行的命令!
	public new void pushDelayCommand(Command cmd, CommandReceiver cmdReceiver, float delayExecute = 0.001f)
	{
		if (cmd == null || cmdReceiver == null)
		{
			UnityUtility.logError("cmd or receiver is null!");
			return;
		}
		if (!cmd.isValid())
		{
			UnityUtility.logError("cmd is invalid! make sure create cmd use CommandSystem.newCmd! pushDelayCommand cmd type : "
				+ cmd.GetType().ToString() + "cmd id : " + cmd.mAssignID);
			return;
		}
		if (!cmd.isDelayCommand())
		{
			UnityUtility.logError("cmd is not a delay command, Command : " + cmd.mAssignID + ", " + cmd.showDebugInfo());
			return;
		}
		if (delayExecute < 0.0f)
		{
			delayExecute = 0.0f;
		}
		if (cmd.getShowDebugInfo())
		{
			UnityUtility.logInfo("CommandSystem : delay cmd : " + cmd.mAssignID + ", " + delayExecute + ", info : " + cmd.showDebugInfo() + ", receiver : " + cmdReceiver.getName(), LOG_LEVEL.LL_NORMAL);
		}
		DelayCommand delayCommand = new DelayCommand(delayExecute, cmd, cmdReceiver);

		mBufferLock.waitForUnlock();
		mCommandBufferInput.Add(delayCommand);
		mBufferLock.unlock();
	}
	public virtual void notifyReceiverDestroied(CommandReceiver receiver)
	{
		if(mSystemDestroy)
		{
			return;
		}
		// 异步列表中
		mBufferLock.waitForUnlock();
		for (int i = 0; i < mCommandBufferInput.Count; ++i)
		{
			if (mCommandBufferInput[i].mReceiver == receiver)
			{
				mCommandBufferInput.Remove(mCommandBufferInput[i]);
				--i;
			}
		}
		mBufferLock.unlock();
		// 同步列表中
		for (int i = 0; i < mCommandBufferProcess.Count; ++i)
		{
			if (mCommandBufferProcess[i].mReceiver == receiver)
			{
				mCommandBufferProcess.Remove(mCommandBufferProcess[i]);
				--i;
			}
		}
		// 执行列表中
		int count = mExecuteList.Count;
		for(int i = 0; i < count; ++i)
		{
			// 已执行或正在执行的命令不作判断,该列表无法删除元素,只能将接收者设置为null
			if (mExecuteList[i].mReceiver == receiver && mExecuteList[i].mCommand.mExecuteState == EXECUTE_STATE.ES_NOT_EXECUTE)
			{
				mExecuteList[i].mReceiver = null;
			}
		}
	}
}