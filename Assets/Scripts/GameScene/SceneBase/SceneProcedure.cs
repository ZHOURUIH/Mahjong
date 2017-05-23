using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class SceneProcedure : GameBase
{
	protected Dictionary<Command, float> mDelayCmdList;	// 流程进入时的延迟命令列表,当命令执行时,会从列表中移除该命令
	protected Dictionary<PROCEDURE_TYPE, SceneProcedure> mChildProcedureList;	// 子流程列表
	protected PROCEDURE_TYPE	mType;					// 该流程的类型
	protected GameScene			mGameScene;				// 流程所属的场景
	protected SceneProcedure	mParentProcedure;		// 父流程
	protected SceneProcedure	mCurChildProcedure;		// 当前正在运行的子流程
	protected bool				mInited;				// 是否已经初始化,子节点在初始化时需要先确保父节点已经初始化
	public SceneProcedure()
	{
		UnityUtility.logError("error : can not create SceneProcedure without parameters!");
	}
	public SceneProcedure(PROCEDURE_TYPE type, GameScene gameScene)
	{
		mInited = false;
		mType = type;
		mGameScene = gameScene;
		mParentProcedure = null;
		mCurChildProcedure = null;
		mDelayCmdList = new Dictionary<Command, float>();
		mChildProcedureList = new Dictionary<PROCEDURE_TYPE, SceneProcedure>();
	}
	// 在onInit中如果要跳转流程,必须使用延迟命令进行跳转
	protected abstract void onInit(SceneProcedure lastProcedure, string intent);
	protected abstract void onUpdate(float elapsedTime);
	protected abstract void onKeyProcess(float elapsedTime);
	protected abstract void onExit(SceneProcedure nextProcedure);
	protected virtual void onBack() { }
	// 由GameScene调用
	// 进入流程
	public void init(SceneProcedure lastProcedure, string intent)
	{
		// 如果父节点还没有初始化,则先初始化父节点
		if (mParentProcedure != null && !mParentProcedure.mInited)
		{
			mParentProcedure.init(lastProcedure, intent);
		}
		// 再初始化自己
		onInit(lastProcedure, intent);
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
	}
	// 退出流程
	public void exit(SceneProcedure exitTo, SceneProcedure nextPro)
	{
		// 中断自己所有未执行的命令
		foreach (var cmd in mDelayCmdList)
		{
			mCommandSystem.interruptCommand(cmd.Key);
		}
		mDelayCmdList.Clear();

		// 当停止目标为自己时,则不再退出
		if(this == exitTo)
		{
			return;
		}
		// 先退出自己
		onExit(nextPro);
		mInited = false;
		// 再退出父节点
		if (mParentProcedure != null)
		{
			mParentProcedure.exit(exitTo, nextPro);
		}
	}
	// 返回到自己进入之前的状态
	public void back(SceneProcedure backTo)
	{
		// 中断自己所有未执行的命令
		foreach (var cmd in mDelayCmdList)
		{
			mCommandSystem.interruptCommand(cmd.Key);
		}
		mDelayCmdList.Clear();

		// 当返回目标为自己时,则不再返回
		if (this == backTo)
		{
			return;
		}
		// 先返回自己进入之前的状态
		onBack();
		mInited = false;
		// 再返回父节点进入之前的状态
		if (mParentProcedure != null)
		{
			mParentProcedure.back(backTo);
		}
	}
	public void keyProcess(float elapsedTime)
	{
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
		mDelayCmdList.Add(cmd, 0.0f);
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
		if(mType == type)
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
	public PROCEDURE_TYPE getType() { return mType; }
	public GameScene getGameScene() { return mGameScene; }
	public SceneProcedure getParent() { return mParentProcedure; }
	public SceneProcedure getParent(PROCEDURE_TYPE type)
	{
		// 没有父节点,返回null
		if(mParentProcedure == null)
		{
			return null;
		}
		// 有父节点,则判断类型是否匹配,匹配则返回父节点
		if(mParentProcedure.getType() == type)
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
		if(mType == type)
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
		if (mChildProcedureList.ContainsKey(child.getType()))
		{
			return false;
		}
		child.setParent(this);
		mChildProcedureList.Add(child.getType(), child);
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
		mDelayCmdList.Remove(cmd);
	}
}