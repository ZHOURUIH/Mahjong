using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Command : GameBase
{
	public CommandReceiver			mReceiver;		// 命令接受者
	public bool						mShowDebugInfo;	// 是否显示调试信息
	public bool						mDelayCommand;	// 是否是延迟执行的命令
	public List<CommandCallback>	mEndCallback;	// 命令执行完毕时的回调函数
	public List<CommandCallback>	mStartCallback;	// 命令开始执行时的回调函数
	public List<object>				mEndUserData;
	public List<object>				mStartUserData;

	public Command(bool showInfo = true, bool delay = false)
	{
		mReceiver = null;
		mShowDebugInfo = showInfo;
		mEndCallback = new List<CommandCallback>();
		mStartCallback = new List<CommandCallback>();
		mEndUserData = new List<object>();
		mStartUserData = new List<object>();
		mDelayCommand = delay;
	}
	// 命令执行
	public abstract void execute();
	// 调试信息，由CommandSystem调用
	public virtual string showDebugInfo() 
	{
		return this.GetType().ToString();
	}
	public bool getShowDebugInfo() { return mShowDebugInfo; }
	public bool isDelayCommand() { return mDelayCommand; }
	public void setDelayCommand(bool delay) { mDelayCommand = delay; }
	public void setReceiver(CommandReceiver Reciver) { mReceiver = Reciver; }
	public CommandReceiver getReceiver() { return mReceiver; }
	public void addEndCommandCallback(CommandCallback cmdCallback, object userdata)
	{
		if (cmdCallback != null)
		{
			mEndCallback.Add(cmdCallback);
			mEndUserData.Add(userdata);
		}
	}
	public void addStartCommandCallback(CommandCallback cmdCallback, object userdata)
	{
		if (cmdCallback != null)
		{
			mStartCallback.Add(cmdCallback);
			mStartUserData.Add(userdata);
		}
	}
	public void runEndCallBack()
	{
		int callbackCount = mEndCallback.Count;
		for (int i = 0; i < callbackCount; ++i)
		{
			mEndCallback[i](mEndUserData[i], this);
		}
		mEndCallback.Clear();
		mEndUserData.Clear();
	}

	public void runStartCallBack()
	{
		int callbackCount = mStartCallback.Count;
		for (int i = 0; i < callbackCount; ++i)
		{
			mStartCallback[i](mStartUserData[i], this);
		}
		mStartCallback.Clear();
		mStartUserData.Clear();
	}
}