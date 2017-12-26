using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class SceneProcedure : GameBase
{
	protected List<int>			mDelayCmdList;	// 流程进入时的延迟命令列表,当命令执行时,会从列表中移除该命令
	protected Dictionary<PROCEDURE_TYPE, SceneProcedure> mChildProcedureList;	// 子流程列表
	protected PROCEDURE_TYPE	mProcedureType;			// 该流程的类型
	protected GameScene			mGameScene;				// 流程所属的场景
	protected SceneProcedure	mParentProcedure;		// 父流程
	protected SceneProcedure	mCurChildProcedure;		// 当前正在运行的子流程
	protected bool				mInited;				// 是否已经初始化,子节点在初始化时需要先确保父节点已经初始化
	// 以下变量为准备退出时使用的
	protected float				mExitTime;				// 从准备退出到真正退出流程所需要的时间,小于等于0表示不需要准备退出
	protected float				mCurPrepareTime;        // 准备退出的计时,大于等于0表示正在准备退出,小于0表示没有正在准备退出
	protected SceneProcedure	mPrepareNext;
	protected string			mPrepareIntent;
	public SceneProcedure()
	{
		UnityUtility.logError("error : can not create SceneProcedure without parameters!");
	}
	public SceneProcedure(PROCEDURE_TYPE type, GameScene gameScene)
	{
		mInited = false;
		mProcedureType = type;
		mGameScene = gameScene;
		mParentProcedure = null;
		mCurChildProcedure = null;
		mDelayCmdList = new List<int>();
		mChildProcedureList = new Dictionary<PROCEDURE_TYPE, SceneProcedure>();
		mCurPrepareTime = -1.0f;
		mExitTime = -1.0f;
		mPrepareNext = null;
		mPrepareIntent = "";
	}
	// 从自己的子流程进入当前流程
	protected virtual void onInitFromChild(SceneProcedure lastProcedure, string intent) { }
	// 在进入流程时调用
	// 在onInit中如果要跳转流程,必须使用延迟命令进行跳转
	protected abstract void onInit(SceneProcedure lastProcedure, string intent);
	// 更新流程时调用
	protected abstract void onUpdate(float elapsedTime);
	// 更新流程时调用
	protected abstract void onKeyProcess(float elapsedTime);
	// 退出当前流程,进入的不是自己的子流程时调用
	protected abstract void onExit(SceneProcedure nextProcedure);
	// 退出当前流程,进入自己的子流程时调用
	protected virtual void onExitToChild(SceneProcedure nextProcedure) { }
	// 退出当前流程进入其他任何流程时调用
	protected virtual void onExitSelf() { }
	// 进入的目标流程已经准备完成(资源加载完毕等等)时的回调
	public virtual void onNextProcedurePrepared(SceneProcedure nextPreocedure) { }
	protected virtual void onPrepareExit(SceneProcedure nextPreocedure) { }
	// 由GameScene调用
	// 进入流程
	public void init(SceneProcedure lastProcedure, string intent)
	{
		// 如果父节点还没有初始化,则先初始化父节点
		if (mParentProcedure != null && !mParentProcedure.mInited)
		{
			mParentProcedure.init(lastProcedure, intent);
		}
		// 再初始化自己,如果是从子节点返回到父节点,则需要调用另外一个初始化函数
		if(lastProcedure != null && lastProcedure.isThisOrParent(mProcedureType))
		{
			onInitFromChild(lastProcedure, intent);
		}
		else
		{
			onInit(lastProcedure, intent);
		}
		mInited = true;
	}
	// 更新流程
	public void update(float elapsedTime)
	{
		// 先更新父节点
		if (mParentProcedure != null)
		{
			mParentProcedure.update(elapsedTime);
		}
		// 再更新自己
		onUpdate(elapsedTime);

		// 正在准备退出流程时,累计时间,
		if (mCurPrepareTime >= 0.0f)
		{
			mCurPrepareTime += elapsedTime;
			if(mCurPrepareTime >= mExitTime)
			{
				// 超过了准备时间,强制跳转流程
				CommandGameSceneChangeProcedure cmd = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>();
				cmd.mProcedure = mPrepareNext.getProcedureType();
				cmd.mIntent = mPrepareIntent;
				cmd.mForceChange = true;
				mCommandSystem.pushCommand(cmd, mGameScene);
			}
		}
	}
	// 退出流程
	public void exit(SceneProcedure exitTo, SceneProcedure nextPro)
	{
		// 中断自己所有未执行的命令
		int count = mDelayCmdList.Count;
		for(int i = 0; i < count; ++i)
		{
			mCommandSystem.interruptCommand(mDelayCmdList[i]);
		}
		mDelayCmdList.Clear();

		// 当停止目标为自己时,则不再退出,此时需要判断当前将要进入的流程是否为当前流程的子流程
		// 如果是,则需要调用onExitToChild,执行退出当前并且进入子流程的操作
		// 如果不是则不需要调用,不需要执行任何退出操作
		if (this == exitTo)
		{
			if (nextPro != null && nextPro.isThisOrParent(mProcedureType))
			{
				onExitToChild(nextPro);
				onExitSelf();
			}
			return;
		}
		// 先退出自己
		onExit(nextPro);
		onExitSelf();
		mInited = false;
		// 再退出父节点
		if (mParentProcedure != null)
		{
			mParentProcedure.exit(exitTo, nextPro);
		}
		// 退出完毕后就修改标记
		mCurPrepareTime = -1.0f;
		mPrepareNext = null;
		mPrepareIntent = "";
	}
	public void prepareExit(SceneProcedure next, string intent)
	{
		mCurPrepareTime = 0.0f;
		mPrepareNext = next;
		mPrepareIntent = intent;
		// 通知自己准备退出
		onPrepareExit(next);
	}
	public void keyProcess(float elapsedTime)
	{
		// 在准备退出当前流程时,不响应任何按键操作
		if(isPreparingExit())
		{
			return;
		}
		// 先处理父节点按键响应
		if (mParentProcedure != null)
		{
			mParentProcedure.keyProcess(elapsedTime);
		}
		// 然后再处理自己的按键响应
		onKeyProcess(elapsedTime);
	}
	public void addDelayCmd(Command cmd)
	{
		mDelayCmdList.Add(cmd.mAssignID);
		cmd.addStartCommandCallback(onCmdStarted, this);
	}
	public void getParentList(ref List<SceneProcedure> parentList)
	{
		// 由于父节点列表中需要包含自己,所以先加入自己
		parentList.Add(this);
		// 再加入父节点的所有父节点
		if (mParentProcedure != null)
		{
			mParentProcedure.getParentList(ref parentList);
		}
	}
	// 获得自己和otherProcedure的共同的父节点
	public SceneProcedure getSameParent(SceneProcedure otherProcedure)
	{
		// 获得两个流程的父节点列表
		List<SceneProcedure> thisParentList = new List<SceneProcedure>();
		List<SceneProcedure> otherParentList = new List<SceneProcedure>();
		getParentList(ref thisParentList);
		otherProcedure.getParentList(ref otherParentList);
		// 从前往后判断,找到第一个相同的父节点
		foreach (var thisParent in thisParentList)
		{
			foreach (var otherParent in otherParentList)
			{
				if(thisParent == otherParent)
				{
					return thisParent;
				}
			}
		}
		return null;
	}
	public bool isThisOrParent(PROCEDURE_TYPE type)
	{
		// 判断是否是自己的类型
		if (mProcedureType == type)
		{
			return true;
		}
		// 判断是否为父节点的类型
		if (mParentProcedure != null)
		{
			return mParentProcedure.isThisOrParent(type);
		}
		// 没有父节点,返回false
		return false;
	}
	public PROCEDURE_TYPE getProcedureType() { return mProcedureType; }
	public GameScene getGameScene() { return mGameScene; }
	public SceneProcedure getParent() { return mParentProcedure; }
	// 是否正在准备退出流程
	public bool isPreparingExit() { return mCurPrepareTime >= 0.0f; }
	// 是否为具有准备退出的流程
	public bool hasPrepareExit() { return mExitTime > 0.0f; }
	public SceneProcedure getParent(PROCEDURE_TYPE type)
	{
		// 没有父节点,返回null
		if(mParentProcedure == null)
		{
			return null;
		}
		// 有父节点,则判断类型是否匹配,匹配则返回父节点
		if (mParentProcedure.getProcedureType() == type)
		{
			return mParentProcedure;
		}
		// 不匹配,则继续向上查找
		else
		{
			return mParentProcedure.getParent(type);
		}
	}
	public T getThisOrParent<T>(PROCEDURE_TYPE type) where T : SceneProcedure
	{
		if (mProcedureType == type)
		{
			return this as T;
		}
		else
		{
			SceneProcedure parent = getParent(type);
			if(parent != null)
			{
				return parent as T;
			}
		}
		return null;
	}
	public SceneProcedure getCurChildProcedure() { return mCurChildProcedure; }
	public SceneProcedure getChildProcedure(PROCEDURE_TYPE type)
	{
		if (mChildProcedureList.ContainsKey(type))
		{
			return mChildProcedureList[type];
		}
		return null;
	}
	public bool addChildProcedure(SceneProcedure child)
	{
		if (child == null)
		{
			return false;
		}
		if (mChildProcedureList.ContainsKey(child.getProcedureType()))
		{
			return false;
		}
		child.setParent(this);
		mChildProcedureList.Add(child.getProcedureType(), child);
		return true;
	}
	//---------------------------------------------------------------------------------------------------------
	protected bool setParent(SceneProcedure parent)
	{
		if (mParentProcedure != null)
		{
			return false;
		}
		mParentProcedure = parent;
		return true;
	}
	protected void onCmdStarted(object userdata, Command cmd)
	{
		if(!mDelayCmdList.Remove(cmd.mAssignID))
		{
			UnityUtility.logError("命令执行后移除流程命令失败");
		}
	}
}