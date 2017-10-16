using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EXECUTE_STATE
{
	ES_NOT_EXECUTE,
	ES_EXECUTING,
	ES_EXECUTED,
}

public abstract class Command : GameBase
{
	public CommandReceiver			mReceiver;		// 命令接受者
	public bool						mShowDebugInfo;	// 是否显示调试信息
	public bool						mDelayCommand;	// 是否是延迟执行的命令
	public bool						mValid;			// 是否为有效命令
	public EXECUTE_STATE			mExecuteState;	// 命令执行状态
	public List<CommandCallback>	mEndCallback;	// 命令执行完毕时的回调函数
	public List<CommandCallback>	mStartCallback;	// 命令开始执行时的回调函数
	public List<object>				mEndUserData;
	public List<object>				mStartUserData;
	public Type						mType;
	public int						mLine;
	public string					mFile;
	public int						mCmdID;
	public int						mAssignID;		// 重新分配时的ID,每次分配都会设置一个新的唯一执行ID
	public Command()
	{
		mReceiver = null;
		mEndCallback = new List<CommandCallback>();
		mStartCallback = new List<CommandCallback>();
		mEndUserData = new List<object>();
		mStartUserData = new List<object>();
		mValid = false;
		mAssignID = -1;
	}
	public virtual void init()
	{
		mReceiver = null;
		mShowDebugInfo = true;
		mDelayCommand = false;
		mValid = false;
		mExecuteState = EXECUTE_STATE.ES_NOT_EXECUTE;
		mEndCallback.Clear();
		mStartCallback.Clear();
		mEndUserData.Clear();
		mStartUserData.Clear();
	}
	// 命令执行
	public abstract void execute();
	// 调试信息，由CommandSystem调用
	public virtual string showDebugInfo() 
	{
		return this.GetType().ToString();
	}
	public bool getShowDebugInfo()					{ return mShowDebugInfo; }
	public bool isDelayCommand()					{ return mDelayCommand; }
	public CommandReceiver getReceiver()			{ return mReceiver; }
	public bool isValid()							{ return mValid; }
	public Type getType()							{ return mType; }
	public EXECUTE_STATE getExecuteState()			{ return mExecuteState; }
	public void setShowDebugInfo(bool show)			{ mShowDebugInfo = show; }
	public void setDelayCommand(bool delay)			{ mDelayCommand = delay; }
	public void setReceiver(CommandReceiver Reciver){ mReceiver = Reciver; }
	public void setValid(bool valid)				{ mValid = valid;}
	public void setType(Type type)					{ mType = type; }
	public void setExecuteState(EXECUTE_STATE state){ mExecuteState = state; }
	// 被分配为延迟命令时的唯一ID
	public void setAssignID(int id)					{ mAssignID = id; }
	public void setID(int id) { mCmdID = id; }
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